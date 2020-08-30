using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	// $

	public sealed class StreamPipelineReader
	{
		readonly PipeReader reader;
		readonly IBufferConsumer consumer;

		public StreamPipelineReader(PipeReader pipeReader, IBufferConsumer delimiter)
		{
			reader = pipeReader;
			this.consumer = delimiter;
		}

		public async Task Read()
		{
			// Start read
			while (true)
			{
				// Read a block
				var result = await reader.ReadAsync();
				var buffer = result.Buffer;

				// Try to consume the buffer
				var consumedPosition = consumer.TryConsume(ref buffer);

				// Tell the PipeReader how much of the buffer has been consumed.
				reader.AdvanceTo(consumedPosition, buffer.End);

				// Stop reading if there's no more data coming.
				if (result.IsCompleted)
				{
					break;
				}
			}

			await reader.CompleteAsync();
		}

		bool SeekDelimiter(IBufferDelimiter delimiter, ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
		{
			// Look for a EOL in the buffer.
			SequencePosition? position = delimiter.Delimit(ref buffer);

			if (position == null)
			{
				line = default;
				return false;
			}

			// Skip the line + the \n.
			line = buffer.Slice(0, position.Value);
			buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
			return true;
		}
	}
}
