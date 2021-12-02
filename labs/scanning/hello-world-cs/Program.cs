using System;
using System.IO;

namespace hello_world
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = args.Length > 0 ? args[0] : "Hello, World";

            var path = Path.GetTempFileName();
            File.WriteAllText(path, text);

            var random = new Random();
            File.WriteAllText(path, $". {random.Next()}");
            
            Console.WriteLine(text);
        }
    }
}
