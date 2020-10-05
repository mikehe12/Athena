using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public sealed class SectionExtractor : IBufferParser<ReadOnlySequence<byte>>
	{
		const byte sectionDelimiter = (byte)'#';
		private readonly string commentStarter = "//";

		public SectionExtractor(string commentStarter = "\\")
		{
			this.commentStarter = commentStarter;
		}

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> result)
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

			result = buffer.Slice(sectionStart.Value, buffer.GetPosition(1, sectionEnd.Value));
			return sectionEnd;
		}
	}
}
