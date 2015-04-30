using System;

namespace Serilog.CustomJsonFormatter
{
	public class PropertyNames
	{
		public PropertyNames()
		{
			Properties = "properties";
			Exception = "exception";
			ExceptionHash = "exceptionHash";
			Message = "message";
			MessageTemplate = "template";
			MessageTemplateHash = "templateHash";
			Level = "level";
			Timestamp = "@timestamp";
			FormattingErrors = "_formattingErrors";
		}
		/// <summary>
		/// The name of the object where non-inlined properties are written. Set this to <c>null</c> to remove this entirely from the output.
		/// Default: <c>properties</c>
		/// </summary>
		public string Properties { get; set; }

		/// <summary>
		/// The name of the property where exceptions will be written.
		/// Default: <c>exception</c>
		/// </summary>
		public string Exception { get; set; }

		/// <summary>
		/// The name of the property where a hash of the exceptions will be written.
		/// Default: <c>exception</c>
		/// </summary>
		public string ExceptionHash { get; set; }


		public string Message { get; set; }
		public string MessageTemplate { get; set; }
		public string MessageTemplateHash { get; set; }
		public string Level { get; set; }
		public string Timestamp { get; set; }

		/// <summary>
		/// The name of the property which will contain the error message if something went wrong during formatting.
		/// Default: <c>_formattingErrors</c>
		/// </summary>
		public string FormattingErrors { get; set; }


	}
}