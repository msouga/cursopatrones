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
    *   Muestra la diferencia fundamental entre una llamada síncrona (que bloquea la aplicación) y una llamada asíncrona (que mantiene la aplicación responsiva) de forma interactiva.

*   **`Antipatterns.cs`**:
    *   **Demo `async void`**: Demuestra el peligro de `async void` y cómo las excepciones lanzadas en su interior pueden hacer que la aplicación se cierre inesperadamente sin ser capturadas por un `try-catch`.
    *   **Demo Deadlock (`.Result`)**:
    
        *   Una aplicación de consola estándar no sufre este tipo de deadlock porque carece de un `SynchronizationContext` que fuerce al código a volver a un hilo específico.
        *   Para demostrar el problema de forma realista, esta demo **simula un entorno de UI** creando un contexto de un solo hilo.
        *   El objetivo es que el código se **congele intencionadamente**. Si la aplicación se pausa por 5 segundos y luego informa **"¡ÉXITO!"**, la demo ha funcionado correctamente al probar el deadlock.
