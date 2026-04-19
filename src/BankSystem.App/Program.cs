using System;
using System.Diagnostics;
using System.Linq;
using BankSystem.DataLayer;
using BankSystem.ParallelProcessing;
using BankSystem.Metrics.Services;

namespace BankSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("================================================================");
            Console.WriteLine("   Sistema de Procesamiento Bancario Paralelo (PROYECTO FINAL)");
            Console.WriteLine("================================================================\n");

            // Valores por defecto
            int accountCount = 1000;
            int transactionCount = 10_000;

            // Argumentos opcionales: dotnet run <transacciones> <cuentas>
            if (args.Length > 0 && int.TryParse(args[0], out int txCount))
                transactionCount = txCount;

            if (args.Length > 1 && int.TryParse(args[1], out int accCount))
                accountCount = accCount;

            // Inicializar componentes
            var taxCollector = new TaxCollector();
            var repository = new AccountRepository(taxCollector);
            var processor = new ParallelTransactionProcessor(repository);
            var generator = new DataGenerator();
            var benchmarkRunner = new BenchmarkRunner(processor);
            var csvExport = new CsvExport();

            try
            {
                Console.WriteLine($"[*] Configuracion:");
                Console.WriteLine($"    - Cuentas:       {accountCount:N0}");
                Console.WriteLine($"    - Transacciones: {transactionCount:N0}");
                Console.WriteLine($"    - Tiempo estimado: ~{EstimateTime(transactionCount)}\n");

                // Generar cuentas
                Console.WriteLine($"[*] Generando {accountCount:N0} cuentas bancarias...");
                var accounts = generator.GenerateAccounts(accountCount);

                Console.WriteLine($"[*] Registrando cuentas en el repositorio...");
                foreach (var account in accounts)
                    repository.AddAccount(account);

                // Generar transacciones
                Console.WriteLine($"[*] Generando {transactionCount:N0} transacciones...");
                var swGen = Stopwatch.StartNew();
                var transactions = generator.GenerateTransactions(transactionCount, accounts);
                swGen.Stop();
                Console.WriteLine($"    Generacion completada en {swGen.Elapsed.TotalSeconds:F2}s");

                // Estado inicial
                Console.WriteLine($"\n[+] Estado Inicial:");
                Console.WriteLine($"    - Cuentas:              {accountCount:N0}");
                Console.WriteLine($"    - Transacciones:        {transactionCount:N0}");
                Console.WriteLine($"    - Balance Total Inicial: ${accounts.Sum(a => a.Balance):N2}");
                Console.WriteLine($"    - Impuestos Recaudados:  ${repository.GetTotalTaxCollected():N2}\n");

                // Ejecutar benchmarks
                Console.WriteLine("[*] Iniciando benchmarks de rendimiento...\n");
                var swTotal = Stopwatch.StartNew();
                var results = benchmarkRunner.RunBenchmarks(transactions.ToList());
                swTotal.Stop();

                // Mostrar tabla de resultados
                Console.WriteLine("\n================================================================");
                Console.WriteLine("   RESULTADOS DEL BENCHMARK");
                Console.WriteLine("================================================================\n");
                Console.WriteLine($"  {"Hilos",-8} {"Tiempo (ms)",-14} {"Speedup",-12} {"Eficiencia",-12} {"Volumen",-10}");
                Console.WriteLine($"  {new string('-', 58)}");
                foreach (var r in results)
                {
                    Console.WriteLine($"  {r.ThreadCount,-8} {r.ExecutionTime,-14:F2} {r.Speedup,-12:F2} {r.Efficiency,-12:F2} {r.TransactionVolume,-10:N0}");
                }

                // Estado final
                Console.WriteLine($"\n[+] Estado Final:");
                Console.WriteLine($"    - Balance Total Final:        ${accounts.Sum(a => a.Balance):N2}");
                Console.WriteLine($"    - Total Impuestos Recaudados: ${repository.GetTotalTaxCollected():N2}");
                Console.WriteLine($"    - Transacciones Exitosas:     {processor.SuccessfulTransactions:N0}");
                Console.WriteLine($"    - Transacciones Fallidas:     {processor.FailedTransactions:N0}");
                Console.WriteLine($"    - Tiempo Total Benchmark:     {swTotal.Elapsed.TotalSeconds:F2}s\n");

                // Exportar CSV
                Console.WriteLine("[*] Exportando resultados a CSV...");
                csvExport.Export(results);

                Console.WriteLine("\n[OK] Procesamiento completado exitosamente!");
                Console.WriteLine($"\n[INFO] Para cambiar volumen: dotnet run <transacciones> <cuentas>");
                Console.WriteLine($"       Ejemplo: dotnet run 1000000 5000");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"        Causa: {ex.InnerException.Message}");
                Console.WriteLine($"        Detalle: {ex.StackTrace}");
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static string EstimateTime(int transactionCount)
        {
            double estimatedMs = (transactionCount / 1000.0) * 1.5;
            return estimatedMs < 1000
                ? $"{estimatedMs / 1000:F1}s"
                : $"{estimatedMs / 1000 / 60:F1} min";
        }
    }
}