# Resumen de Resultados — Benchmark de Paralelismo

Resultados obtenidos ejecutando el sistema en configuracion Debug sobre hardware de escritorio estandar.

---

## Escenario 1: 100,000 transacciones / 2,000 cuentas

| Hilos | Tiempo (ms) | Speedup | Eficiencia |
|-------|-------------|---------|------------|
| 1     | 30.52       | 1.00x   | 100%       |
| 2     | 16.39       | 1.86x   | 93%        |
| 4     | 9.94        | 3.07x   | 77%        |
| 8     | 7.26        | 4.20x   | 53%        |

**Impuestos recaudados:** $49,504.45  
**Transacciones exitosas:** 100,000 / 100,000

---

## Escenario 2: 500,000 transacciones / 5,000 cuentas

| Hilos | Tiempo (ms) | Speedup | Eficiencia |
|-------|-------------|---------|------------|
| 1     | 98.73       | 1.00x   | 100%       |
| 2     | 89.35       | 1.11x   | 55%        |
| 4     | 70.28       | 1.40x   | 35%        |
| 8     | 47.91       | 2.06x   | 26%        |

**Impuestos recaudados:** $247,995.15  
**Transacciones exitosas:** 500,000 / 500,000

---

## Escenario 3: 1,000,000 transacciones / 10,000 cuentas

| Hilos | Tiempo (ms) | Speedup | Eficiencia |
|-------|-------------|---------|------------|
| 1     | 210.25      | 1.00x   | 100%       |
| 2     | 147.53      | 1.43x   | 71%        |
| 4     | 79.93       | 2.63x   | 66%        |
| 8     | 59.19       | 3.55x   | 44%        |

**Impuestos recaudados:** $495,967.01  
**Transacciones exitosas:** 1,000,000 / 1,000,000

---

## Analisis

**Mejor Speedup absoluto:** 4.20x con 8 hilos en 100K transacciones  
**Mejor eficiencia:** 93% con 2 hilos en 100K transacciones  
**Reduccion maxima de tiempo:** de 210ms a 59ms en 1M transacciones (-72%)

El escenario de 100K transacciones muestra la mejor relacion Speedup/Eficiencia porque el volumen es suficiente para amortizar el overhead de creacion de hilos, pero lo suficientemente pequeño para que la contension de locks sea minima.

El escenario de 500K presenta Speedup sublineal con 2 y 4 hilos debido a mayor contension en cuentas compartidas. Con 8 hilos el trabajo por hilo es suficientemente grande para diluir ese overhead.

En 1M transacciones el Speedup crece consistentemente con cada hilo adicional, confirmando que el sistema escala correctamente a volumenes de produccion.

---

## Distribucion de Transacciones Generadas

| Tipo | Porcentaje | Rango de monto |
|------|------------|----------------|
| Deposito | 70% | $10 — $500 |
| Transferencia | 20% | $50 — $540 |
| Retiro | 10% | $10 — $200 |

Generadas con semilla fija `Seed = 42` — resultados reproducibles en cualquier maquina.

---

## Archivos CSV

Los resultados detallados se encuentran en `/metrics/results/` con nombre `benchmark_results_[fecha].csv`.
