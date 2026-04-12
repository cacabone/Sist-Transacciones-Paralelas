using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Interfaces
{
    public interface ITransactionProcessor
    {
        void ProcessTransactions(IEnumerable<Transaction> transactions, int threadCount);
    }
}
