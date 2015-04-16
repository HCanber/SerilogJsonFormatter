using System;
using System.Diagnostics;

namespace Serilog.CustomJsonFormatter
{
	public class DefaultExceptionHasher : IExceptionHasher
	{
		public uint CalculateHash(Exception exception)
		{
			var currentException = exception;
			uint hash = 0;
			while(currentException != null)
			{
				var stackTrace = new StackTrace(currentException, false);
				var frameCount = stackTrace.FrameCount;
				for(var i = 0; i < frameCount; i++)
				{
					var stackFrame = stackTrace.GetFrame(i);
					var methodBase = stackFrame.GetMethod();
					if(methodBase!=null)
						hash = unchecked(hash * 397 ^ (uint) methodBase.GetHashCode());

					hash = unchecked(hash * 397 ^ (uint)stackFrame.GetNativeOffset());
				}

				currentException = currentException.InnerException;
			}
			return hash;
		}
	}
}