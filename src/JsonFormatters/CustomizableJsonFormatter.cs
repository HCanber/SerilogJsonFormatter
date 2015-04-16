using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hcanber.Serilog.JsonFormatters.Filter;
using Serilog.Events;
using Serilog.Formatting;

namespace Hcanber.Serilog.JsonFormatters
{
	/// <summary>
	/// Custom Json formatter that's more customizable than the normal JsonFormatter.
	/// </summary>
	public class CustomizableJsonFormatter : ITextFormatter
	{
		private readonly bool _omitEnclosingObject;
		private readonly string _closingDelimiter;
		private readonly bool _shouldRenderMessage;
		private readonly IFormatProvider _formatProvider;
		private readonly PropertyFilter _propertyFilter;
		private readonly PropertyNames _propertyNames;
		private readonly JsonFormatterHelper _helper;



		/// <summary>
		/// Construct a <see cref="CustomizableJsonFormatter"/>.
		/// </summary>
		/// <param name="omitEnclosingObject">If true, the properties of the event will be written to
		/// the output without enclosing braces. Otherwise, if false, each event will be written as a well-formed
		/// JSON object.</param>
		/// <param name="closingDelimiter">A string that will be written after each log event is formatted.
		/// If null, <see cref="Environment.NewLine"/> will be used. Ignored if <paramref name="omitEnclosingObject"/>
		/// is true.</param>
		/// <param name="shouldRenderMessage">If true, the message will be rendered and written to the output as a property named RenderedMessage.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="jsonPropertyNames">Specifies the json property names.</param>
		/// <param name="propertyFilter">Makes it possible to filter out/remove property values from being rendered. Defaults to <see cref="PropertyFilters.Default"/>.</param>
		public CustomizableJsonFormatter(
			bool omitEnclosingObject = false,
			string closingDelimiter = null,
			bool shouldRenderMessage = true,
			IFormatProvider formatProvider = null,
			PropertyNames jsonPropertyNames = null,
			PropertyFilter propertyFilter = null)
		{
			_propertyNames = jsonPropertyNames ?? new PropertyNames();
			_omitEnclosingObject = omitEnclosingObject;
			_closingDelimiter = closingDelimiter ?? Environment.NewLine;
			_shouldRenderMessage = shouldRenderMessage;
			_formatProvider = formatProvider;
			_propertyFilter = propertyFilter ?? PropertyFilters.Default;
			_helper = new JsonFormatterHelper(formatProvider);
		}

		protected bool OmitEnclosingObject { get { return _omitEnclosingObject; } }
		protected string ClosingDelimiter { get { return _closingDelimiter; } }
		protected bool ShouldRenderMessage { get { return _shouldRenderMessage; } }
		protected IFormatProvider FormatProvider { get { return _formatProvider; } }
		protected PropertyNames PropertyNames { get { return _propertyNames; } }

		/// <summary>
		/// Format the log event into the output.
		/// </summary>
		/// <param name="logEvent">The event to format.</param>
		/// <param name="output">The output.</param>
		public virtual void Format(LogEvent logEvent, TextWriter output)
		{
			if(logEvent == null) throw new ArgumentNullException("logEvent");
			if(output == null) throw new ArgumentNullException("output");

			if(!_omitEnclosingObject)
				output.Write("{");

			WriteContent(logEvent, output);

			if(!_omitEnclosingObject)
			{
				output.Write("}");
				output.Write(_closingDelimiter);
			}
		}

		protected virtual void WriteContent(LogEvent logEvent, TextWriter output)
		{
			var delim = "";
			WriteTimestamp(logEvent.Timestamp, ref delim, output, logEvent);
			WriteLevel(logEvent.Level, ref delim, output, logEvent);
			var properties = DeterminePropertiesHandling(logEvent);
			if(_shouldRenderMessage)
			{
				var message = RenderMessage(logEvent, _formatProvider);
				WriteRenderedMessage(message, ref delim, output, logEvent);
			}

			WriteMessageTemplate(logEvent.MessageTemplate, properties, ref delim, output, logEvent);

			if(logEvent.Properties.Count != 0)
				WriteProperties(properties, ref delim, output, logEvent);

			var exception = logEvent.Exception;
			if(exception != null)
			{
				WriteException(exception, ref delim, output, logEvent);
			}
		}


		protected virtual List<Property> DeterminePropertiesHandling(LogEvent logEvent)
		{
			// ReSharper disable once ConvertClosureToMethodGroup
			return logEvent.Properties.Select(p => DetermineHowPropertyIsToBeHandled(p.Key, p.Value, logEvent)).ToList();
		}

		protected virtual Property DetermineHowPropertyIsToBeHandled(string name, LogEventPropertyValue value, LogEvent logEvent)
		{
			var a = new PropertyAction(name);
			a = _propertyFilter.Filter(a);
			return new Property(name, a.PropertyName, value, a.JsonPropertyAction.GetValueOrDefault(JsonPropertyAction.Inline), a.MessageTemplateAction.GetValueOrDefault(MessageTemplateAction.AsIs));
		}

		protected virtual string RenderMessage(LogEvent logEvent, IFormatProvider formatProvider)
		{
			return logEvent.RenderMessage(formatProvider);
		}


		/// <summary>
		/// Writes out the attached properties
		/// </summary>
		protected virtual void WriteProperties(IReadOnlyCollection<Property> properties, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertiesToRender = new List<Property>();
			foreach(var property in properties)
			{
				switch(property.JsonPropertyAction)
				{
					case JsonPropertyAction.Exclude:
						//Intentionally left blank
						break;
					case JsonPropertyAction.Inline:
						WriteJsonProperty(output, property.Name, property.Value, ref delim);
						break;
					case JsonPropertyAction.AsSharedProperty:
						propertiesToRender.Add(property);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if(propertiesToRender.Count > 0)
			{
				var propertyName = _propertyNames.Properties;
				if(propertyName == null) return;
				output.Write(delim);

				output.Write('"');
				output.Write(propertyName);
				output.Write("\":{");
				WritePropertiesValues(output, propertiesToRender.ToDictionary(k => k.Name, k => k.Value));
				output.Write("}");
				delim = ",";
			}
		}

		/// <summary>
		/// Writes out the attached exception
		/// </summary>
		protected virtual void WriteException(Exception exception, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertyName = _propertyNames.Exception;
			if(propertyName == null) return;
			WriteJsonProperty(output, propertyName, exception, ref delim);
		}


		/// <summary>
		/// (Optionally) writes out the rendered message
		/// </summary>
		protected virtual void WriteRenderedMessage(string message, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertyName = _propertyNames.Message;
			if(propertyName == null) return;
			WriteJsonProperty(output, propertyName, message, ref delim);
		}

		protected virtual void WriteMessageTemplate(MessageTemplate messageTemplate, IReadOnlyCollection<Property> properties, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertyName = _propertyNames.MessageTemplate;
			if(propertyName == null) return;
			var messageTemplateText = messageTemplate.Text;
			var templateHandlingNeeded = properties.Any(p => p.MessageTemplateAction != MessageTemplateAction.AsIs);
			if(templateHandlingNeeded)
			{
				var valuesToReplace = properties.Select(p =>
				{
					switch(p.MessageTemplateAction)
					{
						case MessageTemplateAction.AsIs:
							return (Tuple<string, LogEventPropertyValue>) null;
						case MessageTemplateAction.RenderValue:
							return Tuple.Create(p.MessageTemplateName, p.Value);
						case MessageTemplateAction.Remove:
							return Tuple.Create(p.MessageTemplateName, (LogEventPropertyValue)EmptyLogEventPropertyValue.Instance);
						default:
							throw new ArgumentOutOfRangeException();
					}
				}).Where(t => t != null).ToDictionary(t => t.Item1, t => t.Item2);
				var writer = new StringWriter();
				foreach(var messageTemplateToken in messageTemplate.Tokens)
				{
					//Render the template. 
					// - Text will be rendered as is. 
					// - Properties that exists in valuesToReplace will be replaced with the value (the real value or empty string)
					// - Other properties will be rendered as in the original template
					messageTemplateToken.Render(valuesToReplace, writer, _formatProvider);
				}
				messageTemplateText = writer.ToString();
			}

			WriteMessageTemplate(messageTemplateText, ref delim, output);
		}

		/// <summary>
		/// Writes out the message template for the logevent.
		/// </summary>
		protected virtual void WriteMessageTemplate(string template, ref string delim, TextWriter output)
		{
			var propertyName = _propertyNames.MessageTemplate;
			if(propertyName == null) return;

			WriteJsonProperty(output, propertyName, template, ref delim);
		}

		/// <summary>
		/// Writes out the log level
		/// </summary>
		protected virtual void WriteLevel(LogEventLevel level, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertyName = _propertyNames.Level;
			if(propertyName == null) return;
			var stringLevel = Enum.GetName(typeof(LogEventLevel), level);
			WriteJsonProperty(output, propertyName, stringLevel, ref delim);
		}

		/// <summary>
		/// Writes out the log timestamp
		/// </summary>
		protected virtual void WriteTimestamp(DateTimeOffset timestamp, ref string delim, TextWriter output, LogEvent logEvent)
		{
			var propertyName = _propertyNames.Timestamp;
			if(propertyName == null) return;
			WriteJsonProperty(output, propertyName, timestamp, ref delim);
		}


		protected virtual void WriteJsonProperty(TextWriter output, string propertyName, object value, ref string delim)
		{
			_helper.WriteJsonProperty(output, propertyName, value, ref delim);
		}

		private void WritePropertiesValues(TextWriter output, IEnumerable<KeyValuePair<string, LogEventPropertyValue>> properties)
		{
			string delim = "";
			WritePropertiesValues(output, properties, ref delim);
		}

		private void WritePropertiesValues(TextWriter output, IEnumerable<KeyValuePair<string, LogEventPropertyValue>> properties, ref string delim)
		{
			_helper.WritePropertiesValues(output, properties, ref delim);
		}
	}
}