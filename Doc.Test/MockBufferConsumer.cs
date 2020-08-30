using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Test
{
	class MockBufferConsumer : IBufferConsumer
	{
		public Func<ReadOnlySequence<byte>, SequencePosition> OnTryConsume;

		public SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer) => OnTryConsume.Invoke(buffer);
	}
}
