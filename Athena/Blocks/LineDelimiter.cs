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
		IParserBlock<ReadOnlySequence<byte>, LineReference>
	{
		const byte endOfLine = (byte)'\n';

		public (bool, SequencePosition, ReadOnlySequence<byte>) Parse(ReadOnlySequence<byte> input, ref LineReference context)
		{
			var (eolFound, eolPos, _) = input.FindValue(endOfLine);

			// If line is not found, indicate with a false returns.
			if (!eolFound)
			{
				return (false, input.Start, default);
			}

			// Update the line state by incrementing the line number, having found a line.
			context = new LineReference(context.FileName, context.LineNumber + 1);

			// Indicate that the buffer has been read until the end of the line.
			var readUntil = input.GetPosition(1, eolPos);

			// Slice the extents of the line to return.
			var line = input.Slice(0, eolPos);

			// Return a tuple to indicate that a line has been found.
			return (true, readUntil, line);
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
