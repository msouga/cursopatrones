// Antipatterns.cs: (VERSIÓN FINAL CON CORRECCIÓN DE COMPILACIÓN)
// Se ha corregido el argumento faltante en la llamada a Post.

using System.Collections.Concurrent;

public static class Antipatterns
{
    // --- Demo `async void` (correcta, sin cambios) ---
    public static async Task DemostrarAsyncVoid()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO `async void` ---");
        Console.WriteLine("Llamando a un método `async void` que lanza una excepción...");
        Console.ReadLine();
        try
        {
            LanzarExcepcionAsyncVoid();
        }
        catch (Exception) { /* No funciona */ }
        await Task.Delay(2000);
    }

    private static async void LanzarExcepcionAsyncVoid()
    {
        await Task.Delay(500);
        throw new InvalidOperationException("¡Boom! Esta excepción no será capturada y cerrará la aplicación.");
    }

    // --- DEMO DEADLOCK (REESCRITA CON CONTEXTO REAL) ---
    public static void DemostrarDeadlock()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO DEADLOCK (SIMULADO) ---");
        Console.WriteLine("Vamos a ejecutar código en un contexto de un solo hilo, como una UI.");
        Console.WriteLine("Si el código no responde en 5 segundos, el deadlock se habrá demostrado.");
        Console.WriteLine("Presiona Enter para empezar...");
        Console.ReadLine();

        // Creamos nuestro contexto de un solo hilo.
        using var contexto = new SingleThreadSynchronizationContext();
        
        var tcs = new TaskCompletionSource<bool>();

        // Encolamos la acción que causa el deadlock.
        contexto.Post(_ =>
        {
            try
            {
                Console.WriteLine("   [Contexto de Hilo Único] - Hilo principal se va a bloquear con .Result...");
                
                var resultado = ObtenerDatosAsync().Result;
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ¡FALLO! Esto nunca debería imprimirse. Resultado: {resultado}");
                Console.ResetColor();
                tcs.SetResult(false);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }, null); 
        
        try
        {
            tcs.Task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nFALLO DE LA DEMO: El código terminó, lo cual no debería haber pasado.");
            Console.ResetColor();
        }
        catch (TimeoutException)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n¡ÉXITO! El código no respondió en 5 segundos, demostrando el DEADLOCK.");
            Console.ResetColor();
        }
    }

    private static async Task<string> ObtenerDatosAsync()
    {
        await Task.Delay(100);
        return "Datos de ejemplo";
    }
}

/// <summary>
/// Un SynchronizationContext real que ejecuta todo el trabajo en un único hilo
/// a través de una cola, simulando un bucle de mensajes de UI.
/// </summary>
public sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
{
    private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> _queue = new();
    private readonly Thread _thread;

    public SingleThreadSynchronizationContext()
    {
        _thread = new Thread(RunOnCurrentThread)
        {
            IsBackground = true
        };
        _thread.Start();
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
    }

    private void RunOnCurrentThread()
    {
        SetSynchronizationContext(this);

        foreach (var workItem in _queue.GetConsumingEnumerable())
        {
            workItem.Key(workItem.Value);
        }
    }
    
    public void Dispose()
    {
        _queue.CompleteAdding();
    }
}