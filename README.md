# Curso: Patrones Avanzados de Asincronía y Resiliencia en .NET

¡Bienvenido al código de ejemplo del curso! Este repositorio contiene todos los proyectos de demostración utilizados en cada sesión.

## Sesión 1: Fundamentos de `async/await`

El código de esta sesión se encuentra en la carpeta `Session1_AsyncAwait`.

### ¿Cómo ejecutarlo?

1.  Asegúrate de tener el **SDK de .NET 8** o superior instalado.
2.  Abre una terminal o consola.
3.  Navega a la carpeta `Session1_AsyncAwait`.
    ```bash
    cd Session1_AsyncAwait
    ```
4.  Ejecuta la aplicación de consola.
    ```bash
    dotnet run
    ```
5.  Aparecerá un menú interactivo. Simplemente introduce el número de la demo que quieres ver y presiona Enter.

### Contenido del Proyecto

*   **`Program.cs`**: Contiene el menú principal para seleccionar qué demostración ejecutar.
*   **`SyncVsAsyncDemos.cs`**:
    *   Muestra la diferencia fundamental entre una llamada síncrona (que bloquea la aplicación) y una llamada asíncrona (que mantiene la aplicación responsiva).
*   **`Antipatterns.cs`**:
    *   Demuestra el peligro de `async void` y cómo las excepciones pueden hacer que la aplicación se cierre inesperadamente.
    *   Demuestra cómo el uso de `.Result` en código asíncrono puede llevar a un **deadlock**, congelando la aplicación.