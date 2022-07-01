namespace Calculator
{
	internal class Token
	{
		public TokenType TokenType
		{
			get;
			private set;
		}

		public string Value
		{
			get;
			private set;
		}

		public bool IgnoreCase
		{
			get;
			private set;
		}

		public Token(TokenType tokenType, string value)
			: this(tokenType, value, false)
		{
		}

		public Token(TokenType tokenType, string value, bool ignoreCase)
		{
			TokenType = tokenType;
			Value = value;
			IgnoreCase = ignoreCase;
		}

		public override string ToString()
		{
			return string.Format("{0}«{1}»", Value, TokenType);
		}

		public bool Is(TokenType tokenType)
		{
			return TokenType.Equals(tokenType);
		}

		public bool Is(string strToken)
		{
			return Value.ToUpper() == strToken.ToUpper();
		}

		public bool IsComment()
		{
			return
				TokenType == TokenType.BLOCKCOMMENT
				|| TokenType == TokenType.LINECOMMENT;
		}
	}
}
