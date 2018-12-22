using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace RSA
{
    class Program
    {
        const string ERROR_LOG = "errors.log";

        static void Main(string[] args)
        {
            while (true)
            {
                int Fn = eilerFunc();

                Console.WriteLine("Функция Эйлера: " + Fn);

                int n = ModN();

                Console.WriteLine("n: " + n);

                int e = exp(Fn);

                Console.WriteLine("Открытый ключ: " + e);

                int d = DKey(e, Fn);

                Console.WriteLine("Закрытый ключ: " + d);

                string text = enterText("введите шифруемый текст:");

                string alphabet = string.Empty;

                foreach (var item in text.Distinct())
                    alphabet += item;

                List<BigInteger> result = EncriptRSA(alphabet, text, e, n);

                foreach (var item in result)
                    Console.WriteLine(item);

                Console.WriteLine("Шифрованное сообщение");

                text = result.ToString();

                Console.WriteLine("Нажмите любую клавишу, чтобы расшифровать");
                Console.ReadKey();

                text = DecriptRSA(alphabet, result, d, n);

                Console.WriteLine(text);

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        static List<BigInteger> EncriptRSA(string alphabet, string text, int e, int n)
        {
            List<BigInteger> result = new List<BigInteger>();

            int index, indexResult;

            int a = 0;

            for (int i = 0; i < text.Length; i++)
            {
                indexResult = alphabet.IndexOf(text[i]) + 2;

                index = (indexResult + a) % n;

                a = index;

                result.Add(BigInteger.ModPow(index, e, n));
            }                
 
            return result;
        }



        static string DecriptRSA(string alphabet, List<BigInteger> input, int d, int n)
        {
            string result = string.Empty;

            int i = 0;

            int[] Mas = new int[input.Count];

            try
            {
                int index;

                int[] indexMas = new int[input.Count];

                for (i = 0; i < input.Count; i++)
                {
                    index = (int)input[i];

                    index = (int)BigInteger.ModPow(index, d, n);

                    indexMas[i] = indexMas[i] + index;
                }

                for (i = 0; i < input.Count; i++)
                {
                    if (i == 0)
                        Mas[i] = indexMas[i];
                    else
                        Mas[i] = indexMas[i] - indexMas[i - 1];
                    result += alphabet[Mas[i] - 2];
                }
            }
            catch (Exception ex)
            {
                if (!File.Exists(ERROR_LOG))
                    File.Create(ERROR_LOG);
                using (var log = new StreamWriter(ERROR_LOG, true))
                {
                    log.WriteLine("alphabet: " + alphabet);
                    log.WriteLine("alphabet length: " + alphabet.Length);
                    log.WriteLine("d: " + d);
                    log.WriteLine("n: " + n);
                    log.WriteLine("input[i]: " + input[i]);
                    log.WriteLine("Mas[i]: " + Mas[i]);
                }
                Console.WriteLine(ex.Message);
            }

            return result;
        }
         
        static int DKey(int e, int Fn)
        {
            return modInverse(e, Fn);
        }

        static int exp(int Fn)
        {
            int e;
            do
            {
                Random rdm = new Random();

                e = rdm.Next(10, Fn);
            }
            while (NOD(e, Fn) != 1);

            return e;
        }

        static int eilerFunc()
        {

            simpleNums(out int p, out int q);

            int Fn;

            Fn = (p - 1) * (q - 1);

            return Fn;
        }

        static int ModN()
        {
            simpleNums(out int p, out int q);

            int n;
            
            n = p * q;

            return n;
        }

        static void simpleNums(out int p, out int q)
        {
            do
            {
                Random rdm = new Random();

                p = rdm.Next(1000, 10000);
                q = rdm.Next(1000, 10000);
            }
            while (!(simpleNum1(p) && simpleNum1(q)));

        }

        static int NOD(int a, int b)
        {
            while (b != 0)
                b = a % (a = b);

            return a;
        }

        static bool simpleNum(long a)
        {
            long max = (long)Math.Floor(Math.Sqrt(a));

            for (long i = 2; i <= max; i++)
            {
                if (a % i == 0)
                    return false;
            }

            return true;
        }

        static bool simpleNum1(long n)
        {
            if (n % 2 == 0)
            return false;

            int d = 3;
            while (d * d <= n && n % d != 0)
                d += 2;
            return d * d > n;
        }

        static int modInverse(int a, int n)
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        static string enterText(string text)
        {
            string result = null;

            do
                Console.WriteLine(text);
            while ((result = Console.ReadLine().Trim()) == string.Empty);

            return result;
        }

        static int enterNumber(string text)
        {
            int num = 0;

            do
                Console.Write(text);
            while (!int.TryParse(Console.ReadLine(), out num));

            return num;
        }

    }
}
