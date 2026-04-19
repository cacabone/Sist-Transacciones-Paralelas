using BankSystem.Core.Interfaces;
using BankSystem.Core.Models;
using BankSystem.Core;
using System.Collections.Concurrent;

namespace BankSystem.ParallelProcessing
{
    public class ParallelTransactionProcessor : ITransactionProcessor
    {
        private readonly IAccountRepository _repository;
        private readonly TaxCalculator _taxCalculator;

        // Track transaction statistics using thread-safe counters
        private long _successfulTransactions;
        private long _failedTransactions;

        public long SuccessfulTransactions => Interlocked.Read(ref _successfulTransactions);
        public long FailedTransactions => Interlocked.Read(ref _failedTransactions);

        public ParallelTransactionProcessor(IAccountRepository repository)
        {
            _repository = repository;
            _taxCalculator = new TaxCalculator();
            _successfulTransactions = 0;
            _failedTransactions = 0;
        }

        public void ProcessTransactions(IEnumerable<Transaction> transactions, int threadCount)
        {
            // Reset counters
            Interlocked.Exchange(ref _successfulTransactions, 0);
            Interlocked.Exchange(ref _failedTransactions, 0);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = threadCount
            };

            Parallel.ForEach(transactions, parallelOptions, transaction =>
            {
                ProcessSingle(transaction);
            });
        }

        private void ProcessSingle(Transaction tx)
        {
            try
            {
                decimal tax = _taxCalculator.Calculate(tx);

                switch (tx.Type)
                {
                    case TransactionType.Deposito:
                        _repository.UpdateBalance(tx.SourceAccountId, tx.Amount - tax);
                        break;

                    case TransactionType.Retiro:
                        _repository.UpdateBalance(tx.SourceAccountId, -(tx.Amount + tax));
                        break;

                    case TransactionType.Transferencia:
                        ProcessTransfer(tx, tax);
                        break;
                }

                if (tax > 0)
                {
                    _repository.AddTax(tax);
                    tx.TaxApplied = true;
                    tx.TaxAmount = tax;
                }

                Interlocked.Increment(ref _successfulTransactions);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Fondos insuficientes"))
            {
                // Silently skip transactions with insufficient funds (realistic behavior)
                tx.TaxApplied = false;
                Interlocked.Increment(ref _failedTransactions);
            }
        }

        private void ProcessTransfer(Transaction tx, decimal tax)
        {
            // Transacciones de transferencia se procesan juntas o no se procesan
            _repository.UpdateBalance(tx.SourceAccountId, -(tx.Amount + tax));
            _repository.UpdateBalance(tx.DestinationAccountId, tx.Amount);
        }
    }
}
