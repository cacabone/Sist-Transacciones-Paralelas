using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BankSystem.Metrics.Models;

namespace BankSystem.Metrics.Services
{
    public class CsvExport
    {
    public void Export(List<BenchmarkResult> results, string filename = $"benchmark_results{DateTime.Now:yyMMdd}.csv")
        {
            string rootpath =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..", "..", "results");
            Directory.createDirectory(rootpath);
            string fullpath = Path.Combine(rootpath, filename);

            var sb = new StringBuilder();
            sb.AppendLine("Hilos, Volumen, Tiempo (ms), Speedup, Eficiencia, Colisiones");
            foreach (var r in results)
            {
                string format = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:F1},{3:F2}. {4:F2},{5}"
                , r.ThreadCount, r.TransactionVolume, r.ExecutionTime, r.Speedup, r.Efficiency, r.Collisions );
                sb.AppendLine(format);

            }

            File.WriteAllText(fullpath, sb.ToString());
            Console.WriteLine($"/n[+] archivo csv creado y exportado correctamente");
        }
    
       }
    
}