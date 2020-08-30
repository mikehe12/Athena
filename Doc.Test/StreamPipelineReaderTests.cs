using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Doc.Test
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

			var delimiter = new CharacterBufferDelimiter('\n');
			var consumer = new SingleParsedConsumer<string>(delimiter,
				s => output.WriteLine(s));

			var streamReader = new StreamPipelineReader(reader, consumer);

			await streamReader.Read();

			//ReadResult readResult = await reader.ReadAsync();
			//ReadOnlySequence<byte> buffer = readResult.Buffer;


		}

	}
}
