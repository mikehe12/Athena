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
	public readonly struct Result<T> : IResult<T>
	{
		public bool IsSuccess { get; }
		public T Value { get; }

		public Result(T value)
		{
			IsSuccess = true;
			Value = value;
		}

		public bool TryGet([MaybeNullAttribute] out T value)
		{
			if (!IsSuccess)
			{
				value = default;
				return false;
			}

			value = Value;
			return true;
		}

		public static Result<T> Success(T value) => new Result<T>(value);
		public static Result<T> Failure() => new Result<T>();


		public bool Success([MaybeNullWhen(false)] out T result)
		{
			if (IsSuccess)
			{
				result = Value;
				return true;
			}
			result = default;
			return false;
		}

		public bool Success([MaybeNullWhen(false)] out T result, [MaybeNullWhen(true)] out string message)
		{
			if (IsSuccess)
			{
				result = Value;
				message = null;
				return true;
			}
			result = default;
			message = "error";
			return false;
		}

	}

	public class TestResult
	{
		public Result<int> GetResult(bool success)
		{
			if (success) return new Result<int>(5);
			return default;
		}

		public void UseResult1()
		{
			if (GetResult(true).Success(out int value, out string message))
			{

			}
			else
			{
				Console.WriteLine(message);
			}
		}


		public void UseResult2()
		{
			var res = GetResult(true);
			if (res.Success(out int value))
			{

			}
			else
			{
				Console.WriteLine();
			}
		}


		public void UseResult3()
		{
			var res = GetResult(true);
			var x = res switch
			{
				{ IsSuccess: true } => 5,
				{ IsSuccess: false } => 4
			};
		}
	}

	public static class Result
	{
		public static bool Success<ROfT, T>(this ROfT res, [MaybeNullWhen(false)] out T result) where ROfT: struct, IResult<T>
		{
			if (res.IsSuccess)
			{
				result = res.Value;
				return true;
			}
			result = default;
			return false;
		}
	}

	public interface IResult<T>
	{
		public bool IsSuccess { get; }
		public T Value { get; }
	}

	public interface IFailure
	{

	}

	public readonly struct Success<T>
	{
		public T Value { get; }
	}

	public readonly struct Failure
	{

	}

}
