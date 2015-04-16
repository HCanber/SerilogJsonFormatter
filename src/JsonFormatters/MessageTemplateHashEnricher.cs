using System;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Hcanber.Serilog.JsonFormatters
{
	public class MessageTemplateHashEnricher : ILogEventEnricher
	{
		private readonly string _propertyName;
		private readonly IStringHasher _hasher;

		public MessageTemplateHashEnricher(string propertyName = "MessageTemplateHash")
			: this(propertyName, new XxHashStringHasher())
		{
		}

		public MessageTemplateHashEnricher(string propertyName, IStringHasher hasher)
		{
			_propertyName = propertyName;
			_hasher = hasher;
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var hash = _hasher.CalculateHash(logEvent.MessageTemplate.Text);
			logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_propertyName, hash));
		}
	}
}