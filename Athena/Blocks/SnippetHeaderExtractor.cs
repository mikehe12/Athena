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

		public (bool, ReadOnlySequence<byte>, ReadOnlySequence<byte>) TryGetSnippetHeader(in ReadOnlySequence<byte> text)
		{
			return text.FindValue(tagDelimiter) switch
			{
				(false, _, _) => default,
				(true, _, var remaining) => 
					remaining.FindValue(tagDelimiter) switch
					{
						(false, _, _) => default,
						(true, var end, var afterTag) => (true, remaining.Slice(0, end), afterTag)
					}
			};
		}
	}
}
