using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Test
{
	public static class StringHelpers
	{
		public static string GetRandomAlphanumericString(int length)
		{
			const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			var random = new Random();
			var randomString = new string(Enumerable.Range(0, length)
													.Select(_ => charset[random.Next(charset.Length)])
													.ToArray());
			return randomString;
		}
	}
}
