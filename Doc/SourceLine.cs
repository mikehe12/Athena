using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public readonly struct SourceLine
	{
		public SourceLine(int lineNumber, ReadOnlySequence<byte> line)
		{
			LineNumber = lineNumber;
			Bytes = line;
		}

		public int LineNumber { get; }
		public ReadOnlySequence<byte> Bytes { get; }
	}
}
