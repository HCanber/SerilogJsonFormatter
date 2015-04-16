using System;
using System.Threading;
using Hcanber.Serilog.JsonFormatters;
using Hcanber.Serilog.JsonFormatters.Filter;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.RollingFile;

// ReSharper disable once CheckNamespace
namespace Serilog
{
	public static class SerilogExtensions
	{
		public const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024;
		public const int DefaultRetainedFileCountLimit = 31; // A long month of logs

		/// <summary>
		/// <para>
		/// Write log events to a series of files. Each file will be named according to
		/// the date of the first log entry written to it. Only simple date-based rolling is
		/// currently supported.</para>
		/// <para>Each line will be formatted as a json object in the logstash format and every line will contains a @timestamp</para>
		/// 
		/// <para>All properties' values will be inlined, i.e. directly on the root json object, except:</para>
		/// 
		/// <para>Properties with names consisting of only digits, e.g. <c>{0}</c> and <c>{1}</c>. Their values will not be written.</para>
		/// 
		/// <para>Properties that starts with an underscore, e.g. <c>{_WillBeRemoved}</c>. Their values will not be written.
		/// If the name also ends with <c>_</c> it will be removed from the message template as well and only exist in the rendered message.</para>
		/// 
		/// <para>Properties that starts and ends with two underscores, e.g. <c>{__InlineMe__}</c>. Their values will not be written and the properties will be replaced by their values in the message template.</para>
		/// 
		/// <para>The default behavior can be replaced by specifying <paramref name="propertyFilter" />.
		/// For example, to put all properties under a property named <c>properties</c> set it to <see cref="PropertyFilters.InlineNoFields" />.</para>
		/// 
		/// <example>If the messageTemplate is <c>"{Value} {_Remove1} {_Remove2} {__Inline__}"</c> and the values are their names as strings the result will be:
		/// <code>
		/// {
		///   "template" : "{Value} {_Remove1} Inline",
		///   "message" : "Value Remove1 Remove2 Inline"
		///   "Value" : "Value"
		/// }
		/// </code>
		/// 
		/// </example>
		/// 
		/// <para>Filters can be chained so if you want to override the default behavior you can <c>yourFilter + PropertyFilters.Default</c> to have <c>yourFilter</c> 
		/// applied before the defaults</para>
		/// 
		/// <para>To not remove properties that starts with underscore, set <paramref name="propertyFilter"/> to <see cref="PropertyFilters.ExcludeIntProperties"/></para>
		/// 
		/// <para>The property names for message template, message, timestamp and so on are specified in the class <see cref="PropertyNames"/></para>
		/// </summary>
		/// <param name="sinkConfiguration">Logger sink configuration.</param>
		/// <param name="pathFormat">String describing the location of the log files,
		/// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
		/// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
		/// <param name="restrictedToMinimumLevel">OPTIONAL: The minimum level for
		/// events passed through the sink. Default: <see cref="LevelAlias.Minimum"/></param>
		/// <param name="shouldRenderMessage">OPTIONAL: if set to <c>true</c> the message will be rendered. Default: <c>true</c></param>
		/// <param name="propertyFilter">OPTIONAL: The property filter. Default: <c>null</c>, meaning <see cref="PropertyFilters.Default"/> is used</param>
		/// <param name="jsonPropertyNames">OPTIONAL: The json property names. Default: <c>null</c>, meaning the default values in <see cref="PropertyNames"/> is used</param>
		/// <param name="formatProvider">OPTIONAL: Supplies culture-specific formatting information, or null. Default: <c>null</c></param>
		/// <param name="fileSizeLimitBytes">OPTIONAL: The maximum size, in bytes, to which any single log file will be allowed to grow.
		/// For unrestricted growth, pass null. The default is 1 GB.</param>
		/// <param name="retainedFileCountLimit">OPTIONAL: The maximum number of log files that will be retained,
		/// including the current log file. For unlimited retention, pass null. The default is 31.</param>
		/// <returns>
		/// Configuration object allowing method chaining.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="sinkConfiguration"/> is null</exception>
		/// <remarks>
		/// The file will be written using the UTF-8 character set.
		/// </remarks>
		public static LoggerConfiguration RollingFileWithJson(this LoggerSinkConfiguration sinkConfiguration, string pathFormat, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum, bool shouldRenderMessage = true, PropertyFilter propertyFilter = null, PropertyNames jsonPropertyNames = null, IFormatProvider formatProvider = null, long? fileSizeLimitBytes = DefaultFileSizeLimitBytes, int? retainedFileCountLimit = DefaultRetainedFileCountLimit)
		{
			if(sinkConfiguration == null) throw new ArgumentNullException("sinkConfiguration");
			var formatter = new CustomizableJsonFormatter(formatProvider: formatProvider,
				shouldRenderMessage: shouldRenderMessage, 
				propertyFilter: propertyFilter, 
				jsonPropertyNames: jsonPropertyNames);
			var sink = new RollingFileSink(pathFormat, formatter, fileSizeLimitBytes, retainedFileCountLimit);
			return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
		}

		/// <summary>
		/// Enrich log events with a MessageTemplateHash property containing a hash of the current message template.
		/// The hash can be used to find similar messages when searching the log.
		/// </summary>
		/// <param name="enrichmentConfiguration">The config</param>
		/// <param name="propertyName">OPTIONAL: the name of the property. Default: "MessageTemplateHash"</param>
		/// <returns>Configuration object allowing method chaining.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enrichmentConfiguration"/> is <c>null</c></exception>
		public static LoggerConfiguration WithMessageTemplateHash(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName = "MessageTemplateHash")
		{
			if(enrichmentConfiguration == null) throw new ArgumentNullException("enrichmentConfiguration");
			return enrichmentConfiguration.With(new MessageTemplateHashEnricher(propertyName));
		}

		/// <summary>
		/// Enrich log events with a ExceptionHash property containing a hash of the exception if one is logged.
		///  The hash can be used to find other occurrences of when the same exception was thrown when searching the log.
		/// </summary>
		/// <param name="enrichmentConfiguration">The config</param>
		/// <param name="propertyName">OPTIONAL: the name of the property. Default: "ExceptionHash"</param>
		/// <returns>Configuration object allowing method chaining.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enrichmentConfiguration"/> is <c>null</c></exception>
		public static LoggerConfiguration WithExceptionHash(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName = "ExceptionHash")
		{
			if(enrichmentConfiguration == null) throw new ArgumentNullException("enrichmentConfiguration");
			return enrichmentConfiguration.With(new ExceptionHashEnricher(propertyName));
		}
	}
}