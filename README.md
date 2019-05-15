# SharpUtils


Example:
```cs
Console.WriteLine(Console.ForegroundColor); // white
using (ScopedAction.Create(() => Console.ForegroundColor, ConsoleColor.Red))
{
    Console.WriteLine(Console.ForegroundColor); // red
}
Console.WriteLine(Console.ForegroundColor); // white
```