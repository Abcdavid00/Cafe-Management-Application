using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    internal class CashBox
    {
        private ulong _cafeID;

        private List<Transaction> _transactions;

        /// <summary>
        /// Add a transaction to CashBox.
        /// </summary>
        /// <param name="transaction">The transaction that need to add.</param>
        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }

        public IEnumerable<Transaction> GetTransactionOfStaff(ulong StaffID)
        {
            return _transactions.Where(x => x.StaffID == StaffID);
        }

    }
}
