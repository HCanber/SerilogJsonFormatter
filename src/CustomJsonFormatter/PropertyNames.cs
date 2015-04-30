using System;

namespace Serilog.CustomJsonFormatter
{
	public class PropertyNames : IEquatable<PropertyNames>
	{
		public PropertyNames()
		{
			Properties = "properties";
			Exception = "exception";
			ExceptionHash = "exceptionHash";
			Message = "message";
			MessageTemplate = "template";
			MessageTemplateHash = "templateHash";
			Level = "level";
			Timestamp = "@timestamp";
			FormattingErrors = "_formattingErrors";
		}
		/// <summary>
		/// The name of the object where non-inlined properties are written. Set this to <c>null</c> to remove this entirely from the output.
		/// Default: <c>properties</c>
		/// </summary>
		public string Properties { get; set; }

		/// <summary>
		/// The name of the property where exceptions will be written.
		/// Default: <c>exception</c>
		/// </summary>
		public string Exception { get; set; }

		/// <summary>
		/// The name of the property where a hash of the exceptions will be written.
		/// Default: <c>exception</c>
		/// </summary>
		public string ExceptionHash { get; set; }


		public string Message { get; set; }
		public string MessageTemplate { get; set; }
		public string MessageTemplateHash { get; set; }
		public string Level { get; set; }
		public string Timestamp { get; set; }

		/// <summary>
		/// The name of the property which will contain the error message if something went wrong during formatting.
		/// Default: <c>_formattingErrors</c>
		/// </summary>
		public string FormattingErrors { get; set; }

		public bool Equals(PropertyNames other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return string.Equals(Properties, other.Properties) && string.Equals(Exception, other.Exception) && string.Equals(ExceptionHash, other.ExceptionHash);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((PropertyNames) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Properties != null ? Properties.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Exception != null ? Exception.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (ExceptionHash != null ? ExceptionHash.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(PropertyNames left, PropertyNames right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PropertyNames left, PropertyNames right)
		{
			return !Equals(left, right);
		}
	}
}