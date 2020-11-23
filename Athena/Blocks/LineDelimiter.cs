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

		public (bool, ReadOnlySequence<byte>, SequencePosition) Parse(ReadOnlySequence<byte> input, ref LineReference context)
		{
			var (eolFound, eolPos, remaining) = input.FindValue(endOfLine);

			if (!eolFound)
			{
				return default;
			}

			var line = input.Slice(0, eolPos);

			context = new LineReference(context.FileName, context.LineNumber + 1);

			return (true, line, input.GetPosition(1, eolPos));
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
