using SIIDecryptSharp;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("game.sii"))
            {
                var raw = Decryptor.Decrypt(Path.Combine(Directory.GetCurrentDirectory(), "game.sii"));
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "game.decrypted.sii"), System.Text.Encoding.UTF8.GetString(raw));
            }
            else
            {
                Console.WriteLine("game.sii file does not exist. skipping.");
            }
            Console.Write("Done decrypting. Press any key to exit...");
            Console.ReadKey();

        }
    }
}