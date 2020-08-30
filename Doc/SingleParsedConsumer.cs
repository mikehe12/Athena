using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	public sealed class SingleParsedConsumer<T> : IBufferConsumer
	{
		readonly IBufferParser<T> parser;
		readonly Action<T> consumer;

		public SingleParsedConsumer(IBufferParser<T> parser, Action<T> consumer)
		{
			this.parser = parser;
			this.consumer = consumer;
		}

		public SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer)
		{
			SequencePosition parsed = buffer.Start;

			while (parser.TryParse(buffer.Slice(parsed), out T result) is SequencePosition parsedSection)
			{
				consumer(result);

				// Advance past the parsed section
				parsed = buffer.GetPosition(1, parsedSection);
			}

			return parsed;
		}
	}
}
