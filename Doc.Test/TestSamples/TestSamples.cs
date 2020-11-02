using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Test
{
	public static class TestSamples
	{
		const string SamplesFolder = "TestSamples";

		public static string Folder { get; } = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), SamplesFolder);
	}
}
