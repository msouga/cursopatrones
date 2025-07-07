// Program.cs: El menú principal para ejecutar las demos.

Console.Title = "Curso Patrones .NET - Sesión 1: async/await";

string? option;
do
{
    DisplayMenu();
    option = Console.ReadLine();
    await RunDemoAsync(option);

} while (option != "0");

void DisplayMenu()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("=====================================================");
    Console.WriteLine("  Demostraciones de Asincronía en .NET (`async/await`)");
    Console.WriteLine("=====================================================");
    Console.ResetColor();
    Console.WriteLine("\nElige una demostración:");
    Console.WriteLine("  1. Síncrono (Bloqueante) vs. Asíncrono (No Bloqueante)");
    Console.WriteLine("  2. Antipatrón: El peligro de `async void`");
    Console.WriteLine("  3. Antipatrón: El Deadlock con `.Result`");
    Console.WriteLine("  0. Salir");
    Console.Write("\nTu opción: ");
}

async Task RunDemoAsync(string? selection)
{
    switch (selection)
    {
        case "1":
            await SyncVsAsyncDemos.EjecutarDemo();
            break;
        case "2":
            await Antipatterns.DemostrarAsyncVoid();
            break;
        case "3":
            Antipatterns.DemostrarDeadlock();
            break;
        case "0":
            Console.WriteLine("Saliendo...");
            return;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Opción no válida. Inténtalo de nuevo.");
            Console.ResetColor();
            break;
    }

    Console.WriteLine("\nPresiona Enter para volver al menú...");
    Console.ReadLine();
}
