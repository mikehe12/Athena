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

		public Result<LineBuffer> Parse(FileBlockBuffer input, ref int lineNumber)
		{
			var eol = input.Buffer.PositionOf(endOfLine);

			if (eol == null)
			{
				return Result<LineBuffer>.Failure();
			}

			var line = input.Buffer.Slice(0, input.Buffer.GetPosition(1, eol.Value));

			lineNumber++;

			return Result<LineBuffer>.Success(
				new LineBuffer(input.FileName, lineNumber, line)
				);
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
