// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_Fill
    {
        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Int(int size)
        {
            var a = new int[size];
            var span = new Span<int>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Fill(42);
                    }
                }
            }
        }

        [Benchmark]
        public void JoinAMany()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random(42);
            string[] paths1 = new string[10000];
            for (int i = 0; i < paths1.Length; i++)
            {
                var stringChars = new char[random.Next(25, 75)];
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                if (stringChars.Length < 50) stringChars[stringChars.Length - 1] = DirectorySeparatorChar;
                paths1[i] = new string(stringChars);
            }

            random = new Random(5);
            string[] paths2 = new string[10000];
            for (int i = 0; i < paths2.Length; i++)
            {
                var stringChars = new char[random.Next(25, 75)];
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                if (stringChars.Length < 50) stringChars[0] = DirectorySeparatorChar;
                paths2[i] = new string(stringChars);
            }

            for (int i = 0; i < paths2.Length; i++)
            {
                string temp1 = paths1[i];
                string temp2 = paths2[i];
                Span<char> dest = new char[temp1.Length + temp2.Length + 2];
                TryJoinA(temp1, temp2, dest, out int charsw);
                dest = dest.Slice(0, charsw);

                Console.WriteLine(temp1 + "|" + temp2 + "|" +  new string(dest));
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < paths1.Length; i++)
                    {
                        string path1 = paths1[i];
                        string path2 = paths2[i];
                        Span<char> destination = new char[path1.Length + path2.Length + 2];
                        TryJoinA(path1, path2, destination, out int charsWritten);
                        destination = destination.Slice(0, charsWritten);
                    }
                }
            }
        }

        [Benchmark]
        public void JoinBMany()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random(42);
            string[] paths1 = new string[10000];
            for (int i = 0; i < paths1.Length; i++)
            {
                var stringChars = new char[random.Next(5, 25)];
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                if (stringChars.Length < 15) stringChars[stringChars.Length - 1] = DirectorySeparatorChar;
                paths1[i] = new string(stringChars);
            }

            random = new Random(5);
            string[] paths2 = new string[10000];
            for (int i = 0; i < paths2.Length; i++)
            {
                var stringChars = new char[random.Next(5, 25)];
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                if (stringChars.Length < 15) stringChars[0] = DirectorySeparatorChar;
                paths2[i] = new string(stringChars);
            }

            for (int i = 0; i < paths2.Length; i++)
            {
                string temp1 = paths1[i];
                string temp2 = paths2[i];
                Span<char> dest = new char[temp1.Length + temp2.Length + 2];
                TryJoinA(temp1, temp2, dest, out int charsw);
                dest = dest.Slice(0, charsw);

                Console.WriteLine(temp1 + "|" + temp2 + "|" +  new string(dest));
            }
            
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < paths1.Length; i++)
                    {
                        string path1 = paths1[i];
                        string path2 = paths2[i];
                        Span<char> destination = new char[path1.Length + path2.Length + 2];
                        TryJoinB(path1, path2, destination, out int charsWritten);
                        destination = destination.Slice(0, charsWritten);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [InlineData("abcdefg", "hijklmn")]
        [InlineData("abcdefg/", "/hijklmn")]
        [InlineData("abcdefg", "/hijklmn")]
        [InlineData("abcdefg/", "hijklmn")]
        public void JoinA(string path1, string path2)
        {
            Span<char> destination = new char[path1.Length + path2.Length + 2];
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            bool result = false;
            int charsWritten = 0;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        result = TryJoinA(path1, path2, destination, out charsWritten);
                    }
                }
            }

            Assert.True(result);

            Span<char> expected = new char[path1.Length + path2.Length + 2];
            TryJoinB(path1, path2, expected, out int charsWritten2);

            Assert.Equal(new string(expected.Slice(0, charsWritten2)), new string(destination.Slice(0, charsWritten)));
        }

        [Benchmark(InnerIterationCount = 100000)]
        [InlineData("abcdefg", "hijklmn")]
        [InlineData("abcdefg/", "/hijklmn")]
        [InlineData("abcdefg", "/hijklmn")]
        [InlineData("abcdefg/", "hijklmn")]
        public void JoinB(string path1, string path2)
        {
            Span<char> destination = new char[path1.Length + path2.Length + 2];
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            bool result = false;
            int charsWritten = 0;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        result = TryJoinB(path1, path2, destination, out charsWritten);
                    }
                }
            }

            Assert.True(result);

            Span<char> expected = new char[path1.Length + path2.Length + 2];
            TryJoinA(path1, path2, expected, out int charsWritten2);

            Assert.Equal(new string(expected.Slice(0, charsWritten2)), new string(destination.Slice(0, charsWritten)));
        }

        [Benchmark(InnerIterationCount = 10000)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void JoinALarge(bool path1End, bool path2Start)
        {
            string path1 = "";
            string path2 = "";

            for (int i = 0; i < 100; i++)
            {
                path1 += "a";
                path2 += "b";
            }
            
            if (path1End) path1+= "/";
            if (path2Start) path2 = "/" + path2;

            Span<char> destination = new char[path1.Length + path2.Length + 2];
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            bool result = false;
            int charsWritten = 0;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        result = TryJoinA(path1, path2, destination, out charsWritten);
                    }
                }
            }

            Assert.True(result);

            Span<char> expected = new char[path1.Length + path2.Length + 2];
            TryJoinB(path1, path2, expected, out int charsWritten2);

            Assert.Equal(new string(expected.Slice(0, charsWritten2)), new string(destination.Slice(0, charsWritten)));
        }

        [Benchmark(InnerIterationCount = 10000)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void JoinBLarge(bool path1End, bool path2Start)
        {
            string path1 = "";
            string path2 = "";

            for (int i = 0; i < 100; i++)
            {
                path1 += "a";
                path2 += "b";
            }
            
            if (path1End) path1+= "/";
            if (path2Start) path2 = "/" + path2;

            Span<char> destination = new char[path1.Length + path2.Length + 2];
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            bool result = false;
            int charsWritten = 0;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        result = TryJoinB(path1, path2, destination, out charsWritten);
                    }
                }
            }

            Assert.True(result);

            Span<char> expected = new char[path1.Length + path2.Length + 2];
            TryJoinA(path1, path2, expected, out int charsWritten2);

            Assert.Equal(new string(expected.Slice(0, charsWritten2)), new string(destination.Slice(0, charsWritten)));
        }


        public static bool TryJoinA(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, Span<char> destination, out int charsWritten)
        {
            charsWritten = 0;
            if (path1.Length == 0 && path2.Length == 0)
                return true;

            if (path1.Length == 0 || path2.Length == 0)
            {
                ref ReadOnlySpan<char> pathToUse = ref path1.Length == 0 ? ref path2 : ref path1;
                if (destination.Length < pathToUse.Length)
                {
                    return false;
                }

                pathToUse.CopyTo(destination);
                charsWritten = pathToUse.Length;
                return true;
            }

            bool needsSeparator = !(EndsInDirectorySeparator(path1) || StartsWithDirectorySeparator(path2));
            int charsNeeded = path1.Length + path2.Length + (needsSeparator ? 1 : 0);
            if (destination.Length < charsNeeded)
                return false;

            path1.CopyTo(destination);
            if (needsSeparator)
                destination[path1.Length] = DirectorySeparatorChar;

            path2.CopyTo(destination.Slice(path1.Length + (needsSeparator ? 1 : 0)));

            charsWritten = charsNeeded;
            return true;
        }

        public static bool TryJoinB(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, Span<char> destination, out int charsWritten)
        {
            charsWritten = 0;
            if (path1.IsEmpty)
            {
                if (path2.IsEmpty)
                    return true;
                if (destination.Length < path2.Length)
                    return false;

                path2.CopyTo(destination);
                charsWritten = path2.Length;
                return true;
            }
            else if (path2.IsEmpty)
            {
                if (destination.Length < path1.Length)
                    return false;

                path1.CopyTo(destination);
                charsWritten = path1.Length;
                return true;
            }

            int charsNeeded = path1.Length + path2.Length;
            if (!(IsDirectorySeparator(path1[path1.Length - 1]) || IsDirectorySeparator(path2[0])))
            {
                charsNeeded++;

                if (destination.Length < charsNeeded)
                    return false;
                path1.CopyTo(destination);
                destination[path1.Length] = DirectorySeparatorChar;
                path2.CopyTo(destination.Slice(path1.Length + 1));
            }
            else
            {
                if (destination.Length < charsNeeded)
                    return false;
                path1.CopyTo(destination);
                path2.CopyTo(destination.Slice(path1.Length));
            }

            charsWritten = charsNeeded;
            return true;
        }

        internal static bool EndsInDirectorySeparator(ReadOnlySpan<char> path) => path.Length > 0 && IsDirectorySeparator(path[path.Length - 1]);

        internal static bool StartsWithDirectorySeparator(ReadOnlySpan<char> path) => path.Length > 0 && IsDirectorySeparator(path[0]);

        internal const char DirectorySeparatorChar = '/';
        internal const char DirectorySeparatorChar2 = '\\';

        internal static bool IsDirectorySeparator(char c)
        {
           return c == DirectorySeparatorChar || c == DirectorySeparatorChar2;
        }

    }
}
