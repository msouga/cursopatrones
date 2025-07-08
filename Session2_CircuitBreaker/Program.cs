using Polly;
using Polly.CircuitBreaker;

Console.Title = "Curso Patrones .NET - Sesión 2: Circuit Breaker";

var service = new SimulatedUnreliableService();

// --- Definición de la Política de Circuit Breaker ---
// Queremos que el circuito se abra si ocurren 2 fallos consecutivos.
// Una vez abierto, permanecerá así por 10 segundos.
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>() // Solo reacciona a este tipo de excepción
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 2,
        durationOfBreak: TimeSpan.FromSeconds(10),
        onBreak: (exception, timespan) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n--> CIRCUITO ABIERTO durante {timespan.TotalSeconds} segundos debido a: {exception.Message} <--");
            Console.ResetColor();
        },
        onReset: () =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--> CIRCUITO CERRADO. El servicio parece estar operativo. <--");
            Console.ResetColor();
        },
        onHalfOpen: () =>
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--> CIRCUITO SEMI-ABIERTO. Intentando una llamada de prueba... <--");
            Console.ResetColor();
        }
    );

var i = 0;
while (true)
{
    i++;
    Console.Write($"\nIntento #{i}: ");

    try
    {
        // Ejecutamos la llamada a través de la política
        await circuitBreakerPolicy.ExecuteAsync(() => service.CallAsync());
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Llamada exitosa.");
        Console.ResetColor();
    }
    catch (HttpRequestException ex)
    {
        // Esta excepción es del servicio simulado
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"FALLO DIRECTO del servicio: {ex.Message}");
        Console.ResetColor();
    }
    catch (BrokenCircuitException)
    {
        // Esta excepción es de Polly, cuando el circuito está abierto
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("FALLO RÁPIDO. El circuito está abierto. No se intentó la llamada.");
        Console.ResetColor();
    }

    await Task.Delay(2000); // Esperamos 2 segundos entre intentos
}
