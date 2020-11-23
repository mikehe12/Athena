using Athena.Blocks;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Athena.Test
{

	public class SnippetHeaderExtractorTests
	{
		readonly ITestOutputHelper output;
		SnippetHeaderExtractor snippetExtractor;

		public SnippetHeaderExtractorTests(ITestOutputHelper output)
		{
			this.output = output;
			snippetExtractor = new SnippetHeaderExtractor();
		}

		[Theory]
		[InlineData("")]
		[InlineData("abcd efjk lmno")]
		public void GivenNoSnippet_ReturnsFalse(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var (headerFound, header, readPosition) = snippetExtractor.TryGetSnippetHeader(sequence);

			Assert.False(headerFound);
			Assert.Equal(default, header);
			Assert.Equal(default, readPosition);
		}


		[Theory]
		[InlineData("|abcd efjk lmno|")]
		[InlineData(" |abcd efjk lmno| ")]
		[InlineData(" | abcd efjk lmno | ")]
		[InlineData("	| abcd efjk lmno |	")]
		[InlineData("	| abcd efjk lmno | |")]
		public void GivenSnippetHeader_FindsSnippet(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));
			
			// Expected
			var headerStart = testLine.IndexOf("|") + 1;
			var headerEnd = testLine.Substring(headerStart).IndexOf("|");
			var headerText = testLine.Substring(headerStart, headerEnd);
			var expected = Encoding.UTF8.GetBytes(headerText);

			var (headerFound, header, _) = snippetExtractor.TryGetSnippetHeader(sequence);

			Assert.True(headerFound);
			Assert.Equal(expected, header.ToArray());
		}


		[Theory]
		[InlineData("|abcd efjk lmno")]
		[InlineData(" |abcd efjk lmno ")]
		[InlineData(" | abcd efjk lmno  ")]
		[InlineData("abcd efjk lmno|")]
		[InlineData(" abcd efjk lmno |")]
		[InlineData("	abcd efjk lmno	|	")]
		public void GivenSingleDelimiter_FindsNoSnippet(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var (headerFound, header, readPosition) = snippetExtractor.TryGetSnippetHeader(sequence);

			Assert.False(headerFound);
			Assert.Equal(default, header);
			Assert.Equal(default, readPosition);
		}
	}
}
