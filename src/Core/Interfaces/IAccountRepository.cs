using BankSystem.Core.Models;

namespace BankSystem.Core.Interfaces
{
    public interface IAccountRepository
    {
        Account GetAccount(string accountId);
        void UpdateBalance(string accountId, decimal delta);
        decimal GetTotalTaxCollected();
        void AddTax(decimal taxAmount);
    }
}
