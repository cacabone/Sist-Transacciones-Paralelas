using System;

namespace BankSystem.Core.Models
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SourceAccountId { get; set; }
        public string DestinationAccountId { get; set; } // null si no es transferencia
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionChannel Channel { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool TaxApplied { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
