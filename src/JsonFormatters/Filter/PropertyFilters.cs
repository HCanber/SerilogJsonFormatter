namespace Hcanber.Serilog.JsonFormatters.Filter
{
	public static class PropertyFilters
	{
		/// <summary>A handler that inlines all fields.</summary>
		public static PropertyFilter InlineAllFields
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					action.JsonPropertyAction = JsonPropertyAction.Inline;
					return action;
				});
			}
		}

		/// <summary>
		/// Excludes the values for properties with the specified name.
		/// By default the property will be replaced in the messaget template as well.
		/// Use <paramref name="messageTemplateAction"/> to specify other behaviors.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="messageTemplateAction">OPTIONAL: The message template action. You can either leave th property as is, remove it, or replace with the rendered value. Default: <see cref="MessageTemplateAction.RenderValue"/></param>
		/// <param name="comparisonType">OPTIONAL: Specifies how the strings will be compared. Default: <see cref="StringComparison.OrdinalIgnoreCase"/></param>
		/// <returns>The property filter.</returns>
		public static PropertyFilter ExcludeProperty(string propertyName, MessageTemplateAction? messageTemplateAction=MessageTemplateAction.RenderValue, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
		{
			return new DelegatePropertyFilter(action =>
			{
				if(!action.JsonPropertyAction.HasValue && string.Equals(action.PropertyName, propertyName, comparisonType))
				{
					action.JsonPropertyAction = JsonPropertyAction.Exclude;
					if(messageTemplateAction!=null)
						action.MessageTemplateAction = messageTemplateAction;
				}
				return action;
			});
		}

		/// <summary>A handler that inlines no fields.</summary>
		public static PropertyFilter InlineNoFields
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					action.JsonPropertyAction = JsonPropertyAction.AsSharedProperty;
					return action;
				});
			}
		}

		/// <summary>A filter that puts all properties that have integer names, for example <c>{0}</c> and <c>{1}</c> under the shared property, and not inline.</summary>
		public static PropertyFilter IntPropertiesAsSharedProperty
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					int val;
					if(int.TryParse(action.PropertyName, out val))
						action.JsonPropertyAction = JsonPropertyAction.AsSharedProperty;
					return action;
				});
			}
		}


		/// <summary>A filter that excludes all properties that have integer names, for example <c>{0}</c> and <c>{1}</c>.</summary>
		public static PropertyFilter ExcludeIntProperties
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					int val;
					if(int.TryParse(action.PropertyName, out val))
						action.JsonPropertyAction = JsonPropertyAction.Exclude;
					return action;
				});
			}
		}


		/// <summary>
		/// A filter that:
		/// <para>If the property name starts with double underscores, for example <c>{__Exclude}</c>, its value will not be included as a json property.</para>
		/// <para>If the property name ends with double underscores, for example <c>{InlineValue__}</c>, its value will be inlined in the message template</para>
		/// <para>If the property name both starts and ends with double underscores, for example <c>{__ExcludeAndInlineValue__}</c>, its value will not be included as a json property but its value will be inlined in the message template.</para>
		/// </summary>
		public static PropertyFilter InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					var fieldName = action.PropertyName;
					if(fieldName == null) return action;
					var length = fieldName.Length;
					if(length < 3) return action;

					var startsWith = fieldName[0] == '_' && fieldName[1] == '_';
					var endsWithEqual = length >= 5 && fieldName[length - 1] == '_' && fieldName[length - 2] == '_';

					if(startsWith)
					{
						if(endsWithEqual)
						{
							action.JsonPropertyAction = JsonPropertyAction.Exclude;
							action.MessageTemplateAction = MessageTemplateAction.RenderValue;
							action.PropertyName = fieldName.Substring(2, length - 4);
						}
						else
						{
							action.JsonPropertyAction = JsonPropertyAction.Exclude;
							action.PropertyName = fieldName.Substring(2, length - 2);
						}
					}
					else if(endsWithEqual)
					{
						action.MessageTemplateAction = MessageTemplateAction.RenderValue;
						action.PropertyName = fieldName.Substring(0, length - 2);
					}

					return action;
				});
			}
		}

		/// <summary>
		/// A filter that:
		/// <para>If the property name starts with an underscore, for example <c>{_Ignore}</c>, its value will not be included as a json property.</para>
		/// <para>If the property name also ends with an underscore, for example <c>{_Ignore_}</c>, it will be removed from the message template as well</para>
		/// </summary>
		public static PropertyFilter ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;
					if(action.MessageTemplateAction.HasValue) return action;

					var fieldName = action.PropertyName;
					var length = fieldName.Length;
					if(length < 2) return action;
					var startsWithUnderscore = fieldName[0] == '_';
					if(!startsWithUnderscore) return action;

					action.JsonPropertyAction=JsonPropertyAction.Exclude;

					var endsWithUnderscore = fieldName[length - 1] == '_';
					if(endsWithUnderscore)
					{
						action.PropertyName = fieldName.Substring(1, length - 2);
						action.MessageTemplateAction = MessageTemplateAction.Remove;
					}
					else action.PropertyName = fieldName.Substring(1, length - 1);
					return action;
				});
			}
		}

		/// <summary>
		/// The default handler, that<br/>
		/// - if the property name starts with <c>_</c> its value is excluded, and if it also ends with <c>_</c> it's removed from the template<br/>
		/// - if the property name starts and ends with <c>__</c> its value is excluded, and it is replaced in the template by its value<br/>
		/// - if the property names consists of only digits, for example <c>{0}</c>, its value is excluded.
		/// </summary>
		public static PropertyFilter Default
		{
			get
			{
				return InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores +
				       ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore +
							 IntPropertiesAsSharedProperty;
			}
		}
	}
}