using Athena.DataTypes;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	public sealed class LineDelimiter : IBufferParser<ReadOnlySequence<byte>>,
		IParserBlock<FileBlockBuffer, LineBuffer, Unit>
	{
		const byte endOfLine = (byte)'\n';

		public LineBuffer Parse(FileBlockBuffer input, Unit context)
		{
			throw new NotImplementedException();
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
