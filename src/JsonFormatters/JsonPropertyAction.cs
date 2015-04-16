namespace Hcanber.Serilog.JsonFormatters
{
	public enum JsonPropertyAction
	{
		/// <summary>The property's value is written as a json property directly on the root object.</summary>
		Inline,

		/// <summary>Prevents the property's value from being written as a json property.</summary>
		Exclude,

		/// <summary>The property's value is written as a json property under the shared json property.</summary>
		AsSharedProperty,
	}
}