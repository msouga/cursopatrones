// PaymentServiceReceiver/Program.cs
using Azure.Messaging.ServiceBus;
using System.Text.Json;

Console.WriteLine("--- Payment Service Receiver (Worker) ---");

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
ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions { AutoCompleteMessages = false, MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5) });

processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;

await processor.StartProcessingAsync();
Console.WriteLine($"Escuchando la cola '{queueName}'. Presione cualquier tecla para detener...");
Console.ReadKey();

Console.WriteLine("\nDeteniendo el receptor...");
await processor.StopProcessingAsync();
Console.WriteLine("Receptor detenido.");

async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"\n[RECIBIDO] Mensaje. Intentando procesar...");
    try
    {
        var command = JsonSerializer.Deserialize<ProcessOrderCommand>(body);
        if (command == null) { await args.DeadLetterMessageAsync(args.Message, "Invalid message body"); return; }
        Console.WriteLine($"--> Iniciando Saga para Orden: {command.OrderId}");
        Console.WriteLine("1. Validando orden... OK");
        await Task.Delay(500);
        bool stockReserved = await ReserveStock(command.ProductId);
        if (!stockReserved) { throw new InvalidOperationException($"Stock insuficiente para el producto {command.ProductId}"); }
        await ProcessPayment(command.Amount);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"--> SAGA COMPLETADA con éxito para la orden: {command.OrderId}");
        Console.ResetColor();
        await args.CompleteMessageAsync(args.Message);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[FALLO] La saga ha fallado: {ex.Message}");
        Console.WriteLine("--> Iniciando transacciones de compensación...");
        await ReleaseStock();
        Console.WriteLine("--> Compensación finalizada.");
        Console.ResetColor();
        await args.DeadLetterMessageAsync(args.Message);
    }
}
Task ErrorHandler(ProcessErrorEventArgs args) {
    Console.WriteLine($"[ERROR DEL PROCESADOR] {args.Exception.Message}");
    return Task.CompletedTask;
}
async Task<bool> ReserveStock(string productId) {
    Console.WriteLine("2. Reservando stock...");
    await Task.Delay(1000);
    if (productId == "PROD-FAIL") {
        Console.WriteLine("\t- Fallo: Stock insuficiente.");
        return false;
    }
    Console.WriteLine("\t- Éxito: Stock reservado.");
    return true;
}
async Task ProcessPayment(decimal amount) {
    Console.WriteLine("3. Procesando pago...");
    await Task.Delay(1000);
    Console.WriteLine($"\t- Éxito: Cobrado ${amount}.");
}
async Task ReleaseStock(){
    Console.WriteLine("\t- [COMPENSACIÓN] Liberando stock reservado...");
    await Task.Delay(750);
    Console.WriteLine("\t- [COMPENSACIÓN] Stock liberado.");
}
public record ProcessOrderCommand(Guid OrderId, string UserId, string ProductId, decimal Amount);