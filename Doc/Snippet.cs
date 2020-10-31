using System.Buffers;

namespace Doc
{
	public readonly struct Snippet
	{
		public Snippet(ReadOnlySequence<byte> tag, ReadOnlySequence<byte> text)
		{
			Tag = tag;
			Text = text;
		}

		public ReadOnlySequence<byte> Tag { get; }
		public ReadOnlySequence<byte> Text { get; }

	}
}