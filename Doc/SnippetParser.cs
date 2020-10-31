using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Doc
{
	class SnippetParser : IBufferParser<Snippet>
	{
		const byte tagDelimiter = (byte)'#';

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out Snippet result)
		{
			//var snipBuilder = new SnippetBuilder();

			//var tagStart = buffer.PositionOf(tagDelimiter);	
			result = default;
			return default;
		}
	}
}
