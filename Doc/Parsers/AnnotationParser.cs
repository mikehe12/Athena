using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Parsers
{
	// Parses the start and end of a function.
	public sealed class AnnotationParser : IBufferParser<Annotation>
	{
		const byte Starter = (byte)'$';
		const byte LabelTerminatorSpace = (byte)' ';
		const byte LabelTerminatorTab = (byte)'\t';
		const byte LabelTerminatorNewline = (byte)'\n';

		readonly byte commentCharacter = (byte)'/';

		public SequencePosition? TryParse(ReadOnlySequence<byte> buffer, out Annotation result)
		{
			var section = buffer;

			while (section.PositionOf(Starter) is SequencePosition start)
			{
				var searchSection = section.Slice(start);
				
			}
			result = new();
			return buffer.Start;

			
		}


		private SequencePosition? NextWhiteSpace(ref ReadOnlySequence<byte> section)
		{
			var first = section.FirstSpan;

			if (first.IndexOfAny(LabelTerminatorSpace, LabelTerminatorNewline, LabelTerminatorTab) < 0) return default;
			Vector<byte> v;
			return section.Start;
		}
	}
}
