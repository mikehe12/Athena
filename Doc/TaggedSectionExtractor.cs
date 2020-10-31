using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public sealed class TaggedSectionExtractor : IBufferParser<TaggedSection>
	{
		const byte sectionDelimiter = (byte)'#';
		const byte endTagDelimiter = (byte)' ';
		private readonly string commentStarter = "//";


		public TaggedSectionExtractor(string commentStarter = "\\")
		{
			this.commentStarter = commentStarter;
		}

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out TaggedSection result)
		{
			var sectionStart = buffer.PositionOf(sectionDelimiter);

			if (sectionStart == null)
			{
				result = default;
				return null;
			}

			var remaining = buffer.Slice(buffer.GetPosition(1, sectionStart.Value));

			// Search for corresponding section end.
			var sectionEnd = remaining.PositionOf(sectionDelimiter);

			if (sectionEnd == null)
			{
				result = default;
				// Return section start to skip previous section
				return sectionStart;
			}

			var section = buffer.Slice(sectionStart.Value, buffer.GetPosition(1, sectionEnd.Value));

			var stringB = new StringBuilder();

			stringB.Append

			//buffer.FirstSpan.IndexOfAny()

			//var tagEnd = buffer.IndexOfAn(endTagDelimiter);
			//SpanHelpers.

			result = default;

			return null;
		}
	}
}
