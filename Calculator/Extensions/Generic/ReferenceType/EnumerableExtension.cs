using System;
using System.Collections.Generic;

namespace Calculator.Extensions.Generic.ReferenceType
{
	internal static class EnumerableExtension
	{
		public static IEnumerable<Tuple<T, T?[]>> EnumerateLookingAhead<T>(IEnumerable<T> e, int length) where T : class
		{
			if (length < 1)
			{
				string msg = "value must be greater than or equal to 1.";
				throw new ArgumentException(msg, nameof(length));
			}

			T?[] future = new T?[length];
			int i = 0;
			foreach (var item in e)
			{
				T? current = future[0];
				for (int j = 0; j < length - 1; j++)
				{
					future[j] = future[j + 1];
				}
				future[length - 1] = item;

				if (length - 1 < i)
				{
					yield return Tuple.Create(current!, future);
				}
				i++;
			}

			int n = 0;
			if (i < length)
			{
				n = length - i;
			}

			for (int j = 0; j < length; j++)
			{
				T? current = future[0];
				for (int k = 0; k < length - 1; k++)
				{
					future[k] = future[k + 1];
				}
				future[length - 1] = null;

				if (n <= 0 && current != null)
				{
					yield return Tuple.Create(current, future);
				}
				n--;
			}
		}
	}
}
