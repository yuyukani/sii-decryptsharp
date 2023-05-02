using SIIDecryptSharp;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("game.sii"))
            {
                try
                {
                    var raw = Decryptor.Decrypt(Path.Combine(Directory.GetCurrentDirectory(), "game.sii"));
                    File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "game.decrypted.sii"), System.Text.Encoding.UTF8.GetString(raw));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception");
                    Console.WriteLine(ex.Message.ToString());
                    Console.WriteLine("\r\n\r\nStack:");
                    Console.WriteLine(ex.StackTrace?.ToString() ?? string.Empty);
                }
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