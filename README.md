# Curso de Patrones Avanzados en .NET

Este repositorio contiene el código fuente y los materiales para el curso de patrones de desarrollo avanzados en .NET.

## Estructura del Proyecto

El repositorio está organizado en carpetas, una por cada sesión del curso:

-   **Session1_AsyncAwait/**: Demos sobre asincronía, `async/await`, bloqueos y deadlocks.
-   **Session2_CircuitBreaker/**: Implementación del patrón Circuit Breaker con Polly.
-   **Session3_SagaQueues/**: Implementación del patrón Saga con Orquestación usando Colas de Azure Service Bus.


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

---

## Sesión 2: Resiliencia con el Patrón Circuit Breaker

El código de esta sesión se encuentra en la carpeta `Session2_CircuitBreaker`.

### Dependencias

Este proyecto requiere la librería **Polly**. El archivo `.csproj` ya está configurado para descargarla automáticamente.

### ¿Cómo ejecutarlo?

1.  Navega a la carpeta `Session2_CircuitBreaker`.
    ```bash
    cd ../Session2_CircuitBreaker 
    ```
    *(Nota: `../` si estás en la carpeta de la Sesión 1)*
2.  Ejecuta la aplicación de consola.
    ```bash
    dotnet run
    ```
3.  La aplicación iniciará un bucle infinito que llama a un servicio simulado. Observa la consola para ver cómo la política de Circuit Breaker reacciona a los fallos, abre el circuito y eventualmente lo cierra de nuevo. Para detener la ejecución, presiona `Ctrl + C`.


## Sesión 3: Patrón Saga con Orquestación y Colas

Esta sesión demuestra cómo gestionar una transacción distribuida utilizando un orquestador central y colas de mensajes. El código de esta sesión se encuentra en la carpeta `Session3_SagaQueues`.

### Componentes

-   **CommandSender**: Una aplicación de consola que actúa como **Orquestador**. Envía comandos para iniciar el procesamiento de una orden.
-   **PaymentServiceReceiver**: Una aplicación de consola que actúa como **Worker** o servicio de procesamiento. Escucha los comandos de la cola, ejecuta los pasos de la saga (validar, reservar stock, procesar pago) y maneja las transacciones de compensación en caso de fallo.

### Cómo Ejecutar la Demo

Para ejecutar la demo de la Sesión 3, sigue las instrucciones detalladas en el **Manual del Alumno** a continuación. El manual te guiará a través de la configuración de los recursos necesarios en Azure y la ejecución de las aplicaciones.

---