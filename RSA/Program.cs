using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;
using System.Diagnostics;

namespace RSA
{
    class Program
    {
        const string ERROR_LOG = "errors.log";

        static void Main(string[] args)
        {
            while (true)
            {
                //CompareFunc_simpleNums();

                simpleNums(out BigInteger p, out BigInteger q);

                BigInteger Fn = (p - 1) * (q - 1);

                Console.WriteLine("Функция Эйлера: " + Fn);

                BigInteger n = p * q;

                Console.WriteLine("n: " + n);

                BigInteger e = exp(Fn);

                Console.WriteLine("Открытый ключ: " + e);

                BigInteger d = modInverse(e, Fn);

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

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            }
        }

        static List<BigInteger> EncriptRSA(string alphabet, string text, BigInteger e, BigInteger n)
        {
            List<BigInteger> result = new List<BigInteger>();

            BigInteger index, indexResult;

            BigInteger a = 0;

            for (int i = 0; i < text.Length; i++)
            {
                indexResult = alphabet.IndexOf(text[i]) + 2;

                index = (indexResult + a) % n;

                a = index;

                result.Add(BigInteger.ModPow(index, e, n));
            }                
 
            return result;
        }

        //static void Main_old(string[] args)
        //{
        //    while (true)
        //    {
        //        int Fn = eilerFunc();

        //        Console.WriteLine("Функция Эйлера: " + Fn);

        //        int n = ModN();

        //        Console.WriteLine("n: " + n);

        //        int e = exp(Fn);

        //        Console.WriteLine("Открытый ключ: " + e);

        //        int d = DKey(e, Fn);

        //        Console.WriteLine("Закрытый ключ: " + d);

        //        string text = enterText("введите шифруемый текст:");

        //        string alphabet = string.Empty;

        //        foreach (var item in text.Distinct())
        //            alphabet += item;

        //        List<BigInteger> result = EncriptRSA(alphabet, text, e, n);

        //        foreach (var item in result)
        //            Console.WriteLine(item);

        //        Console.WriteLine("Шифрованное сообщение");

        //        text = result.ToString();

        //        Console.WriteLine("Нажмите любую клавишу, чтобы расшифровать");
        //        Console.ReadKey();

        //        text = DecriptRSA(alphabet, result, d, n);

        //        Console.WriteLine(text);

        //        if (Console.ReadKey().Key == ConsoleKey.Escape)
        //            break;
        //    }
        //}

        static string DecriptRSA(string alphabet, List<BigInteger> input, BigInteger d, BigInteger n)
        {
            string result = string.Empty;

            int i = 0;

            BigInteger[] Mas = new BigInteger[input.Count];

            try
            {
                BigInteger index;

                BigInteger[] indexMas = new BigInteger[input.Count];

                for (i = 0; i < input.Count; i++)
                {
                    index = input[i];

                    index = BigInteger.ModPow(index, d, n);

                    indexMas[i] = indexMas[i] + index;
                }

                for (i = 0; i < input.Count; i++)
                {
                    if (i == 0)
                        Mas[i] = indexMas[i];
                    else
                        Mas[i] = indexMas[i] - indexMas[i - 1];

                    result += alphabet[(int)(Mas[i] - 2)];
                }
            }
            catch (Exception ex)
            {
                if (!File.Exists(ERROR_LOG))
                    using (var file = File.Create(ERROR_LOG))
                    {

                    }

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
        
        //static int DKey(int e, int Fn)
        //{
        //    return modInverse(e, Fn);
        //}

        static BigInteger exp(BigInteger Fn)
        {
            BigInteger result;

            do
            {
                Random rdm = new Random();
                
                result = GetRandomBigInteger(10, Fn, Environment.TickCount);
            }
            while (NOD(result, Fn) != 1);

            return result;
        }

        //static int exp_old(int Fn)
        //{
        //    BigInteger e;

        //    do
        //    {
        //        Random rdm = new Random();

        //        e = rdm.Next(10, Fn);
        //    }
        //    while (NOD(e, Fn) != 1);

        //    return e;
        //}

        static int GetInt32FromBigInteger(BigInteger value)
        {
            var valueBytes = value.ToByteArray();

            int result = 0;
            int length = Math.Min(valueBytes.Length, 4);

            for (int i = 0; i < length; i++)
                result += valueBytes[i] << (i * 8);

            return result;
        }

        public static BigInteger GetRandomBigInteger(BigInteger minValue, BigInteger maxValue, BigInteger seed, bool check2Division = false)
        {
            if (maxValue < minValue || maxValue == 0)
                return -1;

            byte[] bytes = maxValue.ToByteArray();

            BigInteger result;

            var random = new Random(GetInt32FromBigInteger(seed));

            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; // force sign bit to positive

                result = new BigInteger(bytes);
            }
            while ((check2Division && result % 2 == 0) || result < minValue || result > maxValue);

            return result;
        }

        static BigInteger eilerFunc()
        {

            simpleNums(out BigInteger p, out BigInteger q);

            BigInteger result;

            result = (p - 1) * (q - 1);

            return result;
        }

        static BigInteger ModN()
        {
            simpleNums(out BigInteger p, out BigInteger q);

            BigInteger result;

            result = p * q;

            return result;
        }

        static void simpleNums(out BigInteger p, out BigInteger q)
        {
            int iterationCount = 0;

            BigInteger pResult = Environment.TickCount;
            BigInteger qResult = 0;

            var watch = Stopwatch.StartNew();

            do
            {
                ++iterationCount;

                pResult = GetRandomBigInteger(1000000000, 9999999999, pResult, true);
                qResult = GetRandomBigInteger(1000000000, 9999999999, pResult, true);
            }
            while (!(simpleNum1(pResult) && simpleNum1(qResult)));

            watch.Stop();

            Console.WriteLine($"Затрачено времени на генерацию простых чисел: { watch.ElapsedMilliseconds } мс");
            Console.WriteLine($"Потребовалось итераций: { iterationCount }");

            p = pResult;
            q = qResult;
        }

        static void CompareFunc_simpleNums()
        {
            var watch = Stopwatch.StartNew();
            simpleNums_old(out int p_old, out int q_old);
            watch.Stop();
            Console.WriteLine($"Затрачено времени на выполнения метода simpleNums_old: { watch.ElapsedTicks }{ Environment.NewLine }P = { p_old }; Q = { q_old }");

            watch = Stopwatch.StartNew();
            simpleNums(out BigInteger p, out BigInteger q);
            watch.Stop();
            Console.WriteLine($"Затрачено времени на выполнения метода simpleNums: { watch.ElapsedTicks }{ Environment.NewLine }P = { p }; Q = { q }");
        }

        static void simpleNums_old(out int p, out int q)
        {
            int iterationCount = 0;

            do
            {
                ++iterationCount;

                Random rdm = new Random();

                p = rdm.Next(1000, 10000);
                q = rdm.Next(1000, 10000);
            }
            while (!(simpleNum1(p) && simpleNum1(q)));

            Console.WriteLine($"Потребовалось итераций: { iterationCount }");
        }

        static BigInteger NOD(BigInteger a, BigInteger b)
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

        static bool simpleNum1(BigInteger n)
        {
            if (n % 2 == 0)
                return false;

            BigInteger d = 3;
            
            while (d * d <= n && n % d != 0)
                d += 2;

            return d * d > n;
        }

        //static bool simpleNum1_old(long n)
        //{
        //    if (n % 2 == 0)
        //        return false;

        //    int d = 3;

        //    while (d * d <= n && n % d != 0)
        //        d += 2;

        //    return d * d > n;
        //}

        static BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;

            while (a > 0)
            {
                BigInteger t = i / a, x = a;

                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }

            v %= n;

            if (v < 0)
                v = (v + n) % n;

            return v;
        }

        //static int modInverse_old(int a, int n)
        //{
        //    int i = n, v = 0, d = 1;
        //    while (a > 0)
        //    {
        //        int t = i / a, x = a;
        //        a = i % x;
        //        i = x;
        //        x = d;
        //        d = v - t * x;
        //        v = x;
        //    }
        //    v %= n;
        //    if (v < 0) v = (v + n) % n;
        //    return v;
        //}

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
