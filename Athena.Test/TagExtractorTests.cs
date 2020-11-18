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

	public class TagExtractorTests
	{
		readonly ITestOutputHelper output;
		TagExtractor tagExtractor;

		public TagExtractorTests(ITestOutputHelper output)
		{
			this.output = output;
			tagExtractor = new TagExtractor();
		}

		[Theory]
		[InlineData("")]
		public void GivenNoTags_ReturnsFalse(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var (tagFound, tags) = tagExtractor.TryGetTags(sequence);

			Assert.False(tagFound);
			Assert.Equal(default, tags);
		}


		[Theory]
		[InlineData("abcd.efjk.lmno")]
		[InlineData(".abcd.efjk.lmno")]
		[InlineData(". abcd. efjk.lmno")]
		[InlineData("..abcd.efjk.	lmno	.")]
		public void GivenTags_FindsTags(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var (tagFound, tags) = tagExtractor.TryGetTags(sequence);

			Assert.True(tagFound);
			var expectedTags = new string[]{ "abcd", "efjk", "lmno" };
			Assert.Equal(expectedTags, tags);
		}

	}
}
