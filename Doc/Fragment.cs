using System;
using System.Collections.Generic;
using System.Text;

namespace Doc
{
	public sealed class Fragment
	{
		readonly ReadOnlyMemory<char> fragment;
		public string Label { get; }

		// $Doc.Fragment
		public Fragment(string label, ReadOnlyMemory<char> fragment)
		{
			if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException(nameof(label));

			Label = label;
			this.fragment = fragment;
		}
		


	}
}
