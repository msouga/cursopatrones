// Antipatterns.cs: Ejemplos de lo que NO se debe hacer.

public static class Antipatterns
{
    public static async Task DemostrarAsyncVoid()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO `async void` ---");
        Console.WriteLine("Vamos a llamar a un método `async void` que lanza una excepción.");
        Console.WriteLine("El `try-catch` que lo rodea NO podrá capturarla.");
        Console.WriteLine("La aplicación se cerrará abruptamente. Prepárate...");
        Console.ReadLine();

        try
        {
            MetodoQueLlamaAsyncVoid();
        }
        catch (Exception ex)
        {
            // Este bloque de código NUNCA se ejecutará.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Excepción capturada! Mensaje: {ex.Message}");
            Console.ResetColor();
        }
        
        await Task.Delay(2000); 
    }

    private static void MetodoQueLlamaAsyncVoid()
    {
        Console.WriteLine("Llamando al método `async void`...");
        LanzarExcepcionAsyncVoid();
        Console.WriteLine("El método `async void` ha sido llamado, pero la excepción ocurrirá fuera de nuestro control.");
    }

    private static async void LanzarExcepcionAsyncVoid()
    {
        await Task.Delay(500);
        throw new InvalidOperationException("¡Boom! Esta excepción no será capturada y cerrará la aplicación.");
    }

    public static void DemostrarDeadlock()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO DEADLOCK ---");
        Console.WriteLine("Este código demuestra el patrón que causa un deadlock en aplicaciones de UI o ASP.NET clásico.");
        Console.WriteLine("En una app de consola, puede que funcione o no, pero el patrón es IGUALMENTE PELIGROSO.");
        Console.WriteLine("Presiona Enter para intentar ejecutar el código problemático...");
        Console.ReadLine();

        try
        {
            var resultado = ObtenerDatosAsync().Result;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Sorprendentemente, funcionó en este contexto de consola!");
            Console.WriteLine($"Resultado: {resultado.Length} caracteres.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ocurrió una excepción (probablemente por el deadlock): {ex.InnerException?.Message}");
            Console.ResetColor();
        }
    }

    private static async Task<string> ObtenerDatosAsync()
    {
        var resultado = await new HttpClient().GetStringAsync("https://www.google.com");
        return resultado;
    }
}