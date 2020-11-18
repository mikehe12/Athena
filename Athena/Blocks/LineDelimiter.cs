using Athena.DataTypes;
using Athena.Primitives;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	public sealed class LineDelimiter : IBufferParser<ReadOnlySequence<byte>>,
		IParserBlock<FileBlockBuffer, LineBuffer, int>
	{
		const byte endOfLine = (byte)'\n';

		public (bool, LineBuffer) Parse(FileBlockBuffer input, ref int lineNumber)
		{
			var (eolFound, eolPos, remaining) = input.Buffer.FindValue(endOfLine);

			if (!eolFound)
			{
				return default;
			}

			var line = input.Buffer.Slice(0, eolPos);

			lineNumber++;

			return (true, new LineBuffer(input.FileName, lineNumber, line));
		}

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> result)
		{
			var eol = buffer.PositionOf(endOfLine);

			if (eol == null)
			{
				result = default;
				return eol;
			}

			result = buffer.Slice(0, buffer.GetPosition(1, eol.Value));
			return eol;
		}

	}
}
