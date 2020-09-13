using System;
using System.Collections.Generic;
using System.IO;
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

			var reader = file.OpenText();

		}

	}
}
