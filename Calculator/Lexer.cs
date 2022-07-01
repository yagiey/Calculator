using Calculator.Extensions.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
	internal class Lexer : IEnumerable<Tuple<Token, Token?[]>>
	{
		private IEnumerable<RegularExpression> _automata;

		private readonly RegularExpression _comLine;
		private readonly RegularExpression _comBlock;
		private readonly RegularExpression _whiteSpaces;
		private readonly RegularExpression _literalInt;
		private readonly RegularExpression _literalReal;
		private readonly RegularExpression _identifier;
		private readonly RegularExpression _puncSemicolon;
		private readonly RegularExpression _puncEqual;
		private readonly RegularExpression _puncLParen;
		private readonly RegularExpression _puncRParen;
		private readonly RegularExpression _opPlus;
		private readonly RegularExpression _opMinus;
		private readonly RegularExpression _opMultiply;
		private readonly RegularExpression _opDivide;
		private readonly RegularExpression _opMod;
		private readonly RegularExpression _keywordLet;

		public string Source
		{
			get;
			private set;
		}

		public Lexer(string source)
		{
			Source = source;
			_automata = Enumerable.Empty<RegularExpression>();

			bool ignoreCase = false;
			_comLine = new RegularExpression(@"//.*", ignoreCase, TokenType.LINECOMMENT);
			_comBlock = new RegularExpression(@"/\*([^*]|\*[^/])*\*/", ignoreCase, TokenType.BLOCKCOMMENT);
			_whiteSpaces = new RegularExpression(@"\s+", ignoreCase, TokenType.WHITESPACES);
			_literalInt = new RegularExpression(@"(\+|\-)?((1|2|3|4|5|6|7|8|9)\d*|0)", ignoreCase, TokenType.LITERAL_INTEGER);
			_literalReal = new RegularExpression(@"(\-|\+)?(\d+(\.\d*)?|\.\d+)((e|E)(\-|\+)?\d+)?", ignoreCase, TokenType.LITERAL_REAL);
			_identifier = new RegularExpression(@"(a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|_)\w*", ignoreCase, TokenType.IDENTIFIER);
			_puncSemicolon = new RegularExpression(@";", ignoreCase, TokenType.PUNC_SEMICOLON);
			_puncEqual = new RegularExpression(@"=", ignoreCase, TokenType.PUNC_EQUAL);
			_puncLParen = new RegularExpression(@"\(", ignoreCase, TokenType.PUNC_LPAREN);
			_puncRParen = new RegularExpression(@"\)", ignoreCase, TokenType.PUNC_RPAREN);
			_opPlus = new RegularExpression(@"+", ignoreCase, TokenType.OP_PLUS);
			_opMinus = new RegularExpression(@"-", ignoreCase, TokenType.OP_MINUS);
			_opMultiply = new RegularExpression(@"*", ignoreCase, TokenType.OP_MULTIPLY);
			_opDivide = new RegularExpression(@"/", ignoreCase, TokenType.OP_DIVIDE);
			_opMod = new RegularExpression(@"%", ignoreCase, TokenType.OP_MOD);
			_keywordLet = new RegularExpression(@"let", ignoreCase, TokenType.KEYWORD_LET);
			SetupAutomata();
		}

		#region automata

		private void SetupAutomata()
		{
			_automata = Enumerable.Empty<RegularExpression>();

			_automata = _automata.Append(_comLine);
			_automata = _automata.Append(_comBlock);
			_automata = _automata.Append(_whiteSpaces);

			_automata = _automata.Append(_keywordLet);

			_automata = _automata.Append(_puncSemicolon);
			_automata = _automata.Append(_puncEqual);
			_automata = _automata.Append(_puncLParen);
			_automata = _automata.Append(_puncRParen);
			_automata = _automata.Append(_opPlus);
			_automata = _automata.Append(_opMinus);
			_automata = _automata.Append(_opMultiply);
			_automata = _automata.Append(_opDivide);
			_automata = _automata.Append(_opMod);

			_automata = _automata.Append(_literalInt);
			_automata = _automata.Append(_literalReal);
			_automata = _automata.Append(_identifier);

			_automata.ForEach(c => c.Reset());
		}

		private void MoveNextAutomata(char ch)
		{
			_automata.ForEach(a => a.MoveNext(ch));
			_automata = _automata.Where(a => !a.IsError());
		}

		private int CountNextAlive(char ch)
		{
			return _automata.Count(a => !a.IsNextError(ch));
		}

		#endregion

		private Token? GetToken(string str)
		{
			var a = _automata.Where(a => a.IsAcceptable()).FirstOrDefault();
			if (a == null)
			{
				return null;
			}
			return GetToken(a, str);
		}

		private static Token GetToken(RegularExpression regex, string strToken)
		{
			TokenType type = regex.Type;
			string src = strToken;
			return new Token(type, src);
		}

		public IEnumerable<Token> EnumerateTokens()
		{
			int position = -1;
			StringBuilder sb = new();
			Func<IEnumerable<char>, int, IEnumerable<Tuple<char, char?[]>>> EnumerateLookingAhead =
				Extensions.Generic.ValueType.EnumerableExtension.EnumerateLookingAhead;

			foreach (var item in EnumerateLookingAhead(Source, 1))
			{
				position++;
				char current = item.Item1;
				char? next = item.Item2[0];

				MoveNextAutomata(current);
				sb.Append(current);

				int c1al = _automata.Count();
				int c1ac = _automata.Count(a => a.IsAcceptable());

				if (c1al == 0)
				{
					string errorMsg = string.Format("invalid token '{0}' detected", sb.ToString());
					throw new Exception(errorMsg);
				}

				if (next.HasValue)
				{
					int c2al = CountNextAlive(next.Value);
					if (c2al == 0)
					{
						if (c1ac > 0)
						{
							var token1 = GetToken(sb.ToString());
							if (token1 != null
								&& !token1.Is(TokenType.LINECOMMENT)
								&& !token1.Is(TokenType.BLOCKCOMMENT)
								&& !token1.Is(TokenType.WHITESPACES))
							{
								yield return token1;
							}

							SetupAutomata();
							sb.Clear();
						}
						else
						{
							// just before error
						}
					}
				}
			}

			var token2 = GetToken(sb.ToString());
			if (token2 != null
				&& !token2.Is(TokenType.LINECOMMENT)
				&& !token2.Is(TokenType.BLOCKCOMMENT)
				&& !token2.Is(TokenType.WHITESPACES))
			{
				yield return token2;
			}

			yield break;
		}

		#region IEnumerable

		public IEnumerator<Tuple<Token, Token?[]>> GetEnumerator()
		{
			Func<IEnumerable<Token>, int, IEnumerable<Tuple<Token, Token?[]>>> func = Extensions.Generic.ReferenceType.EnumerableExtension.EnumerateLookingAhead;
			return func(EnumerateTokens(), 3).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
