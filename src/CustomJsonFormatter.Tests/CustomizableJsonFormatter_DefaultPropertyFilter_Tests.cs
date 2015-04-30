using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Serilog.CustomJsonFormatter.Tests
{
	public class CustomizableJsonFormatter_DefaultPropertyFilter_Tests
	{
		[Fact]
		public void IntPropsValuesAreExcludedAndPropertiesAreInlined()
		{
			var sut = new CustomizableJsonFormatter();
			var messageTemplateParser = new MessageTemplateParser();
			var messageTemplate = messageTemplateParser.Parse("Before {0:l} {Bar:l} After");
			var properties = new List<LogEventProperty> { new LogEventProperty("0", new ScalarValue("foo")), new LogEventProperty("Bar", new ScalarValue("bar")) };
			var logEvent = new LogEvent(new DateTimeOffset(1980, 1, 1, 14, 15, 16, TimeSpan.FromHours(2)), LogEventLevel.Error, null, messageTemplate, properties);


			var writer = new StringWriter();
			sut.Format(logEvent, writer);
			var json = writer.ToString();


			dynamic o = JsonConvert.DeserializeObject(json);
			((string)o.message).ShouldBe("Before foo bar After");
			((string)o.template).ShouldBe("Before {0:l} {Bar:l} After");
			((string)o.Bar).ShouldBe("bar");
			PropertyShouldNotExist(o, "0");
		}

		private static void PropertyShouldNotExist(dynamic o, string propertyName)
		{
			var jobject = (JObject) o;
			JToken token;
			jobject.TryGetValue(propertyName, out token).ShouldBeFalse("The property \"" + propertyName+"\" should not exist");
		}
	}
}
