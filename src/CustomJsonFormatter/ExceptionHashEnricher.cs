using Serilog.Core;
using Serilog.Events;

namespace Serilog.CustomJsonFormatter
{
	public class ExceptionHashEnricher : ILogEventEnricher
	{
		private readonly string _propertyName;
		private readonly IExceptionHasher _hasher;
		private readonly string _stringFormat;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHashEnricher"/> class that will add 
		/// a property containing the hash of the log event's exception (if one exists).
		/// By default this will be a string (hexadecimal encoding of the uint value) and the property name is "ExceptionHash"
		/// </summary>
		/// <param name="propertyName">OPTIONAL: Name of the property. Default: "ExceptionHash"</param>
		/// <param name="stringFormat">OPTIONAL: The string format. Set to <c>null</c> to write the hash as a number. Default: "X" hexadecimal</param>
		public ExceptionHashEnricher(string propertyName = "ExceptionHash", string stringFormat = "X")
			: this(propertyName, stringFormat, new DefaultExceptionHasher())
		{
		}

		public ExceptionHashEnricher(string propertyName, string stringFormat, IExceptionHasher hasher)
		{
			_propertyName = propertyName;
			_hasher = hasher;
			_stringFormat = stringFormat;
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var exception = logEvent.Exception;
			if(exception != null)
			{
				var hash = _hasher.CalculateHash(exception);
				if(_stringFormat == null)
					logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash));
				else
					logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash.ToString(_stringFormat)));
			}
		}
	}
}