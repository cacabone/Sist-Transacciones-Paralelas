# Sistema de Procesamiento Bancario Paralelo

Proyecto Final — Programacion Paralela | Abril 2026

## Descripcion General

Sistema que procesa transacciones bancarias masivas usando **descomposicion de datos** para demostrar la eficiencia del paralelismo en C#. Procesa depositos, retiros y transferencias provenientes de tres canales distintos, aplicando logica fiscal del 0.15% segun el canal y tipo de operacion.

## Caracteristicas

- Procesamiento paralelo con 1, 2, 4 y 8 hilos usando Task Parallel Library (TPL)
- Tres canales bancarios: Cajero, Agente Bancario y Via Electronica
- Logica fiscal del 0.15% por canal y tipo de transaccion
- Sincronizacion thread-safe con locks granulares por cuenta e Interlocked atomico
- Generacion reproducible de datos con semilla fija (Seed = 42)
- Benchmarking automatico con exportacion a CSV
- Soporte hasta 1,000,000 de transacciones

## Estructura del Repositorio

```
Sist-Transacciones-Paralelas2/
├── src/
│   ├── Core/                    # Modelos, interfaces y enums base
│   ├── BankSystem.Data/         # Repositorio de cuentas y generador de datos
│   ├── ParallelProcessing/      # Motor de procesamiento paralelo
│   ├── Processing/              # Coordinador de procesamiento
│   ├── Metrics/                 # Benchmarking y exportacion CSV
│   └── BankSystem.App/          # Punto de entrada principal
├── docs/                        # Documentacion del proyecto
├── metrics/                     # Resultados CSV de benchmarks
├── tests/                       # Pruebas y generacion de carga
└── README.md
```

## Inicio Rapido

```powershell
# Clonar el repositorio
git clone https://github.com/[usuario]/Sist-Transacciones-Paralelas2
cd Sist-Transacciones-Paralelas2

# Compilar
dotnet build

# Ejecutar con valores por defecto (1,000 cuentas / 10,000 transacciones)
dotnet run --project src/BankSystem.App/BankSystem.App.csproj

# Ejecutar con parametros personalizados
dotnet run --project src/BankSystem.App/BankSystem.App.csproj <transacciones> <cuentas>
```

Los resultados CSV se exportan automaticamente a `/metrics/results/`.

## Equipo

| Integrante | Rol | Modulo |
|---|---|---|
| Robert Gabriel Nunez Matias | Lider | Core + App |
| Emil Remy Lopez Hernandez | Desarrollador | Data Layer |
| Jorge Luis Reynoso Frias | Desarrollador | Parallel Processing |
| Jimmy Alexander Herrera | Desarrollador | Metrics |

## Reglas de Trabajo

1. No trabajar directamente en `main`. Usar rama asignada (`feature/...`).
2. Todo PR debe ser revisado por el lider antes del merge.
3. Cada commit debe ser significativo e identificar al autor.
4. Todo codigo debe permitir medir el tiempo de ejecucion.

## Profesor

Erick Leonardo Perez Veloz — Programacion Paralela
