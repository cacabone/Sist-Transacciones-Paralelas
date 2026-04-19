using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using BankSystem.Core.Models;
using BankSystem.Core;

namespace BankSystem.DataLayer
{
    /// <summary>
    /// Generador de datos de prueba optimizado para volúmenes masivos.
    /// Soporta hasta 1 millón de transacciones con generación paralela.
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
        /// Genera N cuentas bancarias con balance inicial aleatorio.
        /// Optimizado con generación paralela para grandes volúmenes.
      /// </summary>
   public List<Account> GenerateAccounts(int count)
{
       var accounts = new List<Account>(count);
 
            if (count > 10000)
     {
   // Paralelo para grandes volúmenes
       var accountsArray = new Account[count];
       object lockObj = new object();
 int counter = 0;

  Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
        {
          var localRng = new Random(Seed + i); // RNG local por thread
         var account = new Account
      {
   Id = $"ACC-{i + 1:D6}",
      Owner = Names[localRng.Next(Names.Length)],
         Balance = Math.Round((decimal)(localRng.NextDouble() * 49_500 + 500), 2)
        };
     accountsArray[i] = account;
         });

       accounts.AddRange(accountsArray);
}
       else
            {
     // Secuencial para pequeños volúmenes
        for (int i = 1; i <= count; i++)
           {
     accounts.Add(new Account
     {
        Id = $"ACC-{i:D4}",
            Owner = Names[_rng.Next(Names.Length)],
 Balance = Math.Round((decimal)(_rng.NextDouble() * 49_500 + 500), 2)
   });
                }
            }

            return accounts;
        }

        /// <summary>
        /// Genera N transacciones aleatorias distribuidas entre los 3 canales.
        /// Optimizado con generación paralela para volúmenes masivos (hasta 1M).
        /// 70% depósitos, 20% transferencias, 10% retiros para minimizar errores de fondos.
        /// </summary>
        public List<Transaction> GenerateTransactions(int count, List<Account> accounts)
        {
            if (accounts == null || accounts.Count < 2)
       throw new ArgumentException("Se necesitan al menos 2 cuentas para generar transacciones.");

 var transactions = new List<Transaction>(count);
  var channels = (TransactionChannel[])Enum.GetValues(typeof(TransactionChannel));
     
    if (count > 100000)
 {
          // Generación paralela para volúmenes masivos
    var transactionsArray = new Transaction[count];

  Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
                {
      var localRng = new Random(Seed + i); // RNG local por thread
        
        // 70% depósitos, 20% transferencias, 10% retiros
             double typeRandom = localRng.NextDouble();
   var type = typeRandom < 0.7 ? TransactionType.Deposito :
      typeRandom < 0.9 ? TransactionType.Transferencia : TransactionType.Retiro;

      var channel = channels[localRng.Next(channels.Length)];
      var amount = GenerateAmount(type, localRng);

       string sourceId = accounts[localRng.Next(accounts.Count)].Id;
                    string? destinationId = null;

                    if (type == TransactionType.Transferencia)
         {
    do
         {
                 destinationId = accounts[localRng.Next(accounts.Count)].Id;
       } while (destinationId == sourceId);
    }

           transactionsArray[i] = new Transaction
        {
       SourceAccountId = sourceId,
 DestinationAccountId = destinationId,
   Amount = amount,
             Type = type,
        Channel = channel,
            };
});

     transactions.AddRange(transactionsArray);
      }
   else
          {
              // Generación secuencial para pequeños volúmenes
              for (int i = 0; i < count; i++)
                {
             double typeRandom = _rng.NextDouble();
         var type = typeRandom < 0.7 ? TransactionType.Deposito :
   typeRandom < 0.9 ? TransactionType.Transferencia : TransactionType.Retiro;

        var channel = channels[_rng.Next(channels.Length)];
           var amount = GenerateAmount(type, _rng);

                    string sourceId = accounts[_rng.Next(accounts.Count)].Id;
                    string? destinationId = null;

                    if (type == TransactionType.Transferencia)
           {
             do
          {
         destinationId = accounts[_rng.Next(accounts.Count)].Id;
           } while (destinationId == sourceId);
  }

    transactions.Add(new Transaction
    {
     SourceAccountId = sourceId,
            DestinationAccountId = destinationId,
   Amount = amount,
             Type = type,
              Channel = channel,
           });
       }
  }

    return transactions;
        }

  /// <summary>
        /// Genera montos realistas según el tipo de operación.
  /// </summary>
   private decimal GenerateAmount(TransactionType type, Random rng)
     {
            double raw = type switch
       {
     TransactionType.Deposito => rng.NextDouble() * 490 + 10,       // $10 – $500
     TransactionType.Retiro => rng.NextDouble() * 190 + 10,           // $10 – $200
       TransactionType.Transferencia => rng.NextDouble() * 490 + 50,   // $50 – $540
     _ => rng.NextDouble() * 100 + 1
 };

            return Math.Round((decimal)raw, 2);
        }

        /// <summary>
        /// Método de conveniencia: genera cuentas Y transacciones en un solo llamado.
     /// </summary>
        public (List<Account> Accounts, List<Transaction> Transactions) GenerateScenario(
            int accountCount,
    int transactionCount)
   {
  var accounts = GenerateAccounts(accountCount);
       var transactions = GenerateTransactions(transactionCount, accounts);
  return (accounts, transactions);
        }
    }
}