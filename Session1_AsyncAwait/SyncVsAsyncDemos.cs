// SyncVsAsyncDemos.cs: (VERSIÓN FINAL Y CORREGIDA)
// Demuestra el bloqueo de forma inequívoca.

public static class SyncVsAsyncDemos
{
    private static readonly HttpClient _httpClient = new();
    private const string Url = "https://rubin.canto.com/direct/image/n4kvj0cemd5pbdqgtjdgp2jg2t/2rlAe6gN9INGmu5lzQeECpD8fy0/original?content-type=image%2Ftiff&name=lm4-Trifid-10k.tif";
    private const string FileName = "downloaded-file.tif";

    public static async Task EjecutarDemo()
    {
        Console.Clear();
        Console.WriteLine("--- DEMO SÍNCRONO (BLOQUEANTE) ---");
        EjecutarDescargaSincrona();

        Console.WriteLine("\n\nPresiona Enter para continuar con la demo asíncrona...");
        Console.ReadLine();

        Console.Clear();
        Console.WriteLine("--- DEMO ASÍNCRONO (NO BLOQUEANTE) ---");
        await EjecutarDescargaAsincronaAsync();
    }

    private static void EjecutarDescargaSincrona()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine("La aplicación responderá durante 3 segundos. Escribe algo.");

        // Bucle de 3 segundos donde la aplicación está "viva"
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(3))
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true); // Lee la tecla pero no la muestra
                Console.Write(".");    // Muestra un punto como feedback
            }
        }

        Console.WriteLine("\n\n¡TIEMPO! Iniciando descarga síncrona AHORA.");
        Console.WriteLine("==> LA APLICACIÓN SE CONGELARÁ. Intenta escribir, no pasará nada. <==");

        // --- EL BLOQUEO REAL ---
        // Este es el momento en que el único hilo se bloquea.
        // No hay otros hilos trabajando. El programa está muerto hasta que esto termine.
        var bytes = _httpClient.GetByteArrayAsync(Url).Result;
        File.WriteAllBytes(FileName, bytes);
        // --- FIN DEL BLOQUEO ---

        stopwatch.Stop();
        Console.WriteLine($"\n\n¡Descarga completada! Tiempo total: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");

        var info = new FileInfo(FileName);
        Console.WriteLine($"Archivo guardado ({info.Length} bytes).");
    }

    private static async Task EjecutarDescargaAsincronaAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine("Iniciando descarga asíncrona EN SEGUNDO PLANO.");
        Console.WriteLine("==> LA APLICACIÓN SEGUIRÁ RESPONDIENDO. Escribe en cualquier momento. <==");

        // 1. Inicia la operación de descarga pero NO la esperamos todavía.
        //    Esto devuelve una 'Task' que representa el trabajo en curso.
        Task<byte[]> downloadTask = _httpClient.GetByteArrayAsync(Url);

        // 2. Mientras la tarea de descarga no haya terminado, nuestro hilo principal
        //    sigue libre para hacer otras cosas, como ejecutar este bucle.
        while (!downloadTask.IsCompleted)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                Console.Write(".");
            }
            // Hacemos una pequeña pausa asíncrona para no consumir 100% CPU.
            await Task.Delay(100);
        }

        // 3. Ahora que el bucle terminó (porque la descarga se completó),
        //    podemos usar 'await' para obtener el resultado de forma segura.
        var bytes = await downloadTask;
        await File.WriteAllBytesAsync(FileName, bytes);

        stopwatch.Stop();
        Console.WriteLine($"\n\n¡Descarga completada! Tiempo total: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");

        var info = new FileInfo(FileName);
        Console.WriteLine($"Archivo guardado ({info.Length} bytes).");
    }
}
