using System;
using System.Collections.Generic;
using BankSystem.Core.Models;
using BankSystem.Core;

namespace BankSystem.Data
{
    /// <summary>
    /// Generador de datos de prueba con semilla fija.
    /// La semilla fija (42) garantiza que cada ejecución produce
    /// exactamente los mismos datos — fundamental para que P4 pueda
    /// comparar benchmarks de 2/4/8 hilos con condiciones idénticas.
    /// </summary>
    public class DataGenerator
    {
        // Semilla fija: siempre los mismos datos, siempre reproducible.
        private const int Seed = 42;

        private readonly Random _rng;

        // Nombres de ejemplo para los propietarios de cuentas
        private static readonly string[] Names =
        {
            "Ana Pérez", "Luis García", "María López", "Carlos Rodríguez",
            "Sofía Martínez", "Juan Hernández", "Laura Díaz", "Pedro Sánchez",
            "Valentina Torres", "Miguel Ramírez", "Isabel Flores", "Andrés Castro"
        };

        public DataGenerator()
        {
            _rng = new Random(Seed);
        }

        /// <summary>
        /// Genera N cuentas bancarias con balance inicial aleatorio entre $500 y $50,000.
        /// Los IDs son del formato "ACC-0001", "ACC-0002", etc. para fácil trazabilidad.
        /// </summary>
        public List<Account> GenerateAccounts(int count)
        {
            var accounts = new List<Account>(count);

            for (int i = 1; i <= count; i++)
            {
                accounts.Add(new Account
                {
                    Id      = $"ACC-{i:D4}",
                    Owner   = Names[_rng.Next(Names.Length)],
                    Balance = Math.Round((decimal)(_rng.NextDouble() * 49_500 + 500), 2)
                    //         Balance entre $500.00 y $50,000.00
                });
            }

            return accounts;
        }

        /// <summary>
        /// Genera N transacciones aleatorias distribuidas entre los 3 canales.
        /// Los montos siguen rangos realistas por tipo de operación:
        ///   - Depósito:      $10  – $5,000
        ///   - Retiro:        $10  – $2,000
        ///   - Transferencia: $50  – $10,000
        ///
        /// Las cuentas de origen/destino se escogen aleatoriamente del pool de IDs.
        /// </summary>
        public List<Transaction> GenerateTransactions(int count, List<Account> accounts)
        {
            if (accounts == null || accounts.Count < 2)
                throw new ArgumentException("Se necesitan al menos 2 cuentas para generar transacciones.");

            var transactions = new List<Transaction>(count);

            var channels = (TransactionChannel[])Enum.GetValues(typeof(TransactionChannel));
            var types    = (TransactionType[])Enum.GetValues(typeof(TransactionType));

            for (int i = 0; i < count; i++)
            {
                var type    = types[_rng.Next(types.Length)];
                var channel = channels[_rng.Next(channels.Length)];
                var amount  = GenerateAmount(type);

                // Origen aleatorio
                string sourceId = accounts[_rng.Next(accounts.Count)].Id;

                // Destino: solo para transferencias, y diferente al origen
                string destinationId = null;
                if (type == TransactionType.Transferencia)
                {
                    do
                    {
                        destinationId = accounts[_rng.Next(accounts.Count)].Id;
                    } while (destinationId == sourceId);
                }

                transactions.Add(new Transaction
                {
                    // Id y Timestamp se auto-asignan en el constructor del modelo (P1)
                    SourceAccountId      = sourceId,
                    DestinationAccountId = destinationId,
                    Amount               = amount,
                    Type                 = type,
                    Channel              = channel,
                    // TaxApplied y TaxAmount los calculará P3 al procesar
                });
            }

            return transactions;
        }

        /// <summary>
        /// Genera montos realistas según el tipo de operación.
        /// </summary>
        private decimal GenerateAmount(TransactionType type)
        {
            double raw = type switch
            {
                TransactionType.Deposito       => _rng.NextDouble() * 4_990 + 10,    // $10 – $5,000
                TransactionType.Retiro         => _rng.NextDouble() * 1_990 + 10,    // $10 – $2,000
                TransactionType.Transferencia  => _rng.NextDouble() * 9_950 + 50,    // $50 – $10,000
                _                              => _rng.NextDouble() * 1_000 + 1
            };

            return Math.Round((decimal)raw, 2);
        }

        /// <summary>
        /// Método de conveniencia: genera cuentas Y transacciones en un solo llamado.
        /// Útil para P4 cuando necesita preparar escenarios de benchmark rápidamente.
        /// </summary>
        public (List<Account> Accounts, List<Transaction> Transactions) GenerateScenario(
            int accountCount,
            int transactionCount)
        {
            var accounts     = GenerateAccounts(accountCount);
            var transactions = GenerateTransactions(transactionCount, accounts);
            return (accounts, transactions);
        }
    }
}