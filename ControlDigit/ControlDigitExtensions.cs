using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace ControlDigit
{
    public static class ControlDigitExtensions
    {
        public static int ControlDigit(this long number)
        {
            int sum = 0;
            int factor = 1;
            do
            {
                int digit = (int) (number % 10);
                sum += factor * digit;
                factor = 4 - factor;
                number /= 10;
            } while (number > 0);

            int result = sum % 11;
            if (result == 10)
                result = 1;
            return result;
        }

        public static int ControlDigit2(this long number)
        {
            int controlSum = 0;
            var digitsList = GetDigit(number).ToList();
            var factorsList = GetFactor(digitsList.Count).ToList();
            for (int i = 0; i < digitsList.Count; i++)
            {
                controlSum += digitsList[i] * factorsList[i];
            }

            int result = controlSum % 11;
            if (result == 10)
                result = 1;
            return result;
        }

        public static IEnumerable<int> GetFactor(int length)
        {
            for (var i = 0; i < length / 2+1; i++)
            {
                yield return 1;
                yield return 3;
            }
        }

        public static IEnumerable<int> GetDigit(long number)
        {
            while (number > 0)
            {
                int digit = (int)(number % 10);
                yield return digit;
                number /= 10;
            }
        }
    }

    [TestFixture]
	public class ControlDigitExtensions_Tests
	{
		[TestCase(0, ExpectedResult = 0)]
		[TestCase(1, ExpectedResult = 1)]
		[TestCase(2, ExpectedResult = 2)]
		[TestCase(9, ExpectedResult = 9)]
		[TestCase(10, ExpectedResult = 3)]
		[TestCase(15, ExpectedResult = 8)]
		[TestCase(17, ExpectedResult = 1)]
		[TestCase(18, ExpectedResult = 0)]
		public int TestControlDigit(long x)
		{
			return x.ControlDigit();
		}

		[Test]
		public void CompareImplementations()
		{
			for (long i = 0; i < 100000; i++)
				Assert.AreEqual(i.ControlDigit(), i.ControlDigit2());
		}
	}

	[TestFixture]
	public class ControlDigit_PerformanceTests
	{
		[Test]
		public void TestControlDigitSpeed()
		{
			var count = 10000000;
			var sw = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
				12345678L.ControlDigit();
			Console.WriteLine("Old " + sw.Elapsed);
			sw.Restart();
			for (int i = 0; i < count; i++)
				12345678L.ControlDigit2();
			Console.WriteLine("New " + sw.Elapsed);
		}
	}
}
