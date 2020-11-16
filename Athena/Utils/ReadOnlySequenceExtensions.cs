using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	/*
     * 
     * 
     * 
     */
	public static class ReadOnlySequenceExtensions
	{
		public static (bool, SequencePosition, ReadOnlySequence<T>) FindValue<T>(in this ReadOnlySequence<T> source, T searchValue) where T : IEquatable<T>
		{
			if (source.IsSingleSegment)
			{
				int index = source.First.Span.IndexOf(searchValue);
				if (index != -1)
				{
					var pos = source.GetPosition(index);
					return (true, pos, source.Slice(source.GetPosition(index + 1)));
				}
			}
			else
			{
				return FindSequenceInMultiSegment(source, searchValue);
			}

			return (false, default, default);
		}

		public static (bool, SequencePosition, ReadOnlySequence<T>) FindSequenceInMultiSegment<T>(in ReadOnlySequence<T> source, T searchValue) where T : IEquatable<T>
		{
			SequencePosition position = source.Start;
			SequencePosition result = position;
			while (source.TryGet(ref position, out ReadOnlyMemory<T> memory))
			{
				int index = memory.Span.IndexOf(searchValue);
				if (index != -1)
				{
					var pos = source.GetPosition(index, result);
					return (true, pos, source.Slice(source.GetPosition(index + 1)));
				}
				else if (position.GetObject() == null)
				{
					break;
				}

				result = position;
			}

			return (false, default, default);
		}

		public static (bool, SequencePosition, ReadOnlySequence<T>) FindSequence<T>(in this ReadOnlySequence<T> source, ReadOnlySpan<T> searchSequence) where T : IEquatable<T>
		{
			if (source.IsSingleSegment)
			{
				int index = source.First.Span.IndexOf(searchSequence);
				if (index != -1)
				{
					var pos = source.GetPosition(index);
					return (true, pos, source.Slice(source.GetPosition(index + searchSequence.Length)));
				}
			}
			else
			{
				return FindSequenceInMultiSegment(source, searchSequence);
			}

			return (false, default, default);
		}

		public static (bool, SequencePosition, ReadOnlySequence<T>) FindSequenceInMultiSegment<T>(in ReadOnlySequence<T> source, ReadOnlySpan<T> searchSequence) where T : IEquatable<T>
		{
			SequencePosition position = source.Start;
			SequencePosition result = position;
			while (source.TryGet(ref position, out ReadOnlyMemory<T> memory))
			{
				int index = memory.Span.IndexOf(searchSequence);
				if (index != -1)
				{
					var pos = source.GetPosition(index, result);
					return (true, pos, source.Slice(source.GetPosition(index + searchSequence.Length)));
				}
				else if (position.GetObject() == null)
				{
					break;
				}

				result = position;
			}

			return (false, default, default);
		}

		public static SequencePosition? PositionOfSequence<T>(in this ReadOnlySequence<T> source, ReadOnlySpan<T> searchSequence) where T : IEquatable<T>
		{
			if (source.IsSingleSegment)
			{
				int index = source.First.Span.IndexOf(searchSequence);
				if (index != -1)
				{
					return source.GetPosition(index);
				}

				return null;
			}
			else
			{
				return PositionOfAnyMultiSegment(source, searchSequence);
			}
		}

		private static SequencePosition? PositionOfAnyMultiSegment<T>(in ReadOnlySequence<T> source, ReadOnlySpan<T> searchSequence) where T : IEquatable<T>
		{
			SequencePosition position = source.Start;
			SequencePosition result = position;
			while (source.TryGet(ref position, out ReadOnlyMemory<T> memory))
			{
				int index = memory.Span.IndexOf(searchSequence);
				if (index != -1)
				{
					return source.GetPosition(index, result);
				}
				else if (position.GetObject() == null)
				{
					break;
				}

				result = position;
			}

			return null;
		}

	}
}
