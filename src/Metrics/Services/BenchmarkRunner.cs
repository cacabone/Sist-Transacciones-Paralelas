using System;
using System.Collections.Generic;
using System.Diagnostics;
using BankSystem.Core.Interfaces;
using BankSystem.Core.Models;
using BankSystem.Metrics.Models;

namespace BankSystem.Metrics.Services
{
    public class BenchmarkRunner
    {
        private readonly ITransactionProcessor _processor;

        public BenchmarkRunner(ITransactionProcessor processor)
        {
            _processor = processor;
        }

        public List<BenchmarkResult> RunBenchmarks(List<Transaction> transactions)
        {
            if (transactions == null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }

            var results = new List<BenchmarkResult>();
            int volume = transactions.Count;

            Console.WriteLine($"\n[+] Iniciando Benchmark con {volume} transacciones...");

            // Baseline: Sequential execution with 1 thread
            Console.WriteLine("[midiendo con 1 hilo]");
            Stopwatch swSec = Stopwatch.StartNew();
            _processor.ProcessTransactions(transactions, 1);
            swSec.Stop();

            double secTime = swSec.Elapsed.TotalMilliseconds;

            results.Add(new BenchmarkResult
            {
                ThreadCount = 1,
                TransactionVolume = volume,
                ExecutionTime =  secTime,
                Speedup = 1.0,
                Efficiency = 1.0,
                Collisions = 0
            });

            // Parallel execution with multiple thread configurations
            int[] threadConfigs = { 2, 4, 8 };
            foreach (int threads in threadConfigs)
            {
                Console.WriteLine($"[midiendo con {threads} hilos]");
                Stopwatch swPar = Stopwatch.StartNew();
                _processor.ProcessTransactions(transactions, threads);
                swPar.Stop();

                double parTime = swPar.Elapsed.TotalMilliseconds;

                double sp =  secTime / parTime;
                double ef = sp / threads;
                
                results.Add(new BenchmarkResult
                {
                    ThreadCount = threads,
                    TransactionVolume = volume,
                    ExecutionTime =  parTime,
                    Speedup = sp,
                    Efficiency = ef,
                    Collisions = 0
                });
            }

            return results;
        }
    }
}