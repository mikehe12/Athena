using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.DataTypes
{
	public readonly struct LineReference
	{
		public LineReference(string fileName, int lineNumber)
		{
			FileName = fileName;
			LineNumber = lineNumber;
		}

		public string FileName { get; }
		public int LineNumber { get; }
	}
}
