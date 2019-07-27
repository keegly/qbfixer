using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QBFC13Lib;
using LinqToExcel;
using System.Diagnostics;
using System.Threading;


namespace qbfixer
{
    public partial class Form1 : Form
    {
        const string COMPANY_FILE = @"C:\Users\Public\Documents\Intuit\QuickBooks\Company Files\Bluefire Plumbing & Gasfitting.qbw";
        string EXCEL_FILE = "";

        static bool _sessionBegun = false;
        static bool _connectionOpen = false;
        static bool _failed = false;
        static QBSessionManager sessionManager = null;
        static List<Invoice> _invoices = null;
        static List<Invoice> _unpaidInvoices = new List<Invoice>();
        BackgroundWorker _bw;



        public Form1()
        {
            InitializeComponent();

            // Setup our backgroundWorker for our more time consuming methods
            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.ProgressChanged += bw_ProgressChanged;

            // Hide the progress bar
            progressBar1.Hide();
            // Disable the button until we've imported some invoices to process
            import_cheque_btn.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Get the list of customers, so we can choose whom to apply the payment to
            GetCustomers();
        }

        private void load_spreadsheet_btn_Click(object sender, EventArgs e)
        {
            // Open file dialog so we can pick the excel file
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                EXCEL_FILE = openFileDialog1.FileName;
                // Process file and get a list of invoices to be paid
                _invoices = ImportXls();
                if (_invoices != null)
                {
                    excel_file_lbl.Text = openFileDialog1.SafeFileName;
                    // enable the process button
                    import_cheque_btn.Enabled = true;
                }
            }
        }

        private List<Invoice> ImportXls()
        {
            var excel = new ExcelQueryFactory();
            excel.FileName = EXCEL_FILE;
            excel.AddMapping<Invoice>(x => x.Probill, "PROBILL");
            excel.AddMapping<Invoice>(x => x.ChequeNumber, "CHEQUE NUMBER");
            excel.AddMapping<Invoice>(x => x.Total, "TOTAL AMOUNT");

            try
            {
                var probills = from x in excel.Worksheet<Invoice>()
                               select x;

                // In case we're doing more than one batch per opening of program, 
                // clear the listview each time we import a spreadsheet
                invoices_listview.Items.Clear();

                foreach (Invoice i in probills)
                {
                    // Skip displaying any worth $0
                    if (i.Total == 0) continue;
                    if (i.Probill == null) continue;
                    ListViewItem lvi = new ListViewItem(i.Probill);
                    lvi.SubItems.Add(String.Format("${0, 3:0.00}", i.Total));
                    invoices_listview.Items.Add(lvi);
                }

                return probills.ToList();
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                // TODO: Log exception
                MessageBox.Show("There was an error importing the Excel file. (" + e.Message + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Failed to import any invoices
            return null;
        }

        private void GetCustomers()
        {
            sessionManager = new QBSessionManager();
            sessionManager.OpenConnection("appID", "Bluefire Integration TEST");
            _connectionOpen = true;
            sessionManager.BeginSession(COMPANY_FILE, ENOpenMode.omDontCare);
            _sessionBegun = true;

            // Grab the list of customer names to populate our listbox
            IMsgSetRequest messageSet = sessionManager.CreateMsgSetRequest("CA", 12, 0);
            ICustomerQuery custQuery = messageSet.AppendCustomerQueryRq();

            try
            {
                IMsgSetResponse responseSet = sessionManager.DoRequests(messageSet);
                sessionManager.EndSession();
                _sessionBegun = false;

                IResponse response;
                ENResponseType responseType;

                for (int i = 0; i < responseSet.ResponseList.Count; i++)
                {
                    response = responseSet.ResponseList.GetAt(i);

                    if (response.Detail == null)
                        continue;

                    responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtCustomerQueryRs)
                    {
                        ICustomerRetList custList = (ICustomerRetList)response.Detail;
                        for (int custIndex = 0; custIndex < custList.Count; custIndex++)
                        {
                            ICustomerRet customer = (ICustomerRet)custList.GetAt(custIndex);

                            if (customer != null && customer.CompanyName != null)
                                customer_listbox.Items.Add(customer.CompanyName.GetValue());
                        }
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                // something bad happened; tell the user, smash his computer, whatever; its your choice
                MessageBox.Show(comEx.Message);
            }


        }

        private void quit_btn_Click(object sender, EventArgs e)
        {
            if (_sessionBegun)
            {
                sessionManager.EndSession();
            }
            if (_connectionOpen)
            {
                sessionManager.CloseConnection();
            }

            this.Close();
        }

        private void import_cheque_btn_Click(object sender, EventArgs e)
        {
            // Bail out if a customer hasn;t been selected yet
            if (customer_listbox.SelectedItem == null)
            {
                MessageBox.Show("Select a customer first.");
                return;
            }
            else if (EXCEL_FILE == "")
            {
                MessageBox.Show("Select a properly formatted excel worksheet before continuing.");
                return;
            }

            string customer = customer_listbox.SelectedItem.ToString();
            // Show the progress bar to illustrate that we are working
            progressBar1.Show();

            // Disable the button while we crunch
            import_cheque_btn.Enabled = false;
            _bw.RunWorkerAsync(customer);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_invoices == null) return; // Haven't parsed any invoices yet, or there was an error

            sessionManager.BeginSession(COMPANY_FILE, ENOpenMode.omDontCare);
            _sessionBegun = true;
            _failed = false;

            // Grab the list of unpaid invoices from the selected customer
            IMsgSetRequest messageSet = sessionManager.CreateMsgSetRequest("CA", 12, 0);
            IInvoiceQuery invoiceQuery = messageSet.AppendInvoiceQueryRq();
            invoiceQuery.ORInvoiceQuery.InvoiceFilter.PaidStatus.SetValue(ENPaidStatus.psNotPaidOnly);
            invoiceQuery.ORInvoiceQuery.InvoiceFilter.EntityFilter.OREntityFilter.FullNameList.Add(e.Argument.ToString());

            try
            {
                IMsgSetResponse responseSet = sessionManager.DoRequests(messageSet);

                IResponse response;
                ENResponseType responseType;

                for (int i = 0; i < responseSet.ResponseList.Count; i++)
                {
                    response = responseSet.ResponseList.GetAt(i);

                    if (response.Detail == null)
                        continue;

                    responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtInvoiceQueryRs)
                    {
                        IInvoiceRetList invoiceList = (IInvoiceRetList)response.Detail;

                        for (int invoiceIndex = 0; invoiceIndex < invoiceList.Count; invoiceIndex++)
                        {
                            IInvoiceRet invoice = (IInvoiceRet)invoiceList.GetAt(invoiceIndex);

                            if (invoice != null)
                            {
                                //unpaid_listbox.Items.Add(invoice.CustomerRef.FullName.GetValue() + " - " + invoice.RefNumber.GetValue() + " ( " + invoice.TxnID.GetValue() + " )");
                                _unpaidInvoices.Add(new Invoice(invoice.RefNumber.GetValue(), invoice.TxnID.GetValue()));
                                int index = _invoices.FindIndex(x => x.Probill == invoice.RefNumber.GetValue());

                                if (index >= 0)
                                {
                                    _invoices[index].TxnID = invoice.TxnID.GetValue();
                                    Trace.WriteLine("Applying TxnID " + invoice.TxnID.GetValue() + " to Invoice " + _invoices[index].Probill);
                                }
                            }
                        }
                    }
                }

                messageSet = sessionManager.CreateMsgSetRequest("CA", 12, 0);
                messageSet.Attributes.OnError = ENRqOnError.roeContinue;


                IReceivePaymentAdd payment = messageSet.AppendReceivePaymentAddRq();

                // Populate the request with all the details
                payment.CustomerRef.FullName.SetValue(e.Argument.ToString());
                payment.RefNumber.SetValue(_invoices.First().ChequeNumber);
                payment.PaymentMethodRef.FullName.SetValue("Cheque");
                IAppliedToTxnAdd txn;
                double totalAmt = 0.0;
                int count = 0;

                foreach (Invoice i in _invoices)
                {
                    // Don't add payments that are negative (IE. back charges, etc)
                    if (i.Total < 0)
                    {
                        Trace.WriteLine(String.Format("Rejecting negative transaction {0} for ${1}", i.Probill, i.Total));
                        // Subtract it from our total
                        // TODO: add an expense?
                        //totalAmt -= i.Total;
                        continue;
                    }
                    if (i.Total == 0)
                    {
                        // Discard any empty entries
                        Trace.WriteLine("Rejecting empty entry");
                        continue;
                    }
                    if (i.TxnID == "")
                    {
                        // Discard any invalid entries (couldn't find an unpaid invoice that matches)
                        Trace.WriteLine("Rejecting entry with no TxnID");
                        continue;
                    }

                    txn = payment.ORApplyPayment.AppliedToTxnAddList.Append();
                    txn.PaymentAmount.SetValue(i.Total);
                    txn.TxnID.SetValue(i.TxnID);
                    totalAmt += i.Total;
                    count += 1;
                    Trace.WriteLine("Adding Transaction of " + i.Total + " with TxnID " + i.TxnID + " ( " + count + " )");
                }

                Trace.WriteLine("Total amount received: " + totalAmt);
                // Round off the total to two decimal places, otherwise it will sometimes inexplicably fail 
                totalAmt = Math.Round(totalAmt, 2);
                payment.TotalAmount.SetValue(totalAmt);
                // TODO: fix hardcoded version number
                payment.Memo.SetValue("Automatically generated by qbFixer 0.1 beta");
                //payment.TxnDate.SetValue(DateTime.Today);

                responseSet = sessionManager.DoRequests(messageSet);



                for (int i = 0; i < responseSet.ResponseList.Count; i++)
                {
                    response = responseSet.ResponseList.GetAt(i);

                    if (response.StatusCode > 0)
                    {
                        MessageBox.Show(response.StatusMessage);
                        return;
                    }
                }

                _bw.ReportProgress(100);
                _bw.CancelAsync();
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                MessageBox.Show(comEx.Message);
                _failed = true;
            }
            finally
            {
                sessionManager.EndSession();
                _sessionBegun = false;
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Re enable button
            import_cheque_btn.Enabled = true;
            // TODO: Clear the listbox?
            if (!_failed)
            {
                MessageBox.Show("Payment processed successfully!");
            }
            progressBar1.Hide();
        }

    }
}
