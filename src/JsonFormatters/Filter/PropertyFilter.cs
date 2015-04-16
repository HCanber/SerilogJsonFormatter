namespace Hcanber.Serilog.JsonFormatters.Filter
{
	public abstract class PropertyFilter
	{
		public abstract PropertyAction Filter(PropertyAction propertyAction);

		public static PropertyFilter operator +(PropertyFilter filterFirst, PropertyFilter filterAfter)
		{
			return new CombinedFilter(filterFirst, filterAfter);
		}

		public static PropertyFilter operator |(PropertyFilter filterFirst, PropertyFilter filterAfter)
		{
			return new CombinedFilter(filterFirst, filterAfter);
		}

		private class CombinedFilter : PropertyFilter
		{
			private readonly PropertyFilter _first;
			private readonly PropertyFilter _second;

			public CombinedFilter(PropertyFilter first, PropertyFilter second)
			{
				_first = first;
				_second = second;
			}

			public override PropertyAction Filter(PropertyAction propertyAction)
			{
				return _second.Filter(_first.Filter(propertyAction));
			}
		}
	}
}