using System;
using System.IO;
using Serilog.Events;

namespace Serilog.CustomJsonFormatter
{
	public class EmptyLogEventPropertyValue : LogEventPropertyValue
	{
		public static EmptyLogEventPropertyValue Instance = new EmptyLogEventPropertyValue();
		private EmptyLogEventPropertyValue(){}
		public override void Render(TextWriter output, string format = null, IFormatProvider formatProvider = null)
		{
			//Intentionally left blank
		}
	}
}