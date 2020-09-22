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

namespace Doc.Test
{
	public class StreamPipelineReaderSelfParseTest
	{
		readonly static string filename = Path.Combine(TestSamples.Folder, "StreamPipelineReader.cs");

		readonly ITestOutputHelper output;

		public StreamPipelineReaderSelfParseTest(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void CanFindCorrectFile()
		{
			var file = new FileInfo(filename);

			Assert.True(file.Exists);
		}

		[Fact]
		public void CanParseFileLines()
		{
			const int MinExpectedLines = 20;

			List<string> lines = new();

			using (var fileStream = new FileStream(filename, FileMode.Open))
			{
				var reader = PipeReader.Create(fileStream);

				var consumer = new SingleParsedConsumer<ReadOnlySequence<byte>>(new LineDelimiter(), s => lines.Add(Encoding.UTF8.GetString(s)));
				var pipelineReader = new StreamPipelineReader(reader, consumer);
			}
		}
	}
}