using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Athena.Test
{
	public class StreamPipelineReaderTests
	{
		public static IEnumerable<object[]> TestData => new object[][]
		{
			new object[]{ new string('t', 100) + "\n" + new string('t', 100) + "\n" }
		};

		readonly ITestOutputHelper output;

		public StreamPipelineReaderTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Theory, MemberData(nameof(TestData))]
		public async Task TestNoEndline(string data)
		{
			var stream = new MemoryStream(Encoding.ASCII.GetBytes(data));
			var reader = PipeReader.Create(stream);

			var parsedStrings = new List<string>();

			var delimiter = new CharacterBufferDelimiter('\n');
			var consumer = new SingleParsedConsumer<string, UInt16>(delimiter,
				s => parsedStrings.Add(s));

			var streamReader = new StreamPipelineReader(reader, consumer);

			await streamReader.Read();

			Assert.Equal(2, parsedStrings.Count);
		}


		[Theory]
		[InlineData(1024)]
		[InlineData(8192)]
		[InlineData(65536)]
		public async Task GivenConsumer_ShouldPassAllData(int byteCount)
		{
			var data = new byte[byteCount];

			// Generate random data
			var rand = new Random();
			rand.NextBytes(data);

			// Initialize pipe
			var stream = new MemoryStream(data);
			var reader = PipeReader.Create(stream);

			List<byte[]> reads = new();
			var consumer = new MockBufferConsumer()
			{
				OnTryConsume = (x) => {
					reads.Add(x.ToArray());
					return x.End;
				}
			};

			var streamReader = new StreamPipelineReader(reader, consumer);

			// Execute read
			await streamReader.Read();

			// Check received data = input data
			var consumed = reads.SelectMany(arr => arr).ToArray();

			Assert.Equal(data.Length, consumed.Length);
			Assert.Equal(data, consumed);
		}

		[Theory]
		[InlineData(1024, 128)]
		[InlineData(8192, 128)]
		[InlineData(65536, 128)]
		public async Task GivenPartialConsumer_ShouldObserveAllData(int byteCount, int consumeCount)
		{
			var data = new byte[byteCount];

			// Generate random data
			var rand = new Random();
			rand.NextBytes(data);

			// Initialize pipe
			var stream = new MemoryStream(data);
			var reader = PipeReader.Create(stream);

			List<byte[]> reads = new();
			byte[] unread = default;
			var consumer = new MockBufferConsumer()
			{
				OnTryConsume = (x) => {
					var consumed = Math.Min(consumeCount, x.Length);
					reads.Add(x.Slice(0, consumed).ToArray());
					unread = x.Slice(consumed).ToArray();
					return x.GetPosition(consumed);
				}
			};

			var streamReader = new StreamPipelineReader(reader, consumer);

			// Execute read
			await streamReader.Read();

			// Check received data = input data
			var consumed = reads.SelectMany(arr => arr).Concat(unread.ToArray()).ToArray();

			Assert.Equal(data.Length, consumed.Length);
			Assert.Equal(data, consumed);
		}

		[Theory]
		[InlineData(10, 12)]
		[InlineData(1024, 128)]
		[InlineData(8192, 256)]
		public async Task GivenLineConsumer_ShouldProduceLines(int lineCount, int lineLength)
		{
			var data = string.Concat(
				Enumerable.Range(1, lineCount).Select(
					_ => new string('x', lineLength) + '\n'));

			// Initialize pipe
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
			var reader = PipeReader.Create(stream);

			List<byte[]> reads = new();
			byte eol = (byte)'\n';
			var consumer = new MockBufferConsumer()
			{
				OnTryConsume = (x) => {
					SequencePosition consumed = x.Start;
					while (x.PositionOf(eol) is SequencePosition lineEnd)
					{
						var end = x.GetPosition(1, lineEnd);
						reads.Add(x.Slice(0, end).ToArray());
						x = x.Slice(end);
						consumed = end;
					}
					return consumed;

				}
			};

			var streamReader = new StreamPipelineReader(reader, consumer);

			// Execute read
			await streamReader.Read();

			// Should produce expected number of lines.
			Assert.Equal(lineCount, reads.Count);
			// Check each line ends with \n?
		}


		[Theory]
		[InlineData(1024)]
		[InlineData(8192)]
		[InlineData(65536)]
		public async Task GivenNonConsumer_ShouldCompleteBuffer(int byteCount)
		{
			var data = new byte[byteCount];

			// Generate random data
			var rand = new Random();
			rand.NextBytes(data);

			// Initialize pipe
			var stream = new MemoryStream(data);
			var reader = PipeReader.Create(stream);

			List<long> readSize = new();
			var consumer = new MockBufferConsumer()
			{
				OnTryConsume = (x) => {
					readSize.Add(x.Length);
					return x.Start;
				}
			};

			var streamReader = new StreamPipelineReader(reader, consumer);

			// Execute read
			await streamReader.Read();

			// Check complete buffer is examined
			Assert.Equal(readSize.Last(), byteCount);
			// Check stream is closed
			Assert.False(stream.CanRead || stream.CanSeek || stream.CanWrite);
		}
	}
}
