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

	public class CommentExtractorTests
	{
		readonly ITestOutputHelper output;
		CommentExtractor commentExtractor;
		CommentState state;

		public CommentExtractorTests(ITestOutputHelper output)
		{
			this.output = output;
			commentExtractor = new CommentExtractor();
			state = new CommentState(1, false);
		}

		[Fact]
		public void GivenNoComment_ReturnsFalse()
		{
			const string TestLine = "abcd efjk lmno";
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(TestLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			Assert.False(commentFound);
			Assert.Equal(default(ReadOnlySequence<byte>), comment);
		}


		[Theory]
		[InlineData("// abcd efjk lmno")]
		[InlineData("  // abcd efjk lmno")]
		[InlineData("	// abcd efjk lmno")]
		[InlineData("//		abcd efjk lmno")]
		[InlineData("// abcd ef//jk lmno")]
		[InlineData("// abcd ef//jk lmno//")]
		public void GivenLineComment_FindsComment(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			Assert.True(commentFound);
			var commentText = testLine.Substring(testLine.IndexOf("//") + 2);
			Assert.Equal(Encoding.UTF8.GetBytes(commentText), comment.ToArray());
		}


		[Theory]
		[InlineData("/* abcd efjk lmno")]
		[InlineData("  /* abcd efjk lmno")]
		[InlineData("	/* abcd efjk lmno")]
		[InlineData("/*		abcd efjk lmno")]
		public void GivenBlockCommentStart_FindsBlockComment(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			Assert.True(commentFound);
			var commentText = testLine.Substring(testLine.IndexOf("/*") + 2);
			Assert.Equal(Encoding.UTF8.GetBytes(commentText), comment.ToArray());

			// Should indicate within a block comment
			Assert.True(state.InBlockComment);
		}

		[Theory]
		[InlineData("/* abcd efjk lmno*/")]
		[InlineData(" /* abcd efjk lmno*/ ")]
		[InlineData("	/* abcd efjk lmno*/		")]
		[InlineData("	/* abcd //efjk lmno*/		")]
		public void GivenSingleLineBlockComment_FindsBlockComment(string testLine)
		{
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			// Asserts
			Assert.True(commentFound);
			var startIndex = testLine.IndexOf("/*") + 2;
			var endIndex = testLine.IndexOf("*/");
			var commentText = testLine.Substring(startIndex, endIndex - startIndex);
			Assert.Equal(Encoding.UTF8.GetBytes(commentText), comment.ToArray());

			// Should indicate not in a block comment (completed)
			Assert.False(state.InBlockComment);
		}



		[Theory]
		[InlineData("* abcd efjk lmno")]
		[InlineData("	* abcd efjk lmno")]
		[InlineData("*abcd //efjk lmno	")]
		public void GivenWithinBlockComment_FindsLineAfterPrefix(string testLine)
		{
			state = new CommentState(1, true);
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			// Asserts
			Assert.True(commentFound);
			var startIndex = testLine.IndexOf("*") + 1;
			var commentText = testLine.Substring(startIndex);
			Assert.Equal(Encoding.UTF8.GetBytes(commentText), comment.ToArray());

			// Should indicate still in a block comment
			Assert.True(state.InBlockComment);
		}


		[Theory]
		[InlineData("* abcd efjk lmno*/")]
		[InlineData("	* abcd efjk lmno */")]
		[InlineData("*abcd //efjk lmno		*/  asd")]
		public void GivenWithinBlockCommentWithEnd_FindsLineAfterPrefixAndEnd(string testLine)
		{
			state = new CommentState(1, true);
			var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(testLine));

			var commentFound = commentExtractor.TryGetLineComment(sequence, ref state, out ReadOnlySequence<byte> comment);

			// Asserts
			Assert.True(commentFound);
			var startIndex = testLine.IndexOf("*") + 1;
			var endIndex = testLine.IndexOf("*/");
			var commentText = testLine.Substring(startIndex, endIndex - startIndex);
			Assert.Equal(Encoding.UTF8.GetBytes(commentText), comment.ToArray());

			// Should indicate no longer in a block comment
			Assert.False(state.InBlockComment);
		}
	}
}
