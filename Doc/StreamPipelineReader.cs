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

			// Wait for finishing of processing before closing?
			await reader.CompleteAsync();
		}
	}
}
