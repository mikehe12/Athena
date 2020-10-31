using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public sealed class LineInterpreter
	{
		public enum LineType
		{
			Ignore,
			SectionTag,
			SectionHeader,
			Code,

		}

		ReadOnlySequence<byte> lines;

		public void FeedLine(ReadOnlySequence<byte> nextLine)
		{
			nextLine.Slice()
		}
	}
}
