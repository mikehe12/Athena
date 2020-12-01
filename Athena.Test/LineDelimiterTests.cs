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
	public class LineDelimiterTests
	{
		readonly ITestOutputHelper output;
		readonly LineDelimiter delimiter;

		public LineDelimiterTests(ITestOutputHelper output)
		{
			delimiter = new();
			this.output = output;
		}

		[Theory]
		[InlineData("")]
		[InlineData("test string")]
		[InlineData("	Hello World \r")]
		public void GivenNoEndOfLine_ShouldReturnFalse(string s)
		{
			// Retrieve buffer of bytes
			var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(s));

			// Initialize line number context
			var lineNumber = new LineReference(string.Empty, 0);

			// Parse line
			var (lineFound, readCursor, line) = delimiter.Parse(buffer, ref lineNumber);

			Assert.False(lineFound);
			Assert.Equal(buffer.Start, readCursor);
			Assert.Equal(default, line);
		}


		[Theory]
		[InlineData("\n")]
		[InlineData("test string\n")]
		[InlineData("	Hello World \r\n")]
		[InlineData("Hello World \r\n next line")]
		[InlineData("Hello World \n next line")]
		public void GivenSingleEndOfLine_ShouldFindLine(string s)
		{
			// Retrieve buffer of bytes
			var buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(s));

			// Initialize line number context
			var lineNumber = new LineReference(string.Empty, 0);

			// Parse line
			var (lineFound, readCursor, line) = delimiter.Parse(buffer, ref lineNumber);

			// Calculate expected values
			var (_, eolPos, _) = buffer.FindValue((byte)'\n');
			var afterLinePos = buffer.GetPosition(1, eolPos); 

			Assert.True(lineFound);
			Assert.Equal(afterLinePos, readCursor);
			Assert.Equal(buffer.Slice(0, afterLinePos), line);
			Assert.Equal(1, lineNumber.LineNumber);
		}




		[Theory]
		[InlineData(10)]
		[InlineData(256)]
		[InlineData(1024)]
		public void GivenLineSequence_ShouldSplitLines(int lineCount)
		{
			const int maxLineLength = 10;
			var rand = new Random();

			// Initialize data
			var rawLines = Enumerable.Range(0, lineCount)
				.Select(_ => StringHelpers.GetRandomAlphanumericString(rand.Next(maxLineLength)))
				.ToArray();
			
			var completeString = string.Join('\n', rawLines) + "\n";
			var bytes = Encoding.UTF8.GetBytes(completeString);
			var buffer = new ReadOnlySequence<byte>(bytes);

			// Initialize delimiter
			List<ReadOnlySequence<byte>> lines = new();

			// Initialize read pointer
			SequencePosition parsedToPos = buffer.Start;

			// Initialize line counter
			LineReference lineReference = new(string.Empty, 0);

			while (delimiter.Parse(buffer.Slice(parsedToPos), ref lineReference) is (true, var eol, var line))
			{
				lines.Add(line);

				// Advance past the parsed section
				parsedToPos = eol;
			}

			// Calculate expected data
			var expectedLines = rawLines.Select(s => s + '\n')
				.Select(s => Encoding.UTF8.GetBytes(s))
				.ToArray();

			Assert.Equal(lineCount, lines.Count);
			Assert.Equal(expectedLines, lines.Select(x => x.ToArray()));
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
