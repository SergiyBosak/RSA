using System;
using System.Numerics;
using System.Collections.Generic;
using RSA.Common;

namespace RSA
{
    class Program
    {        
        static void Main(string[] args)
        {
            while (true)
            {
                if (!RSA.GetKeysAndN(out BigInteger publicKey, out BigInteger privateKey, out BigInteger n))
                {
                    Console.WriteLine($"Ошибка: не удалось сгенерировать простые числа за { RSA.MaxIterationCount } попыток");
                    Console.WriteLine("Нажмите Escape, чтобы выйти или любую клавишу, чтобы повторить");

                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;

                    continue;
                }

                Console.WriteLine("N: " + n);
                Console.WriteLine("Открытый ключ: " + publicKey);
                Console.WriteLine("Закрытый ключ: " + privateKey);

                Console.WriteLine();

                string text = IO.EnterText("Введите шифруемый текст:");

                List<BigInteger> result = RSA.EncriptRSA(text, publicKey, n);

                Console.WriteLine("Шифрованное сообщение:");

                foreach (var item in result)
                    Console.WriteLine(item);

                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу, чтобы расшифровать");
                Console.ReadKey();

                text = RSA.DecriptRSA(result, privateKey, n);

                Console.WriteLine(text);

                Console.WriteLine("Нажмите Escape, чтобы выйти или любую клавишу, чтобы повторить");

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;

                Console.WriteLine();
            }
        }
    }
}