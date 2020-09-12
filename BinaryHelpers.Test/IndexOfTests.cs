using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BinaryHelpers.Test
{
	public class IndexOfTests
	{
		const byte junk = 0x0;
		const byte target = 0x04;

		[Theory]
		[InlineData(1024)]
		public void GivenTargetAtEnd_TestSearchOfLength(int length)
		{
			// Arrange data
			var data = new byte[length];
			Array.Clear(data, 0, length);
			data[length - 1] = target;

			var span = new Span<byte>(data);

			var index = BinaryIndex.IndexOfOrLessThan(
				ref MemoryMarshal.GetReference(span),
				0x4,
				0x2,
				0x0,
				span.Length
				);

			Assert.Equal(length-1, index);
		}
	}
}
