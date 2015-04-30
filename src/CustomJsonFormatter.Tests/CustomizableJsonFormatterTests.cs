using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.CustomJsonFormatter.Filter;
using Serilog.Parsing;
using Xunit;

namespace Serilog.CustomJsonFormatter.Tests
{
	public class CustomizableJsonFormatterTests
	{
		[Fact]
		public void CustomizableJsonFormatter_using_ExcludePropertiesStartingWithUnderscore_should_inline_and_not_fail_when_formatting_underlineName()
		{
			var propertyNames=new PropertyNames();
			var sut = new CustomizableJsonFormatter(propertyFilter: PropertyFilters.ExcludePropertiesStartingWithUnderscore(), jsonPropertyNames:propertyNames);
			var writer = new StringWriter();
			var logger = new LoggerConfiguration()
				.WriteTo.Sink(new TextWriterSink(writer, sut))
				.CreateLogger();

			//Tests that
			// 1. message get correctly formatted
			// 2. {_abc} gets inlined in template
			// 3. {abc} and {_abc} don't clash
			logger.Information("{abc} {_abc}", 42, 4711);
			var json =(JObject) JsonConvert.DeserializeObject(writer.ToString());
			json[propertyNames.Message].ToString().ShouldBe("42 4711");
			json[propertyNames.MessageTemplate].ToString().ShouldBe("{abc} 4711");
		} 
	}
}