using SIIDecryptSharp;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var compressed = Decryptor.Decrypt(Path.Combine(Directory.GetCurrentDirectory(), "game.1.46.sii"));
            
        }
    }
}