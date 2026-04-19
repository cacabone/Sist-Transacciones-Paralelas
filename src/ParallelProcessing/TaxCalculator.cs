using BankSystem.Core.Models;
using BankSystem.Core;

namespace BankSystem.ParallelProcessing
{
    public class TaxCalculator
    {
        private const decimal TAX_RATE = 0.0015m;

        public decimal Calculate(Transaction tx)
        {
            // Se aplica impuesto a todas las transacciones por canal
            if (tx.Channel == TransactionChannel.ViaElectronica)
            {
                return tx.Amount * TAX_RATE;
            }

            return 0;
        }
    }
}
