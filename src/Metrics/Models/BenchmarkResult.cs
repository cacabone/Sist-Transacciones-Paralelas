using System;


namespace BankSystem.Metrics.Models
{
    public class BenchmarkResult
    {
        public int ThreadCount { get; set;}
        public int TransactionVolume { get; set;}
        public TimeSpan ExecutionTime { get; set;}
        public double Speedup { get; set;}
        public double Efficiency { get; set;}
        public int Collisions { get; set;}
    }
}