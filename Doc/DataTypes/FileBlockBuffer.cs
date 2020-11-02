using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.DataTypes
{
	public readonly struct FileBlockBuffer
	{
		public string FileName { get; }
		public ReadOnlySequence<byte> Buffer { get; }
	}
}
