using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    internal class Transaction
    {
        public readonly string TransactionID;
        public readonly long TransactionAmount;
        public readonly int StaffID;
        public readonly DateTime TimeStamp;
        public readonly string Note;

        public Transaction(string transactionID, long transactionAmount, int staffID, DateTime timeStamp, string note)
        {
            this.TransactionID = transactionID;
            this.TransactionAmount = transactionAmount;
            this.StaffID = staffID;
            this.TimeStamp = timeStamp;
            this.Note = note;
        }


    }
}
