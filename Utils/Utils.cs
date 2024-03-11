using System.Diagnostics;

namespace Bib_RobinBachus.Utils
{
	/// <summary>
	/// Utility class containing various helper methods.
	/// </summary>
	internal static class Utils
	{
		/// <summary>
		/// Tries to parse the specified value into a strongly-typed result.
		/// </summary>
		/// <typeparam name="T">The type of the result.</typeparam>
		/// <param name="value">The value to parse.</param>
		/// <param name="result">When this method returns, contains the parsed result if the parsing was successful, or the default value of <typeparamref name="T"/> if the parsing failed.</param>
		/// <param name="err">The error message to display if the parsing fails.</param>
		/// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Tries to parse the specified value into a string result.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <param name="result">When this method returns, contains the parsed result if the parsing was successful, or an empty string if the parsing failed.</param>
		/// <param name="err">The error message to display if the parsing fails.</param>
		/// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Opens the specified file path using the default program associated with the file type.
		/// <para> Source: <see href="https://stackoverflow.com/a/54275102"/> </para>
		/// </summary>
		/// <param name="path">The path of the file to open.</param>
		public static void OpenWithDefaultProgram(string path)
		{
			using Process fileopener = new();

			fileopener.StartInfo.FileName = "explorer";
			fileopener.StartInfo.Arguments = "\"" + path + "\"";
			fileopener.Start();
		}

		/// <summary>
		/// Capitalizes the first letter of the specified string.
		/// </summary>
		/// <param name="value">The string to capitalize.</param>
		/// <returns>The string with the first letter capitalized.</returns>
		public static string CapitalizeFirst(this string value)
		{
			return value[..1].ToUpper() + value[1..].ToLower();
		}

		/// <summary>
		/// Capitalizes the first letter of each word in the specified string.
		/// </summary>
		/// <param name="value">The string to capitalize.</param>
		/// <returns>The string with the first letter of each word capitalized.</returns>
		public static string CapitalizeAll(this string value)
		{
			string[] words 
				= value
				.Split(" ")
				.Select(word => word.CapitalizeFirst())
				.ToArray();
			return string.Join(" ", words);
		}
	}
}
