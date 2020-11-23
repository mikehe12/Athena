using Athena.DataTypes;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	public sealed class SingleParsedConsumer<TOut, TContext> : IBufferConsumer
	{
		readonly IBufferParser<TOut> parser;
		readonly IParserBlock<TOut, TContext> parserBlock;
		readonly Action<TOut> consumer;

		public SingleParsedConsumer(IBufferParser<TOut> parser, Action<TOut> consumer)
		{
			this.parser = parser;
			this.consumer = consumer;
		}


		public SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer, ref TContext context)
		{
			SequencePosition parsed = buffer.Start;

			while (parserBlock.Parse(buffer, ref context) is (true, var output, var readUntil))
			{
				consumer(output);

				// Advance past the parsed section
				parsed = buffer.GetPosition(1, readUntil);
			}

			return parsed;
		}

		public SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer)
		{
			SequencePosition parsed = buffer.Start;

			while (parser.TryParse(buffer.Slice(parsed), out TOut result) is SequencePosition parsedSection)
			{
				consumer(result);

				// Advance past the parsed section
				parsed = buffer.GetPosition(1, parsedSection);
			}

			return parsed;
		}
	}
}
