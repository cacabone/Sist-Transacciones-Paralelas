using System;


namespace BankSystem.Metrics.Models
{
    public class BenchmarkResult
    {
        public int ThreadsCount { get; set;}
        public int TransactionVolume { get; set;}
        public TimeSpan ExecutionTime { get; set;}
        public double Speedup { get; set;}
        public double efficiency { get; set;}
        public int coallisions { get; set;}
    }
}