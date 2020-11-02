using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBufferConsumer
	{
		/// <summary>
		/// Inspects a buffer and indicates whether a portion is successfully consumed.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		SequencePosition TryConsume(ref ReadOnlySequence<byte> buffer);
	}
}
