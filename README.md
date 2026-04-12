# Sistema de Procesamiento Bancario Paralelo (PROYECTO FINAL)

## Descripción General
Este sistema procesa transacciones bancarias masivas utilizando **descomposición de datos** para demostrar la eficiencia del paralelismo. El sistema integra:
* **Canales de Origen:** Cajero, Agente Bancario y Vía Electrónica.
* **Lógica Fiscal:** Retención del 0.15% según el canal y monto.
* **Sincronización:** Uso de Mutex/Locks para proteger balances y la cuenta global de impuestos.

## Reglas de Trabajo
1. **Ramas:** No trabajar en `main`. Usar la rama asignada (`feature/...`).
2. **Pull Requests:** Antes de unir código a `main`, el líder debe revisar el PR.
3. **Métricas:** Todo código debe permitir medir el tiempo de ejecución.

## Estructura de Carpetas
* `/src`: Código fuente.
* `/docs`: Propuesta y manuales.
* `/tests`: Generación de carga masiva.
* `/metrics`: Gráficas de Speedup y Eficiencia.
