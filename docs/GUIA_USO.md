# Guia de Uso — Sistema de Procesamiento Bancario Paralelo

## Requisitos Previos

- .NET 8.0 SDK — https://dotnet.microsoft.com/download
- Git

Verificar instalacion:
```powershell
dotnet --version
```

---

## Compilar el Proyecto

```powershell
# Desde la raiz del repositorio
dotnet build
```

---

## Ejecutar el Sistema

### Ejecucion basica (valores por defecto: 1,000 cuentas / 10,000 transacciones)
```powershell
dotnet run --project src/BankSystem.App/BankSystem.App.csproj
```

### Ejecucion con parametros personalizados
```powershell
# Formato: <transacciones> <cuentas>
dotnet run --project src/BankSystem.App/BankSystem.App.csproj <transacciones> <cuentas>
```

### Ejemplos por volumen

| Escenario | Comando |
|-----------|---------|
| Test pequeño (10K) | `dotnet run --project src/BankSystem.App/BankSystem.App.csproj 10000 100` |
| Test mediano (100K) | `dotnet run --project src/BankSystem.App/BankSystem.App.csproj 100000 500` |
| Test grande (500K) | `dotnet run --project src/BankSystem.App/BankSystem.App.csproj 500000 2000` |
| Test maximo (1M) | `dotnet run --project src/BankSystem.App/BankSystem.App.csproj --configuration Release 1000000 10000` |

> Usar `--configuration Release` en volumenes grandes para mejor rendimiento.

---

## Ver Resultados

Los archivos CSV se generan automaticamente en:

```
/metrics/results/benchmark_results_[fecha].csv
```

### Abrir desde PowerShell
```powershell
# Listar archivos generados
Get-ChildItem metrics/results/ | Sort-Object LastWriteTime -Descending

# Ver contenido del mas reciente
Get-Content (Get-ChildItem metrics/results/ | Sort-Object LastWriteTime -Descending | Select-Object -First 1).FullName
```

### Abrir en Excel
Los CSV son compatibles directamente con Excel, Google Sheets, Power BI y Python/pandas.

---

## Interpretar los Resultados

Cada fila del CSV representa una ejecucion con distinto numero de hilos:

```csv
Hilos,Volumen,Tiempo (ms),Speedup,Eficiencia,Colisiones
1,100000,30.52,1.00,1.00,0
2,100000,16.39,1.86,0.93,0
4,100000,9.94,3.07,0.77,0
8,100000,7.26,4.20,0.53,0
```

| Metrica | Formula | Interpretacion |
|---------|---------|----------------|
| Speedup | T1 / Tn | Cuantas veces mas rapido que 1 hilo |
| Eficiencia | Speedup / N_hilos | 1.0 = uso perfecto del CPU |
| Colisiones | - | Conflictos de acceso concurrente |

### Referencia rapida

- Eficiencia > 0.75: excelente escalabilidad
- Eficiencia 0.50–0.75: buena, overhead moderado
- Eficiencia < 0.50: alta contension, considerar menos hilos

---

## Logica Fiscal por Canal

| Canal | Tipo | Aplica impuesto | Condicion |
|-------|------|-----------------|-----------|
| Cajero | Retiro | Si | Monto >= $10,000 |
| Cajero | Deposito / Transferencia | No | — |
| Agente Bancario | Retiro / Transferencia | Si | Cualquier monto |
| Agente Bancario | Deposito | No | — |
| Via Electronica | Cualquier tipo | Si | Siempre |

Tasa de impuesto: **0.15%**

---

## Reproducibilidad

Los datos se generan con semilla fija (`Seed = 42`). Ejecutar el mismo comando en cualquier maquina produce exactamente los mismos datos, lo que permite comparar resultados entre equipos de forma confiable.

---

## Troubleshooting

**El Speedup es menor a 1 con pocos datos**
Normal con volumenes pequeños (< 50K). El overhead de crear hilos supera la ganancia. Aumentar el volumen de transacciones.

**Transacciones fallidas**
Son fondos insuficientes detectados correctamente. El sistema las maneja de forma silenciosa y las contabiliza en "Transacciones Fallidas".

**Lento en volumen grande**
Agregar `--configuration Release` al comando para activar optimizaciones del compilador.

**CSV no aparece en metrics/results/**
Verificar que la carpeta `/metrics/results/` existe en la raiz del repositorio. Crearla manualmente si es necesario:
```powershell
New-Item -ItemType Directory -Force -Path metrics/results
```
