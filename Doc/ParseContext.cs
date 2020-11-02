using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	public enum ParseState
	{
		Skip,
		Comment,
	}

	public readonly struct ParseContext
	{
		public ParseContext(int lineNumber, ParseState state)
		{
			LineNumber = lineNumber;
			State = state;
		}

		public int LineNumber { get; }
		public ParseState State { get; }

		public ParseContext WithState(ParseState state)
		{
			return new ParseContext(LineNumber, state);
		}
	}
}
