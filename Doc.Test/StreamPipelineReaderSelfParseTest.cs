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
		public async Task CanParseFileLines()
		{
			List<string> lines = new();

			using var fileStream = new FileStream(filename, FileMode.Open);
			var reader = PipeReader.Create(fileStream);

			// Delimit lines
			var delimiter = new LineDelimiter();

			var consumer = new SingleParsedConsumer<ReadOnlySequence<byte>>(delimiter,
				s => lines.Add(Encoding.UTF8.GetString(s)));
			var pipelineReader = new StreamPipelineReader(reader, consumer);

			await pipelineReader.Read();

			var expectedLineCount = (await File.ReadAllLinesAsync(filename)).Length;
			Assert.Equal(expectedLineCount, lines.Count);
		}

		[Fact]
		public async Task GivenSections_CanExtract()
		{
			List<string> sections = new();

			using var fileStream = new FileStream(filename, FileMode.Open);
			var reader = PipeReader.Create(fileStream);

			// Search for sections
			var sectioner = new SectionExtractor();

			var consumer = new SingleParsedConsumer<ReadOnlySequence<byte>>(sectioner,
				s => sections.Add(Encoding.UTF8.GetString(s)));
			var pipelineReader = new StreamPipelineReader(reader, consumer);

			await pipelineReader.Read();

			Assert.Single(sections);

			
		}
	}
}