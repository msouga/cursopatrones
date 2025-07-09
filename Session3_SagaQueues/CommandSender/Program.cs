// CommandSender/Program.cs
using Azure.Messaging.ServiceBus;
using System.Text.Json;

Console.WriteLine("--- Command Sender (Orquestador) ---");

// Lee la configuración de una variable de entorno. ¡No es necesario cambiar este código!
var connectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
const string queueName = "pagos-procesar-orden";

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: La variable de entorno 'SERVICEBUS_CONNECTION_STRING' no está configurada.");
    Console.WriteLine("Asegúrese de ejecutar el script de configuración de Azure en esta terminal.");
    Console.ResetColor();
    return;
}

await using var client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(queueName);

Console.WriteLine("Presione 'S' para simular un pedido exitoso.");
Console.WriteLine("Presione 'F' para simular un pedido fallido (stock insuficiente).");
Console.WriteLine("Presione 'Q' para salir.");

while (true)
{
    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
    if (key.Key == ConsoleKey.Q) break;
    bool simulateSuccess = key.Key == ConsoleKey.S;
    // Crear generador de números aleatorios
    var random = new Random();

    // Generar un número decimal aleatorio entre 1 y 500, redondeado a 2 decimales
    decimal randomAmount = Math.Round((decimal)(random.NextDouble() * 499.0 + 1.0), 2);

    // Crear el comando
    var orderCommand = new ProcessOrderCommand(
        Guid.NewGuid(),
        "user-123",
        simulateSuccess ? "PROD-SUCCESS" : "PROD-FAIL",
        randomAmount
    );
    string messageBody = JsonSerializer.Serialize(orderCommand);
    var serviceBusMessage = new ServiceBusMessage(messageBody);
    try
    {
        await sender.SendMessageAsync(serviceBusMessage);
        Console.WriteLine($"\n[OK] Comando enviado a la cola '{queueName}' para la orden: {orderCommand.OrderId}");
        Console.WriteLine($"\t- Simulación: {(simulateSuccess ? "ÉXITO" : "FALLO")}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ERROR] No se pudo enviar el mensaje: {ex.Message}");
    }
}

Console.WriteLine("\nCerrando el Command Sender...");
public record ProcessOrderCommand(Guid OrderId, string UserId, string ProductId, decimal Amount);