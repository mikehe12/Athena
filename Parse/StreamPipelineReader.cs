using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parse
{
	public sealed class StreamPipelineReader
	{
		readonly PipeReader reader;

		public StreamPipelineReader(PipeReader pipeReader)
		{
			reader = pipeReader;
		}

		public async Task Read()
		{

			while (true)
			{
				var result = await reader.ReadAsync();
				var buffer = result.Buffer;

				while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
				{
					// Process the line.
					ProcessLine(line);
				}

				// Tell the PipeReader how much of the buffer has been consumed.
				reader.AdvanceTo(buffer.Start, buffer.End);

				// Stop reading if there's no more data coming.
				if (result.IsCompleted)
				{
					break;
				}
			}
		}

		bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
		{
			// Look for a EOL in the buffer.
			SequencePosition? position = buffer.PositionOf((byte)'$');

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

		void ProcessLine(in ReadOnlySequence<byte> buffer)
		{
			foreach (var segment in buffer)
			{
				Console.Write(Encoding.UTF8.GetString(segment.Span));
			}
			Console.WriteLine();
		}
	}
}
