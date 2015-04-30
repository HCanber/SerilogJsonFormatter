using System;

namespace Serilog.CustomJsonFormatter.Filter
{
	public static class PropertyFilters
	{
		/// <summary>A handler that inlines all properties.</summary>
		public static PropertyFilter AllPropertiesInlined
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;

					action.JsonPropertyAction = JsonPropertyAction.Inline;
					return action;
				});

			}
		}

		/// <summary>A handler that puts all properties under a shared property.</summary>
		public static PropertyFilter AllPropertiesUnderSharedProperty
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;

					action.JsonPropertyAction = JsonPropertyAction.AsSharedProperty;
					return action;
				});
			}
		}

		/// <summary>A filter that puts all properties that have integer names, for example <c>{0}</c> and <c>{1}</c> under the shared property.</summary>
		public static PropertyFilter IntPropertiesUnderSharedProperty
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;

					int val;
					if(int.TryParse(action.PropertyName, out val))
						action.JsonPropertyAction = JsonPropertyAction.AsSharedProperty;
					return action;
				});
			}
		}

		/// <summary>A filter that excludes all properties that have integer names, for example <c>{0}</c> and <c>{1}</c>.</summary>
		public static PropertyFilter IntPropertiesExcluded
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					if(action.JsonPropertyAction.HasValue) return action;

					int val;
					if(int.TryParse(action.PropertyName, out val))
						action.JsonPropertyAction = JsonPropertyAction.Exclude;
					return action;
				});
			}
		}

		/// <summary>
		/// Excludes the values for properties with the specified name.
		/// By default the property will be replaced in the message template as well.
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

		/// <summary>
		/// Excludes the values for properties with the specified name.
		/// By default the property will be replaced in the message template as well.
		/// Use <paramref name="messageTemplateAction"/> to specify other behaviors.
		/// </summary>
		/// <param name="messageTemplateAction">OPTIONAL: The message template action. You can either leave the property as is in the message template, i.e. keep {TheProperty}, remove it, or replace it with the rendered value. Default: <see cref="MessageTemplateAction.RenderValue"/></param>
		/// <returns>The property filter.</returns>
		public static PropertyFilter ExcludePropertiesStartingWithUnderscore(MessageTemplateAction? messageTemplateAction = MessageTemplateAction.RenderValue)
		{
			return new DelegatePropertyFilter(action =>
			{
				var propertyName = action.PropertyName;
				if(!action.JsonPropertyAction.HasValue && propertyName[0] == '_')
				{
					action.JsonPropertyAction = JsonPropertyAction.Exclude;
					if(messageTemplateAction != null)
						action.MessageTemplateAction = messageTemplateAction;
				}
				return action;
			});
		}


		/// <summary>
		/// A filter that:
		/// <para>If the property name starts with double underscores, for example <c>{__Exclude}</c>, its value will not be included as a json property.</para>
		/// <para>If the property name ends with double underscores, for example <c>{InlineValue__}</c>, its value will be inlined in the message template</para>
		/// <para>If the property name both starts and ends with double underscores, for example <c>{__ExcludeAndInlineValue__}</c>, its value will not be included as a json property but its value will be inlined in the message template.</para>
		/// </summary>
		internal static PropertyFilter InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores	//Is internal because I'm unsure if we really want this. /Håkan
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
		internal static PropertyFilter ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore //Is internal because I'm unsure if we really want this /Håkan
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
		/// A filter that converts all property names to camel case, so "MyValue" will become "myValue"
		/// </summary>
		public static PropertyFilter CamelCasePropertyNames
		{
			get
			{
				return new DelegatePropertyFilter(action =>
				{
					var propertyName = action.PropertyName[0];
					action.PropertyName = char.ToLowerInvariant(propertyName) + action.PropertyName.Substring(1);
					return action;
				});
			}
		}

		/// <summary>
		/// The default handler, that<br/>
		/// - if the property names consists of only digits, for example <c>{0}</c>, its value is excluded.<br/>
		/// - all other properties are inline and placed directly under the root in the resulting json object.<br/>
		/// </summary>
		public static PropertyFilter Default
		{
			get
			{
				return IntPropertiesExcluded + AllPropertiesInlined;
			}
		}
	}
}