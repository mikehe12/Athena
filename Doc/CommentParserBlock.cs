using Doc.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	/// <summary>
	/// Finds comments in source code
	/// </summary>
	public sealed class CommentParserBlock
	{
		readonly byte[] CommentSequence = Encoding.UTF8.GetBytes("//");

		public (ParseContext, ReadOnlySequence<byte>) ParseLine(ParseContext state, ReadOnlySequence<byte> source)
		{
			var commentPosition = source.PositionOfSequence(CommentSequence);

			if (commentPosition == null) return (state.WithState(ParseState.Skip), source);


		}
	}
}
