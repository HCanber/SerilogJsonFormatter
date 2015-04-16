using System.Text;
using xxHashSharp;

namespace Serilog.CustomJsonFormatter
{
	public class XxHashStringHasher : IStringHasher
	{
		public uint CalculateHash(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			return xxHash.CalculateHash(bytes);
		}
	}
}