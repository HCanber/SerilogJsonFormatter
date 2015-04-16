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
		public void IntPropsValuesAreExcluded()
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

		[Fact]
		public void UnderScorePropsValuesAreExcluded()
		{
			var sut = new CustomizableJsonFormatter();
			var messageTemplateParser = new MessageTemplateParser();
			var messageTemplate = messageTemplateParser.Parse("Before {_Foo:l} {_Bar_:l} After");
			var properties = new List<LogEventProperty> { new LogEventProperty("_Foo", new ScalarValue("foo")), new LogEventProperty("_Bar_", new ScalarValue("bar")) };
			var logEvent = new LogEvent(new DateTimeOffset(1980, 1, 1, 14, 15, 16, TimeSpan.FromHours(2)), LogEventLevel.Error, null, messageTemplate, properties);


			var writer = new StringWriter();
			sut.Format(logEvent, writer);
			var json = writer.ToString();


			dynamic o = JsonConvert.DeserializeObject(json);
			((string)o.message).ShouldBe("Before foo bar After");
			((string)o.template).ShouldBe("Before {_Foo:l}  After");
			PropertyShouldNotExist(o, "Foo");
			PropertyShouldNotExist(o, "Bar");
		}


		[Fact]
		public void DoubleUnderscorePropsValuesAreExcludedAndInlined()
		{
			var sut = new CustomizableJsonFormatter();
			var messageTemplateParser = new MessageTemplateParser();
			var messageTemplate = messageTemplateParser.Parse("Before {__Foo:l} {Bar__:l} {__FooBar__:l} After");
			var properties = new List<LogEventProperty> { new LogEventProperty("__Foo", new ScalarValue("foo")), new LogEventProperty("Bar__", new ScalarValue("bar")), new LogEventProperty("__FooBar__", new ScalarValue("foobar")), };
			var logEvent = new LogEvent(new DateTimeOffset(1980, 1, 1, 14, 15, 16, TimeSpan.FromHours(2)), LogEventLevel.Error, null, messageTemplate, properties);


			var writer = new StringWriter();
			sut.Format(logEvent, writer);
			var json = writer.ToString();


			dynamic o = JsonConvert.DeserializeObject(json);
			((string)o.message).ShouldBe("Before foo bar foobar After");
			((string)o.template).ShouldBe("Before {__Foo:l} bar foobar After");
			((string)o.Bar).ShouldBe("bar");

			PropertyShouldNotExist(o, "Foo");
			PropertyShouldNotExist(o, "Foobar");
		}



		private static void PropertyShouldNotExist(dynamic o, string propertyName)
		{
			var jobject = (JObject) o;
			JToken token;
			jobject.TryGetValue(propertyName, out token).ShouldBeFalse("The property \"" + propertyName+"\" should not exist");
		}
	}
}
