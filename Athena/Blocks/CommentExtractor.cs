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
			if (state.InBlockComment)
			{
				// Find index to trim comment prefix
				if (line.PositionOfSequence(blockCommentPrefixBytes) is SequencePosition prefixPosition)
				{
					commentStart = line.GetPosition(blockCommentPrefixBytes.Length, prefixPosition);
				}

				// Find if block comment is ending
				if (line.PositionOfSequence(blockCommentEndBytes) is SequencePosition endCommentPosition)
				{
					commentEnd = line.GetPosition(0, endCommentPosition);

					// Set state to indicate end of block comment
					state = new CommentState(state.LineNumber, false);
				}
			}
			else
			{
				// Check for block comment
				if (line.PositionOfSequence(blockCommentStartBytes) is SequencePosition startCommentPosition)
				{
					commentStart = line.GetPosition(blockCommentStartBytes.Length, startCommentPosition);

					// Check for block comment end within same line:
					var commentSlice = line.Slice(startCommentPosition);
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
				else if (line.PositionOfSequence(lineCommentStartBytes) is SequencePosition startLineCommentPosition)
				{
					commentStart = line.GetPosition(blockCommentStartBytes.Length, startLineCommentPosition);
				}
				// No comment found
				else
				{
					commentText = default;
					return false;
				}
			}

			commentText = line.Slice(commentStart, commentEnd);
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
