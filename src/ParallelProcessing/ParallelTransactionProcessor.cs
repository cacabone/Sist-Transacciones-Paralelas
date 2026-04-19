using BankSystem.Core.Interfaces;
using BankSystem.Core.Models;

namespace BankSystem.ParallelProcessing
{
    public class ParallelTransactionProcessor : ITransactionProcessor
    {
        private readonly IAccountRepository _repository;
        private readonly TaxCalculator _taxCalculator;

        public ParallelTransactionProcessor(IAccountRepository repository)
        {
            _repository = repository;
            _taxCalculator = new TaxCalculator();
        }

        public void ProcessTransactions(IEnumerable<Transaction> transactions)
        {
            Parallel.ForEach(transactions, transaction =>
            {
                ProcessSingle(transaction);
            });
        }

        private void ProcessSingle(Transaction tx)
        {
            decimal tax = _taxCalculator.Calculate(tx);

            switch (tx.Type)
            {
                case TransactionType.Deposit:
                    _repository.UpdateBalance(tx.SourceAccountId, tx.Amount - tax);
                    break;

                case TransactionType.Withdraw:
                    _repository.UpdateBalance(tx.SourceAccountId, -(tx.Amount + tax));
                    break;

                case TransactionType.Transfer:
                    ProcessTransfer(tx, tax);
                    break;
            }

            if (tax > 0)
            {
                _repository.AddTax(tax);
                tx.TaxApplied = true;
                tx.TaxAmount = tax;
            }
        }

        private void ProcessTransfer(Transaction tx, decimal tax)
        {
            //  No es atómico (limitación del diseño actual)
            _repository.UpdateBalance(tx.SourceAccountId, -(tx.Amount + tax));
            _repository.UpdateBalance(tx.DestinationAccountId, tx.Amount);
        }
    }
}
