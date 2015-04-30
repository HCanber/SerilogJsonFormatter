using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.CustomJsonFormatter.Tests
{
	public class TextWriterSink : ILogEventSink
	{
		readonly TextWriter _textWriter;
		readonly ITextFormatter _textFormatter;
		readonly object _syncRoot = new object();

		public TextWriterSink(TextWriter textWriter, ITextFormatter textFormatter)
		{
			if(textFormatter == null) throw new ArgumentNullException("textFormatter");
			_textWriter = textWriter;
			_textFormatter = textFormatter;
		}

		public void Emit(LogEvent logEvent)
		{
			if(logEvent == null) throw new ArgumentNullException("logEvent");
			lock(_syncRoot)
			{
				_textFormatter.Format(logEvent, _textWriter);
				_textWriter.Flush();
			}
		}
	}
}