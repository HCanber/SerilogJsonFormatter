namespace Serilog.CustomJsonFormatter.Filter
{
	public class PropertyAction
	{
		private readonly string _originalPropertyName;

		public PropertyAction(string originalPropertyName)
		{
			_originalPropertyName = originalPropertyName;
			PropertyName = originalPropertyName;
		}

		public JsonPropertyAction? JsonPropertyAction { get; set; }
		public MessageTemplateAction? MessageTemplateAction { get; set; }
		public string PropertyName { get; set; }
		public string OriginalPropertyName { get { return _originalPropertyName; } }
	}
}