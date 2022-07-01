using System.Collections.Generic;

namespace Calculator
{
	internal class RegularExpression
	{
		private readonly Yagiey.Lib.RegularExpressions.RegularExpression _regex;

		public string Source
		{
			get;
			private set;
		}

		public TokenType Type
		{
			get;
			private set;
		}

		public RegularExpression(string source, bool ignoreCase, TokenType tokenType)
		{
			_regex = new Yagiey.Lib.RegularExpressions.RegularExpression(source, ignoreCase);
			Source = source;
			Type = tokenType;
		}

		#region IDeterministicFiniteAutomaton

		public void Reset()
		{
			_regex.Reset();
		}

		public void MoveNext(char input)
		{
			_regex.MoveNext(input);
		}

		public bool IsInitialState()
		{
			return _regex.IsInitialState();
		}

		public bool IsAcceptable()
		{
			return _regex.IsAcceptable();
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			foreach (char ch in source)
			{
				MoveNext(ch);
			}
			return IsAcceptable();
		}

		public bool IsError()
		{
			return _regex.IsError();
		}

		public bool IsNextError(char input)
		{
			return _regex.IsNextError(input);
		}

		#endregion
	}
}
