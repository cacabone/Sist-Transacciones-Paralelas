using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Interfaces
{
    /// <summary>
    /// Contrato del repositorio de cuentas.
    /// P3 y P4 trabajan contra esta interfaz — cualquier método
    /// que necesiten debe estar declarado aquí.
    /// </summary>
    public interface IAccountRepository
    {
        // --- Gestión de cuentas ---

        /// <summary>
        /// Registra una cuenta nueva en el repositorio.
        /// Llamado por DataGenerator durante la inicialización del sistema.
        /// </summary>
        void AddAccount(Account account);

        /// <summary>
        /// Obtiene una cuenta por su ID.
        /// </summary>
        Account GetAccount(string accountId);

        /// <summary>
        /// Retorna todas las cuentas (útil para reportes de P4).
        /// </summary>
        IEnumerable<Account> GetAllAccounts();

        // --- Operaciones de balance ---

        /// <summary>
        /// Actualiza el balance con un delta (+ crédito, - débito).
        /// La implementación debe ser thread-safe.
        /// </summary>
        void UpdateBalance(string accountId, decimal delta);

        // --- Fiscalidad ---

        /// <summary>
        /// Retorna el total acumulado de impuestos recaudados.
        /// </summary>
        decimal GetTotalTaxCollected();

        /// <summary>
        /// Agrega un monto a la recaudación fiscal global.
        /// La implementación debe ser thread-safe.
        /// </summary>
        void AddTax(decimal taxAmount);
    }
}