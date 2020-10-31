using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	/// <summary>
	/// Finds comments in source code
	/// </summary>
	public sealed class CommentIdentifier
	{
		readonly char[] CommentSequence = "//".ToCharArray();

		public void ParseLine(SourceLine line)
		{
			
		}
	}
}
