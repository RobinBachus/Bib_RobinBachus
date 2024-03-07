namespace Bib_RobinBachus
{
	internal static class Utils
	{
		public static bool TryParse<T>(string? value, out T result, string err = "Ongeldige waarde, probeer opnieuw.") where T : struct, IParsable<T>
		{
			if (value is null)
			{
				result = default;
				Console.WriteLine("Input was leeg. Probeer opnieuw.");
				return false;
			}
			bool valid = T.TryParse(value, null, out result);
			if (!valid) Console.WriteLine(err);
			return valid;
		}

		public static bool TryParse(string? value, out string result, string err = "Input was leeg, probeer opnieuw.")
		{
			if (string.IsNullOrEmpty(value))
			{
				result = "";
				Console.WriteLine(err);
				return false;
			}

			result = value;
			return true;
		}
	}
}
