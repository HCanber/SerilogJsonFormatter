namespace Hcanber.Serilog.JsonFormatters
{
	public interface IStringHasher
	{
		uint CalculateHash(string value);
	}
}