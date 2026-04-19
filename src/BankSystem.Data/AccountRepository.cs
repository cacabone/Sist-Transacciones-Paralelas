using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BankSystem.Core.Interfaces;
using BankSystem.Core.Models;

namespace BankSystem.DataLayer
{
    /// <summary>
    /// Repositorio central de cuentas bancarias.
    /// Usa ConcurrentDictionary para acceso thread-safe a nivel de colección,
    /// y locks individuales por cuenta para proteger operaciones de balance.
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly ConcurrentDictionary<string, Account> _accounts;

        // GetOrAdd garantiza que el lock siempre existe aunque UpdateBalance
        // se llame antes de AddAccount (escenario defensivo).
        private readonly ConcurrentDictionary<string, object> _accountLocks;

        private readonly TaxCollector _taxCollector;

        public AccountRepository(TaxCollector taxCollector)
        {
            _accounts     = new ConcurrentDictionary<string, Account>();
            _accountLocks = new ConcurrentDictionary<string, object>();
            _taxCollector = taxCollector;
        }

        /// <summary>
        /// Registra una cuenta en el repositorio.
        /// Llamado por DataGenerator al inicializar el sistema.
        /// </summary>
        public void AddAccount(Account account)
        {
            _accounts[account.Id]     = account;
            _accountLocks[account.Id] = new object();
        }

        /// <summary>
        /// Obtiene una cuenta por su ID.
        /// </summary>
        public Account GetAccount(string accountId)
        {
            if (!_accounts.TryGetValue(accountId, out var account))
                throw new InvalidOperationException($"Cuenta '{accountId}' no encontrada.");

            return account;
        }

        /// <summary>
        /// Retorna todas las cuentas (para reportes de P4).
        /// </summary>
        public IEnumerable<Account> GetAllAccounts() => _accounts.Values;

        /// <summary>
        /// Actualiza el balance de una cuenta con un delta.
        /// SINCRONIZADO con lock granular por cuenta.
        /// VALIDA que el balance no quede negativo (fondos insuficientes).
        /// </summary>
        public void UpdateBalance(string accountId, decimal delta)
        {
            if (!_accounts.TryGetValue(accountId, out var account))
                throw new InvalidOperationException($"Cuenta '{accountId}' no encontrada.");

            // GetOrAdd es defensivo: crea el lock si por alguna razón no existe,
            // evitando un KeyNotFoundException en escenarios inesperados.
            var accountLock = _accountLocks.GetOrAdd(accountId, _ => new object());

            lock (accountLock)
            {
                // Validación de fondos insuficientes (solo aplica a débitos).
                if (account.Balance + delta < 0)
                    throw new InvalidOperationException(
                        $"Fondos insuficientes en cuenta '{accountId}'. " +
                        $"Balance: {account.Balance:C}, Débito solicitado: {Math.Abs(delta):C}");

                account.Balance += delta;
            }
        }

        /// <summary>
        /// Retorna el total acumulado de impuestos recaudados.
        /// </summary>
        public decimal GetTotalTaxCollected() => _taxCollector.TotalCollected;

        /// <summary>
        /// Agrega un monto a la recaudación fiscal y registra el evento.
        /// </summary>
        public void AddTax(decimal taxAmount)
        {
            _taxCollector.Add(taxAmount);
            _taxCollector.RecordTaxEvent(); // ← corregido: ahora sí se llama
        }
    }
}