using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qbfixer
{
    class Invoice
    {
        public string Probill { get; set; }
        public string ChequeNumber { get; set; }
        public double Total { get; set; }
        public string TxnID { get; set; }

        public Invoice(string Probill, string ChequeNumber, double Total)
        {
            this.Probill = Probill;
            this.ChequeNumber = ChequeNumber;
            this.Total = Total;
        }

        public Invoice(string Probill, string TxnID)
        {
            this.Probill = Probill;
            this.TxnID = TxnID;
        }

        public Invoice()
        {

        }
    }
}
