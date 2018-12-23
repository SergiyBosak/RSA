using System;
using System.Linq;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;

namespace RSA
{
    class Program
    {
        protected const int MAX_ITERATION_COUNT = 10000; // чтобы не зависнуть в случае чего
        protected const int SHIFT = 2;

        protected static string Alphabet;

        static void Main(string[] args)
        {
            while (true)
            {
                if (!GetKeysAndN(out BigInteger publicKey, out BigInteger privateKey, out BigInteger n))
                {
                    Console.WriteLine($"Ошибка: не удалось сгенерировать простые числа за { MAX_ITERATION_COUNT } попыток");
                    Console.WriteLine("Нажмите Escape, чтобы выйти или любую клавишу, чтобы повторить");

                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;

                    continue;
                }

                Console.WriteLine("N: " + n);
                Console.WriteLine("Открытый ключ: " + publicKey);
                Console.WriteLine("Закрытый ключ: " + privateKey);

                Console.WriteLine();

                string text = EnterText("Введите шифруемый текст:");

                List<BigInteger> result = EncriptRSA(text, publicKey, n);

                Console.WriteLine("Шифрованное сообщение:");

                foreach (var item in result)
                    Console.WriteLine(item);

                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу, чтобы расшифровать");
                Console.ReadKey();

                text = DecriptRSA(result, privateKey, n);

                Console.WriteLine(text);

                Console.WriteLine("Нажмите Escape, чтобы выйти или любую клавишу, чтобы повторить");

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;

                Console.WriteLine();
            }
        }

        protected static bool GetKeysAndN(out BigInteger publicKey, out BigInteger privateKey, out BigInteger n)
        {
            publicKey = -1;
            privateKey = -1;
            n = -1;

            if (!GetSimpleNums(out BigInteger p, out BigInteger q))
                return false;

            BigInteger Fn = (p - 1) * (q - 1);
            Console.WriteLine("Функция Эйлера: " + Fn);

            publicKey = Exp(Fn);
            privateKey = ModInverse(publicKey, Fn);

            n = p * q;

            return true;
        }

        protected static void SetAlphabet(string fromText)
        {
            Alphabet = string.Empty;

            foreach (var item in fromText.Distinct())
                Alphabet += item;
        }

        protected static bool GetSimpleNums(out BigInteger p, out BigInteger q)
        {
            p = -1;
            q = -1;

            int iterationCount = 0;

            BigInteger min = new BigInteger(10000000000);
            BigInteger max = new BigInteger(99999999999);

            BigInteger pResult = Environment.TickCount;
            BigInteger qResult = DateTime.Now.Ticks;

            var watch = Stopwatch.StartNew();

            do
            {
                pResult = GetRandomBigInteger(qResult, min, max, true);
                qResult = GetRandomBigInteger(pResult, min, max, true);
            }
            while (++iterationCount < MAX_ITERATION_COUNT && !(IsSimpleNumber(pResult) && IsSimpleNumber(qResult)));

            watch.Stop();

            if (iterationCount == MAX_ITERATION_COUNT)
                return false;

            p = pResult;
            q = qResult;

            Console.WriteLine($"Затрачено времени на генерацию простых чисел: { watch.ElapsedMilliseconds } мс");
            Console.WriteLine($"Потребовалось итераций: { iterationCount }");
            Console.WriteLine($"P: { pResult }");
            Console.WriteLine($"Q: { qResult }");

            return true;
        }

        protected static BigInteger ModInverse(BigInteger a, BigInteger n)
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

        public static List<BigInteger> EncriptRSA(string text, BigInteger e, BigInteger n)
        {
            SetAlphabet(text);

            List<BigInteger> result = new List<BigInteger>();

            BigInteger index = 0;

            foreach (var letter in text)
            {
                index = (Alphabet.IndexOf(letter) + index + SHIFT) % n;

                result.Add(BigInteger.ModPow(index, e, n));
            }

            return result;
        }

        public static string DecriptRSA(List<BigInteger> input, BigInteger d, BigInteger n)
        {
            string result = string.Empty;

            BigInteger currentValue = 0;
            BigInteger previouslyValue = 0;

            foreach (var item in input)
            {
                currentValue = BigInteger.ModPow(item, d, n);

                result += Alphabet[(int)(currentValue - previouslyValue) - SHIFT];

                previouslyValue = currentValue;
            }

            return result;
        }

        protected static BigInteger Exp(BigInteger Fn)
        {
            BigInteger result;

            do
                result = GetRandomBigInteger(DateTime.Now.Millisecond, 10, Fn, true);
            while (BigInteger.GreatestCommonDivisor(result, Fn) != 1);

            return result;
        }

        protected static BigInteger GetRandomBigInteger(BigInteger seed, BigInteger minValue, BigInteger maxValue, bool checkThatNotIsEven = false)
        {
            if (maxValue < minValue || maxValue == 0)
                return -1;

            byte[] bytes = maxValue.ToByteArray();

            BigInteger result;

            var random = new Random(GetInt32FromBigInteger(seed));

            do
            {
                random.NextBytes(bytes);

                bytes[bytes.Length - 1] &= 0x7F; // принудительно делаем положительным числом

                result = new BigInteger(bytes);
            }
            while (
                (checkThatNotIsEven && result.IsEven)
                || result < minValue
                || result > maxValue
            );

            return result;
        }

        protected static int GetInt32FromBigInteger(BigInteger value)
        {
            var valueBytes = value.ToByteArray();

            int result = 0;
            int length = Math.Min(valueBytes.Length, 4);

            for (int i = 0; i < length; i++)
                result += valueBytes[i] << (i * 8);

            return result;
        }

        protected static bool IsSimpleNumber(BigInteger number)
        {
            if (number.IsEven)
                return false;

            BigInteger d = 3;
            
            while (d * d <= number && number % d != 0)
                d += 2;

            return d * d > number;
        }

        protected static string EnterText(string message)
        {
            string result = null;

            do
                Console.WriteLine(message);
            while ((result = Console.ReadLine().Trim()) == string.Empty);

            return result;
        }

        protected static int EnterNumber(string message)
        {
            int num = 0;

            do
                Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out num));

            return num;
        }

/*
        static void Main_old(string[] args)
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



        static int DKey(int e, int Fn)
        {
            return modInverse(e, Fn);
        }

        static int exp_old(int Fn)
        {
            BigInteger e;

            do
            {
                Random rdm = new Random();

                e = rdm.Next(10, Fn);
            }
            while (NOD(e, Fn) != 1);

            return e;
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

        static void CompareFunc_simpleNums()
        {
            var watch = Stopwatch.StartNew();
            simpleNums_old(out int p_old, out int q_old);
            watch.Stop();
            Console.WriteLine($"Затрачено времени на выполнения метода simpleNums_old: { watch.ElapsedTicks }{ Environment.NewLine }P = { p_old }; Q = { q_old }");

            watch = Stopwatch.StartNew();
            GetSimpleNums(out BigInteger p, out BigInteger q);
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
            while (!(IsSimpleNumber(p) && IsSimpleNumber(q)));

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

        static bool simpleNum1_old(long n)
        {
            if (n % 2 == 0)
                return false;

            int d = 3;

            while (d * d <= n && n % d != 0)
                d += 2;

            return d * d > n;
        }

        static int modInverse_old(int a, int n)
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
*/
    }
}