using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc
{
	internal struct SnippetBuilder
	{
		public SequencePosition TagStart { get; set; }
		public SequencePosition TagEnd { get; set; }
		public SequencePosition TextStart { get; set; }
		public SequencePosition TextEnd { get; set; }
	}
}
