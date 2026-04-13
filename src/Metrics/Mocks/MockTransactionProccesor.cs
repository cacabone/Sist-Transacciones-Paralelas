using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BankSystem.Core.interfaces;
using BankSystem.Core.Models;

namespace BankSystem.Metrics.Mocks
{
    public class mockTransactionProcessor : ITransactionProcessor
    {
    public void ProcessTransaction(IEnumerable<Transaction> transactions, int threadCount)
    {
        int count = transactions.Count();
        int baseTime =  count * 1;
        int simulatedTime = (baseTime / threadCount) + (threadCount * 5);

        ThreadSleep(simulatedTime);
    }
    }
}