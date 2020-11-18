using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Blocks
{
	public sealed class CommentExtractor
	{
		readonly byte[] lineCommentStartBytes;
		readonly byte[] blockCommentStartBytes;
		readonly byte[] blockCommentPrefixBytes;
		readonly byte[] blockCommentEndBytes;

		public CommentExtractor()
		{
			lineCommentStartBytes = Encoding.UTF8.GetBytes("//");
			blockCommentStartBytes = Encoding.UTF8.GetBytes("/*");
			blockCommentPrefixBytes = Encoding.UTF8.GetBytes("*");
			blockCommentEndBytes = Encoding.UTF8.GetBytes("*/");
		}

		public bool TryGetLineComment(ReadOnlySequence<byte> line, ref CommentState state, out ReadOnlySequence<byte> commentText)
		{
			SequencePosition commentStart = line.Start;
			SequencePosition commentEnd = line.End;
			var focus = line;

			if (state.InBlockComment)
			{
				// Find if block comment is ending
				var (blockCommentEnding, endCommentPosition, _) = focus.FindSequence(blockCommentEndBytes);

				if (blockCommentEnding)
				{
					commentEnd = focus.GetPosition(0, endCommentPosition);
					// Slice out the end chars and after
					focus = focus.Slice(0, commentEnd);

					// Set state to indicate end of block comment
					state = new CommentState(state.LineNumber, false);
				}

				// Find index to trim comment prefix
				var (prefixFound, prefixPosition, remaining) = focus.FindSequence(blockCommentPrefixBytes);
				if (prefixFound)
				{
					commentStart = focus.GetPosition(blockCommentPrefixBytes.Length, prefixPosition);
				}
			}
			else
			{
				// Check for block comment
				if (focus.PositionOfSequence(blockCommentStartBytes) is SequencePosition startCommentPosition)
				{
					commentStart = focus.GetPosition(blockCommentStartBytes.Length, startCommentPosition);

					// Check for block comment end within same line:
					var commentSlice = focus.Slice(startCommentPosition);
					if (commentSlice.PositionOfSequence(blockCommentEndBytes) is SequencePosition blockCommentEnd)
					{
						commentEnd = blockCommentEnd;
					}
					else
					{
						state = new CommentState(state.LineNumber, true);
					}
				}
				// Check for line comment
				else if (focus.PositionOfSequence(lineCommentStartBytes) is SequencePosition startLineCommentPosition)
				{
					commentStart = focus.GetPosition(blockCommentStartBytes.Length, startLineCommentPosition);
				}
				// No comment found
				else
				{
					commentText = default;
					return false;
				}
			}

			commentText = focus.Slice(commentStart, commentEnd);
			return true;
		}
	}

	public readonly struct CommentState
	{
		public CommentState(int lineNumber, bool inBlockComment)
		{
			LineNumber = lineNumber;
			InBlockComment = inBlockComment;
		}

		public int LineNumber { get; }
		public bool InBlockComment { get; }
	}
}
