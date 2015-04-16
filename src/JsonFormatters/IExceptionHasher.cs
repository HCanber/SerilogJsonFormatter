using System;

namespace Hcanber.Serilog.JsonFormatters
{
	public interface IExceptionHasher
	{
		/// <summary>
		/// Gets the hash of the exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>The hash of the exception</returns>
		uint CalculateHash(Exception exception);
	}
}