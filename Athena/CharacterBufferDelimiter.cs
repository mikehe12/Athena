using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	public sealed class CharacterBufferDelimiter : IBufferParser<string>
	{
		readonly byte delimiter;

		public CharacterBufferDelimiter(char delimiter)
		{
			this.delimiter = (byte)delimiter;
		}

		public SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer)
		{
			return buffer.PositionOf(delimiter) ?? buffer.Start;
		}

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out string result)
		{
			var eol = buffer.PositionOf(delimiter);

			if (eol == null)
			{
				result = string.Empty;
				return eol;
			}

			result = Encoding.UTF8.GetString(buffer.Slice(0, eol.Value));
			return eol;
		}
	}
}
