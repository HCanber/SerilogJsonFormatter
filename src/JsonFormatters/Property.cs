using System;
using Serilog.Events;

namespace Hcanber.Serilog.JsonFormatters
{
	public class Property
	{
		private readonly string _messageTemplateName;
		private readonly string _name;
		private readonly LogEventPropertyValue _value;
		private readonly JsonPropertyAction _jsonPropertyAction;
		private readonly MessageTemplateAction _messageTemplateAction;

		public Property(string messageTemplateName, string name, LogEventPropertyValue value, JsonPropertyAction jsonPropertyAction, MessageTemplateAction messageTemplateAction)
		{
			_messageTemplateName = messageTemplateName;
			_name = name;
			_value = value;
			_jsonPropertyAction = jsonPropertyAction;
			_messageTemplateAction = messageTemplateAction;
		}

		public string MessageTemplateName { get { return _messageTemplateName; } }
		public string Name { get { return _name; } }
		public LogEventPropertyValue Value { get { return _value; } }
		public JsonPropertyAction JsonPropertyAction { get { return _jsonPropertyAction; } }
		public MessageTemplateAction MessageTemplateAction { get { return _messageTemplateAction; } }

	}
}