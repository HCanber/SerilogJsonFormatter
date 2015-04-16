using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Serilog.CustomJsonFormatter
{
	public class JsonFormatterHelper
	{
		private readonly InternalJsonFormatter _internalJsonFormatter;

		public JsonFormatterHelper(IFormatProvider formatProvider = null)
		{
			_internalJsonFormatter = new InternalJsonFormatter(formatProvider: formatProvider);
		}

		public void WriteJsonProperty(TextWriter output, string propertyName, object value, ref string delim)
		{
			_internalJsonFormatter.WriteJsonProperty(output, propertyName, value, ref delim);
		}

		public void WritePropertiesValues(TextWriter output, IEnumerable<KeyValuePair<string, LogEventPropertyValue>> properties, ref string delim)
		{
			_internalJsonFormatter.WritePropertiesValues(output,properties, ref delim);
		}

		private class InternalJsonFormatter : JsonFormatter
		{
			public InternalJsonFormatter(bool omitEnclosingObject = false, string closingDelimiter = null, bool renderMessage = false, IFormatProvider formatProvider = null) : base(omitEnclosingObject, closingDelimiter, renderMessage, formatProvider)
			{
			}

			public void WriteJsonProperty(TextWriter output, string propertyName, object value, ref string delim)
			{
				WriteJsonProperty(propertyName, value, ref delim, output);
			}

			public void WritePropertiesValues(TextWriter output, IEnumerable<KeyValuePair<string, LogEventPropertyValue>> properties, ref string delim)
			{
				foreach(var property in properties)
				{
					WriteJsonProperty(property.Key, property.Value, ref delim, output);
				}
			}
		}
	}
}