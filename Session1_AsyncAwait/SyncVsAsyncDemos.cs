// SyncVsAsyncDemos.cs: Muestra la diferencia clave.

public static class SyncVsAsyncDemos
{
    private static readonly HttpClient _httpClient = new();

    public static async Task EjecutarDemo()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO SÍNCRONO (BLOQUEANTE) ---");
        EjecutarDescargaSincrona();

        Console.WriteLine("\n\n--- DEMO ASÍNCRONO (NO BLOQUEANTE) ---");
        await EjecutarDescargaAsincronaAsync();
    }

    private static void EjecutarDescargaSincrona()
    {
        Console.WriteLine("Iniciando descarga síncrona. La aplicación parecerá 'congelada'.");
        Console.WriteLine("No verás los puntos de actividad hasta que la descarga termine.");

        var actividadTask = MostrarActividadMientrasEspera();

        // Esta llamada bloquea el hilo principal.
        var resultado = DescargarSitioSincrono();

        Console.WriteLine($"\nDescarga síncrona completada. Tamaño: {resultado.Length} caracteres.");
        actividadTask.Wait();
    }

    private static async Task EjecutarDescargaAsincronaAsync()
    {
        Console.WriteLine("Iniciando descarga asíncrona. La aplicación seguirá 'viva'.");
        Console.WriteLine("Verás los puntos de actividad MIENTRAS se realiza la descarga.");

        var actividadTask = MostrarActividadMientrasEspera();

        // Esta llamada NO bloquea el hilo principal. Lo libera.
        var resultado = await DescargarSitioAsincronoAsync();

        Console.WriteLine($"\nDescarga asíncrona completada. Tamaño: {resultado.Length} caracteres.");
        await actividadTask;
    }

    private static string DescargarSitioSincrono()
    {
        // ¡ANTIPATRÓN! Usamos .Result para forzar el bloqueo y simular código síncrono.
        return _httpClient.GetStringAsync("https://www.microsoft.com").Result;
    }

    private static async Task<string> DescargarSitioAsincronoAsync()
    {
        // FORMA CORRECTA: Usamos await para esperar sin bloquear.
        return await _httpClient.GetStringAsync("https://www.microsoft.com");
    }

    private static async Task MostrarActividadMientrasEspera()
    {
        for (int i = 0; i < 15; i++)
        {
            Console.Write(".");
            await Task.Delay(200);
        }
    }
}
