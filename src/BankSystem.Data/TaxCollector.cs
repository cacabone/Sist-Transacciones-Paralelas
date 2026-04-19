using System.Threading;

namespace BankSystem.DataLayer
{
    /// <summary>
    /// Variable global de recaudación fiscal.
    /// Esta clase es el punto más crítico de sincronización del sistema:
    /// TODOS los hilos de P3 suman aquí sus impuestos simultáneamente.
    ///
    /// Usa Interlocked para operaciones atómicas sin lock explícito,
    /// lo que es más eficiente que un lock tradicional para simples sumas.
    /// </summary>
    public class TaxCollector
    {
        // Se almacena como long para usar Interlocked (no soporta decimal directamente).
        // Convertimos decimal → long multiplicando por 10000 (4 decimales de precisión).
        // Ej: $1.2345 se guarda como 12345.
        private long _totalCollectedRaw;

        private const long ScaleFactor = 10_000;

        /// <summary>
        /// Total acumulado de impuestos, convertido de vuelta a decimal.
        /// </summary>
        public decimal TotalCollected
            => Interlocked.Read(ref _totalCollectedRaw) / (decimal)ScaleFactor;

        /// <summary>
        /// Suma un monto de impuesto de forma ATÓMICA.
        /// Interlocked.Add garantiza que la suma es indivisible —
        /// no puede haber dos hilos sumando a la vez y perder uno.
        ///
        /// Alternativa más simple con lock:
        ///   lock(_lockObj) { _total += taxAmount; }
        /// pero Interlocked es más rápido bajo alta concurrencia.
        /// </summary>
        public void Add(decimal taxAmount)
        {
            // Convertir a long para la operación atómica
            long rawAmount = (long)(taxAmount * ScaleFactor);
            Interlocked.Add(ref _totalCollectedRaw, rawAmount);
        }

        /// <summary>
        /// Reinicia el contador (útil entre benchmarks de P4).
        /// </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref _totalCollectedRaw, 0);
        }

        /// <summary>
        /// Número de veces que se registró un impuesto (para métricas de P4).
        /// </summary>
        private long _taxEventCount;

        public long TaxEventCount => Interlocked.Read(ref _taxEventCount);

        public void RecordTaxEvent()
        {
            Interlocked.Increment(ref _taxEventCount);
        }
    }
}