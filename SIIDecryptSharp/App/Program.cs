using SIIDecryptSharp;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool decode = true;
            foreach (string s in args)
            {
                if (String.Compare(s, "--DecryptOnly", comparisonType: StringComparison.OrdinalIgnoreCase) == 0)
                {
                    decode = false;
                    continue;
                }
                if (!File.Exists(s))
                {
                    Console.WriteLine(s + " file does not exist. skipping.");
                    continue;
                }
                Console.WriteLine("decrypting " + s);

                try
                {
                    var raw = Decryptor.Decrypt(s, decode);
                    if (raw.Length > 0)
                    {
                        var dir = Path.GetDirectoryName(s);
                        if (dir == null) dir = "";
                        string bakfn = Path.Combine(dir, Path.GetFileName(s) + ".bak");
                        if (File.Exists(bakfn))
                        {
                            File.Delete(bakfn);
                        }
                        File.Move(s, bakfn);
                        File.WriteAllBytes(s, raw);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception");
                    Console.WriteLine(ex.Message.ToString());
                    Console.WriteLine("\r\n\r\nStack:");
                    Console.WriteLine(ex.StackTrace?.ToString() ?? string.Empty);
                    Console.ReadKey();
                }
            }
        }
    }
}