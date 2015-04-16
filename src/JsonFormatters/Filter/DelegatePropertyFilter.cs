using System;

namespace Hcanber.Serilog.JsonFormatters.Filter
{
	public class DelegatePropertyFilter : PropertyFilter
	{
		private readonly Func<PropertyAction, PropertyAction> _filter;

		public DelegatePropertyFilter(Func<PropertyAction,PropertyAction> filter)
		{
			_filter = filter;
		}

		public override PropertyAction Filter(PropertyAction propertyAction)
		{
			return _filter(propertyAction);
		}
	}
}