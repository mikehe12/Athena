using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public interface IBufferParser<TOut>
	{
		SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out TOut result);
	}
}
