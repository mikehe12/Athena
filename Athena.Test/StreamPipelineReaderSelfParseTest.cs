using Athena.DataTypes;
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
	public class StreamPipelineReaderSelfParseTest : IDisposable
	{
		readonly static string filename = Path.Combine(TestSamples.Folder, "StreamPipelineReader.cs");
		readonly ITestOutputHelper output;
		readonly FileStream fileStream;
		readonly PipeReader reader;


		public StreamPipelineReaderSelfParseTest(ITestOutputHelper output)
		{
			this.output = output;

			// Load file to parse
			this.fileStream = new FileStream(filename, FileMode.Open);
			this.reader = PipeReader.Create(fileStream);
		}

		public void Dispose()
		{
			fileStream.Dispose();
			reader.Complete();
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

			// Delimit lines
			var delimiter = new LineDelimiter();

			var consumer = new SingleParsedConsumer<ReadOnlySequence<byte>, LineReference>(delimiter,
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

			// Search for sections
			var sectioner = new SectionExtractor();

			var consumer = new SingleParsedConsumer<ReadOnlySequence<byte>, LineReference>(sectioner,
				s => sections.Add(Encoding.UTF8.GetString(s)));
			var pipelineReader = new StreamPipelineReader(reader, consumer);

			await pipelineReader.Read();

			Assert.Single(sections);
		}
	}
}