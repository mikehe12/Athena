using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Primitives
{
	/// <summary>
	/// Struct based Result Monad
	/// </summary>
	public readonly struct Result<T>
	{
		private readonly T value;
		private readonly bool isSuccess;

		public Result(T value)
		{
			isSuccess = true;
			this.value = value;
		}

		public bool TryGet([MaybeNullAttribute] out T value)
		{
			if (!isSuccess)
			{
				value = default;
				return false;
			}

			value = this.value;
			return true;
		}

		public static Result<T> Success(T value) => new Result<T>(value);
		public static Result<T> Failure() => new Result<T>();

	}

	public readonly struct Success<T>
	{
		
	}

}
