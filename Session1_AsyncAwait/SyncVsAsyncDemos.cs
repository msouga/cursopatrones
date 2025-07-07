// SyncVsAsyncDemos.cs: Muestra la diferencia clave.

public static class SyncVsAsyncDemos
{
    private static readonly HttpClient _httpClient = new();
    // Archivo que descargaremos y guardaremos localmente.
    private const string Url = "https://rubin.canto.com/direct/image/n4kvj0cemd5pbdqgtjdgp2jg2t/2rlAe6gN9INGmu5lzQeECpD8fy0/original?content-type=image%2Ftiff&name=lm4-Trifid-10k.tif";
    private const string FileName = "lm4-Trifid-10k.tif";

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
        var rutaArchivo = DescargarArchivoSincrono();

        var info = new FileInfo(rutaArchivo);
        Console.WriteLine($"\nDescarga síncrona completada. Archivo guardado en {info.FullName} ({info.Length} bytes).");
        actividadTask.Wait();
    }

    private static async Task EjecutarDescargaAsincronaAsync()
    {
        Console.WriteLine("Iniciando descarga asíncrona. La aplicación seguirá 'viva'.");
        Console.WriteLine("Verás los puntos de actividad MIENTRAS se realiza la descarga.");

        var actividadTask = MostrarActividadMientrasEspera();

        // Esta llamada NO bloquea el hilo principal. Lo libera.
        var rutaArchivo = await DescargarArchivoAsincronoAsync();

        var info = new FileInfo(rutaArchivo);
        Console.WriteLine($"\nDescarga asíncrona completada. Archivo guardado en {info.FullName} ({info.Length} bytes).");
        await actividadTask;
    }

    private static string DescargarArchivoSincrono()
    {
        // ¡ANTIPATRÓN! Usamos .Result para forzar el bloqueo y simular código síncrono.
        var bytes = _httpClient.GetByteArrayAsync(Url).Result;
        File.WriteAllBytes(FileName, bytes);
        return Path.GetFullPath(FileName);
    }

    private static async Task<string> DescargarArchivoAsincronoAsync()
    {
        // FORMA CORRECTA: Usamos await para esperar sin bloquear.
        var bytes = await _httpClient.GetByteArrayAsync(Url);
        await File.WriteAllBytesAsync(FileName, bytes);
        return Path.GetFullPath(FileName);
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
