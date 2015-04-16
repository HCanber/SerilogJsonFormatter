using Serilog.CustomJsonFormatter.Filter;
using Xunit;

namespace Serilog.CustomJsonFormatter.Tests
{
	public class PropertyFiltersTests
	{
		[Fact]
		public void ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore_Ignore_those_that_do_not_start_with_underscore()
		{
			var a = PropertyFilters.ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore.Filter(new PropertyAction("Foo_"));
			a.JsonPropertyAction.ShouldBe(null);
			a.MessageTemplateAction.ShouldBe(null);
			a.OriginalPropertyName.ShouldBe("Foo_");
			a.PropertyName.ShouldBe("Foo_");
		}
		[Fact]
		public void ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore_Exclude_those_that_starts_with_underscore()
		{
			var a = PropertyFilters.ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore.Filter(new PropertyAction("_Foo"));
			a.JsonPropertyAction.ShouldBe(JsonPropertyAction.Exclude);
			a.MessageTemplateAction.ShouldBe(null);
			a.OriginalPropertyName.ShouldBe("_Foo");
			a.PropertyName.ShouldBe("Foo");
		}

		[Fact]
		public void ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore_Exclude_andRemove_those_that_starts_and_ends_with_underscore()
		{
			var a = PropertyFilters.ExcludePropertiesThatStartsWithUnderscoreRemoveFromTemplateIfEndsWithUnderscore.Filter(new PropertyAction("_Foo_"));
			a.JsonPropertyAction.ShouldBe(JsonPropertyAction.Exclude);
			a.MessageTemplateAction.ShouldBe(MessageTemplateAction.Remove);
			a.OriginalPropertyName.ShouldBe("_Foo_");
			a.PropertyName.ShouldBe("Foo");
		}

		[Fact]
		public void InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores_Ignore_those_that_do_not_start_or_ends_with_double_underscores()
		{
			var a = PropertyFilters.InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores.Filter(new PropertyAction("Foo"));
			a.JsonPropertyAction.ShouldBe(null);
			a.MessageTemplateAction.ShouldBe(null);
			a.OriginalPropertyName.ShouldBe("Foo");
			a.PropertyName.ShouldBe("Foo");
		}


		[Fact]
		public void InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores_Exclude_those_that_starts_with_double_underscores()
		{
			var a = PropertyFilters.InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores.Filter(new PropertyAction("__Foo"));
			a.JsonPropertyAction.ShouldBe(JsonPropertyAction.Exclude);
			a.MessageTemplateAction.ShouldBe(null);
			a.OriginalPropertyName.ShouldBe("__Foo");
			a.PropertyName.ShouldBe("Foo");
		}

		[Fact]
		public void InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores_RenderValue_those_that_ends_with_double_underscores()
		{
			var a = PropertyFilters.InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores.Filter(new PropertyAction("Foo__"));
			a.JsonPropertyAction.ShouldBe(null);
			a.MessageTemplateAction.ShouldBe(MessageTemplateAction.RenderValue);
			a.OriginalPropertyName.ShouldBe("Foo__");
			a.PropertyName.ShouldBe("Foo");
		}


		[Fact]
		public void InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores_Exclude_and_RenderValue_those_that_starts_and_ends_with_double_underscores()
		{
			var a = PropertyFilters.InlinePropertiesInTemplateThatStartsAndEndsWithDoubleUnderscores.Filter(new PropertyAction("__Foo__"));
			a.JsonPropertyAction.ShouldBe(JsonPropertyAction.Exclude);
			a.MessageTemplateAction.ShouldBe(MessageTemplateAction.RenderValue);
			a.OriginalPropertyName.ShouldBe("__Foo__");
			a.PropertyName.ShouldBe("Foo");
		}
	}
}