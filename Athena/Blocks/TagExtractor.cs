using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Blocks
{
	public sealed class TagExtractor
	{
		const char TagDelimiter = '.';

		public (bool, string[]) TryGetTags(in ReadOnlySequence<byte> source)
		{
			var subTags = Encoding.UTF8.GetString(source).Trim().Split(TagDelimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			if (subTags.Length <= 0) return default;
			return (true, subTags);
		}
	}
}
