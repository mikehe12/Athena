using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.DataTypes
{
	public readonly struct LineBuffer
	{
		public LineBuffer(string fileName, int lineNumber, ReadOnlySequence<byte> buffer)
		{
			FileName = fileName;
			LineNumber = lineNumber;
			Buffer = buffer;
		}

		public string FileName { get; }
		public int LineNumber { get; }
		public ReadOnlySequence<byte> Buffer { get; }
	}
}
