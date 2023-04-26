﻿using SIIDecryptSharp;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var raw = Decryptor.Decrypt(Path.Combine(Directory.GetCurrentDirectory(), "game.1.47.0.sii"));
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "testing.1.47.0.txt"), System.Text.Encoding.UTF8.GetString(raw));
        }
    }
}