namespace Serilog.CustomJsonFormatter
{
	public interface IStringHasher
	{
		uint CalculateHash(string value);
	}
}