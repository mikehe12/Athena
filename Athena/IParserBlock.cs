﻿using Athena.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
	/// <summary>
	/// A block in a parsing pipeline that performs a parsing step.
	/// The block can be constructed with configuration, but operates
	/// functionally.
	/// </summary>
	public interface IParserBlock<TIn, TOut, TState>
	{
		public (SequencePosition?, Result<TOut>) Parse(TIn input, ref TState context);
	}
}
