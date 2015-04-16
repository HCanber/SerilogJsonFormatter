using Serilog.Core;
using Serilog.Events;

namespace Hcanber.Serilog.JsonFormatters
{
	public class ExceptionHashEnricher : ILogEventEnricher
	{
		private readonly string _propertyName;
		private readonly IExceptionHasher _hasher;

		public ExceptionHashEnricher(string propertyName = "ExceptionHash")
			: this(propertyName, new DefaultExceptionHasher())
		{
		}

		public ExceptionHashEnricher(string propertyName, IExceptionHasher hasher)
		{
			_propertyName = propertyName;
			_hasher = hasher;
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var exception = logEvent.Exception;
			if(exception!=null)
			{
				var hash = _hasher.CalculateHash(exception);
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash));
				
			}
		}
	}
}