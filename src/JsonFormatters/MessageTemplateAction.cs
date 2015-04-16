namespace Hcanber.Serilog.JsonFormatters
{
	public enum MessageTemplateAction
	{
		/// <summary>The property is not changed in the message template, e.g. <c>{TheProperty}</c> is left intact. This is the default if not <see cref="RenderValue"/> or <see cref="Remove"/> has been specified.</summary>
		AsIs,

		/// <summary>The property is replaced in the message template by it's value, e.g. if the value for TheProperty is "some value" then <c>{TheProperty}</c> is replaced by "some value" in the message template.</summary>
		RenderValue,

		/// <summary>The property is removed entirely from the message template, e.g. <c>"{TheProperty}"</c> is replaced by <c>""</c>.</summary>
		Remove,
	}
}