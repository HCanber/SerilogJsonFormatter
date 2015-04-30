using System.Text;

namespace Serilog.CustomJsonFormatter
{
	internal static class StringBuilderExtensions
	{
		public static StringBuilder AppendDelimiter(this StringBuilder sb, string delimiter)
		{
			if(sb != null && sb.Length > 0)
				sb.Append(delimiter);
			return sb;
		}
	}
}