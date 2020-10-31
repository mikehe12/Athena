using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	/// <summary>
	/// A tagged section of source.
	/// </summary>
	public sealed class TaggedSection
	{
		public TaggedSection(string tag, ReadOnlySequence<byte> section)
		{
			Tag = tag;
			Section = section;
		}

		public string Tag { get; }
		public ReadOnlySequence<byte> Section { get; }
	}
}
