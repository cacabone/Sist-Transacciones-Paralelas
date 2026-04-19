using BankSystem.Core.Models;

namespace BankSystem.ParallelProcessing
{
    public class TaxCalculator
    {
        private const decimal TAX_RATE = 0.0015m;

        public decimal Calculate(Transaction tx)
        {
            // Ejemplo: solo canal electrónico paga impuesto
            if (tx.Channel == TransactionChannel.Electronic)
            {
                return tx.Amount * TAX_RATE;
            }

            return 0;
        }
    }
}
