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
		/// <summary>
		/// Parses a buffer to seek a pattern or section.
		/// </summary>
		/// <param name="buffer">The source buffer.</param>
		/// <param name="result">The object of interest within the buffer. This value is valid if the returned SequencePosition is not null.</param>
		/// <returns>A SequencePosition indicating how much of the buffer has been searched through and can be skipped. A null indicates an inconclusive result.</returns>
		SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out TOut result);
	}
}
