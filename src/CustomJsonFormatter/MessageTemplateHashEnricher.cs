using Serilog.Core;
using Serilog.Events;

namespace Serilog.CustomJsonFormatter
{
	public class MessageTemplateHashEnricher : ILogEventEnricher
	{
		private readonly string _stringFormat;
		private readonly string _propertyName;
		private readonly IStringHasher _hasher;

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageTemplateHashEnricher"/> class that will add 
		/// a property containing the hash of the log event's message template.
		/// By default this will be a string (hexadecimal encoding of the uint value) and the property name is "MessageTemplateHash"
		/// </summary>
		/// <param name="propertyName">OPTIONAL: Name of the property. Default: "MessageTemplateHash"</param>
		/// <param name="stringFormat">OPTIONAL: The string format. Set to <c>null</c> to write the hash as a number. Default: "X" hexadecimal</param>
		public MessageTemplateHashEnricher(string propertyName = "MessageTemplateHash", string stringFormat = "X")
			: this(propertyName, stringFormat, new XxHashStringHasher())
		{
		}

		public MessageTemplateHashEnricher(string propertyName, string stringFormat, IStringHasher hasher)
		{
			_propertyName = propertyName;
			_hasher = hasher;
			_stringFormat = stringFormat;

		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var hash = _hasher.CalculateHash(logEvent.MessageTemplate.Text);
			if(_stringFormat == null)
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash));
			else
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash.ToString(_stringFormat)));

		}
	}
}