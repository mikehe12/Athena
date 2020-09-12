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
	public class LineDelimiterTests
	{
		readonly ITestOutputHelper output;

		public LineDelimiterTests(ITestOutputHelper output)
		{
			this.output = output;
		}


		[Theory]
		[InlineData(256)]
		[InlineData(1024)]
		public void GivenLineSequence_ShouldSplitLines(int lineCount)
		{
			const int maxLineLength = 10;
			var rand = new Random();

			// Initialize data
			var data = string.Concat(
				Enumerable.Range(1, lineCount).Select(
					_ => new string('x', rand.Next(maxLineLength)) + '\n'));
			var bytes = Encoding.UTF8.GetBytes(data);
			var buffer = new ReadOnlySequence<byte>(bytes);

			// Initialize delimiter
			LineDelimiter delimiter = new();
			List<ReadOnlySequence<byte>> lines = new();

			SequencePosition parsed = buffer.Start;
			while (delimiter.TryParse(buffer.Slice(parsed), out ReadOnlySequence<byte> result) is SequencePosition parsedSection)
			{
				lines.Add(result);

				// Advance past the parsed section
				parsed = buffer.GetPosition(1, parsedSection);
			}

			Assert.Equal(lineCount, lines.Count);
			Assert.All(lines, l => Assert.True(l.ToArray().Last() == (byte)'\n'));
		}


		[Theory]
		[InlineData(256)]
		[InlineData(1024)]
		public void GivenCrlfSequence_ShouldSplitLines(int lineCount)
		{
			const int maxLineLength = 10;
			var rand = new Random();

			// Initialize data
			var data = string.Concat(
				Enumerable.Range(1, lineCount).Select(
					_ => new string('x', rand.Next(maxLineLength)) + "\r\n"));
			var bytes = Encoding.UTF8.GetBytes(data);
			var buffer = new ReadOnlySequence<byte>(bytes);

			// Initialize delimiter
			LineDelimiter delimiter = new();
			List<ReadOnlySequence<byte>> lines = new();

			SequencePosition parsed = buffer.Start;
			while (delimiter.TryParse(buffer.Slice(parsed), out ReadOnlySequence<byte> result) is SequencePosition parsedSection)
			{
				lines.Add(result);

				// Advance past the parsed section
				parsed = buffer.GetPosition(1, parsedSection);
			}

			Assert.Equal(lineCount, lines.Count);
			Assert.All(lines, l => Assert.True(l.ToArray().Last() == (byte)'\n'));
		}
	}
}
