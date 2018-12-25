using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Diagnostics;

namespace RSA
{
    static class RSA
    {
        public static int MaxIterationCount { get { return MAX_ITERATION_COUNT; } }

        private const int MAX_ITERATION_COUNT = 10000; // чтобы не зависнуть в случае чего
        private const int SHIFT = 2;

        private static string Alphabet;

        public static bool GetKeysAndN(out BigInteger publicKey, out BigInteger privateKey, out BigInteger n)
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

        private static void SetAlphabet(string fromText)
        {
            Alphabet = string.Empty;

            foreach (var item in fromText.Distinct())
                Alphabet += item;
        }

        private static bool GetSimpleNums(out BigInteger p, out BigInteger q)
        {
            p = -1;
            q = -1;

            int iterationCount = 0;

            BigInteger min = new BigInteger(100000);
            BigInteger max = new BigInteger(999999);

            BigInteger pResult = Environment.TickCount;
            BigInteger qResult = DateTime.Now.Ticks;

            var watch = Stopwatch.StartNew();

            do
            {
                pResult = GetRandomBigInteger(qResult, min, max, true);
                qResult = GetRandomBigInteger(pResult, min, max, true);
            }
            while (++iterationCount < MAX_ITERATION_COUNT && !(pResult != qResult && IsSimpleNumber(pResult) && IsSimpleNumber(qResult)));

            watch.Stop();

            if (iterationCount == MAX_ITERATION_COUNT)
                return false;

            p = pResult;
            q = qResult;

            Console.WriteLine($"Затрачено времени на генерацию простых чисел: { watch.ElapsedMilliseconds } мс");
            Console.WriteLine($"Потребовалось итераций: { iterationCount }");
            Console.WriteLine($"P: { p }");
            Console.WriteLine($"Q: { q }");

            return true;
        }

        private static BigInteger MersenNum(BigInteger Num)
        {
            return Num = BigInteger.Pow(2, (int)Num) - 1;
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger n)
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

            int[] intRes = new int[input.Count];

            BigInteger previouslyValue = 0;

            foreach (var item in input)
            {
                currentValue = BigInteger.ModPow(item, d, n);

                result += Alphabet[(int)(currentValue - previouslyValue) - SHIFT];

                previouslyValue = currentValue;
            }

            //for (int i = 0; i < input.Count; i++)
            //{
            //    currentValue = BigInteger.ModPow(input[i], d, n);

            //    intRes[i] = (int)(intRes[i] + currentValue);

            //    Console.WriteLine(intRes[i]);
            //}

            return result;
        }

        private static BigInteger Exp(BigInteger Fn)
        {
            BigInteger result;

            do
                result = GetRandomBigInteger(DateTime.Now.Millisecond, 10, Fn, true);
            while (BigInteger.GreatestCommonDivisor(result, Fn) != 1);

            return result;
        }

        private static BigInteger GetRandomBigInteger(BigInteger seed, BigInteger minValue, BigInteger maxValue, bool checkThatNotIsEven = false)
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

        private static int GetInt32FromBigInteger(BigInteger value)
        {
            var valueBytes = value.ToByteArray();

            int result = 0;
            int length = Math.Min(valueBytes.Length, 4);

            for (int i = 0; i < length; i++)
                result += valueBytes[i] << (i * 8);

            return result;
        }

        private static bool IsSimpleNumber(BigInteger number)
        {
            if (number.IsEven)
                return false;

            BigInteger d = 3;

            while (d * d <= number && number % d != 0)
                d += 2;

            return d * d > number;
        }

    }
}
