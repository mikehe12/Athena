using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Blocks
{
	/// <summary>
	/// Determines when a Snippet begins. Snippet indicators are assumed
	/// to be within a comment.
	/// </summary>
	public sealed class SnippetHeaderExtractor
	{
		readonly byte tagDelimiter = (byte)'|';

		public (bool, ReadOnlySequence<byte>, SequencePosition) TryGetSnippetHeader(in ReadOnlySequence<byte> text)
		{
			return text.FindValue(tagDelimiter) switch
			{
				(false, _, _) => default,
				(true, var firstDelimiterPos, var remaining) => 
					remaining.FindValue(tagDelimiter) switch
					{
						(false, _, _) => default,
						(true, var end, _) => (true, remaining.Slice(0, end),
						text.GetPosition(text.GetOffset(firstDelimiterPos) + remaining.GetOffset(end)))
					}
			};
		}
	}
}
