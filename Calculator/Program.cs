using System;

namespace Calculator
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			const string src = @"1 2 3 23 42 12.3 -1 -2 -3 -23 -42 -12.3 abc + - * / % a+b; let pi=3.141592;";
			Lexer l = new(src);
			foreach (var t in l)
			{
				Console.WriteLine(t.Item1);
			}
		}
	}
}
