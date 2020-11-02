using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.DataTypes
{
	public readonly struct LineBuffer
	{
		public string FileName { get; }
		public int LineNumber { get; }
		public ReadOnlySequence<byte> Buffer { get; }
	}
}
