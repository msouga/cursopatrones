/// <summary>
/// Simula un servicio externo que a veces falla.
/// Lógica: Falla las 4 primeras llamadas, luego se recupera y funciona bien.
/// </summary>
public class SimulatedUnreliableService
{
    private int _callCount = 0;

    public Task CallAsync()
    {
        _callCount++;
        Console.Write("Llamando al servicio... ");

        // Simula la lógica de fallo
        if (_callCount <= 4)
        {
            // Las primeras 4 llamadas fallan
            throw new HttpRequestException($"Fallo simulado en el intento #{_callCount}");
        }

        // A partir de la 5ta llamada, el servicio se "recupera"
        // y responde correctamente.
        return Task.CompletedTask;
    }
}
