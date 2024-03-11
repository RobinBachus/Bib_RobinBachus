using static Bib_RobinBachus.Utils.Utils;

namespace Bib_RobinBachus.Utils
{
	/// <summary>
	///     Utility class containing various helper methods for user input.
	/// </summary>
	internal static class UserInput
	{
		/// <summary>
		///     Prompts the user for a value of type T and returns the parsed value.
		///     If the user's input is not parseable to type T, it prompts the user again.
		/// </summary>
		/// <typeparam name="T">The type of the value to parse. Must be a struct and implement IParsable.</typeparam>
		/// <param name="prompt">The prompt message to display to the user.</param>
		/// <param name="err">The error message to display if the user's input is not parseable to type T.</param>
		/// <returns>The parsed value of type T.</returns>
		public static T Prompt<T>(string prompt, string err = "Ongeldige waarde, probeer opnieuw.")
			where T : struct, IParsable<T>
		{
			bool exit = false;
			T parsed = default;

			while (!exit)
			{
				Console.Write(prompt);
				exit = TryParse(Console.ReadLine(), out parsed, err);
			}

			return parsed;
		}

		/// <summary>
		///     Prompts the user for a string value and returns the parsed value.
		///     If the user's input is empty, it prompts the user again.
		/// </summary>
		/// <param name="prompt">The prompt message to display to the user.</param>
		/// <param name="err">The error message to display if the user's input is empty.</param>
		/// <returns>The parsed string value.</returns>
		public static string Prompt(string prompt, string err = "Input was leeg, probeer opnieuw.")
		{
			bool exit = false;
			string parsed = "";

			while (!exit)
			{
				Console.Write(prompt);
				exit = TryParse(Console.ReadLine(), out parsed, err);
			}

			return parsed;
		}

		/// <summary>
		///     Prompts the user for a value within a specified range. If the user's input is not within the range,
		///     it prompts the user again.
		/// </summary>
		/// <typeparam name="T">The type of the value to parse. Must be a struct, implement IParsable, and IComparable.</typeparam>
		/// <param name="prompt">The prompt message to display to the user.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <param name="err">The error message to display if the user's input is not within the range.</param>
		/// <returns>The parsed value within the specified range.</returns>
		public static T PromptRange<T>(string prompt, T min, T max,
			string err = "Waarde buiten range, probeer opnieuw.")
			where T : struct, IParsable<T>, IComparable<T>
		{
			T parsed;
			bool initial = true;
			do
			{
				if (!initial) Console.WriteLine(err);
				parsed = Prompt<T>($"{prompt} [{min}-{max}]: ");
				initial = false;
			} while (parsed.CompareTo(min) < 0 || parsed.CompareTo(max) > 0);

			return parsed;
		}

		/// <summary>
		///     Prompts the user for a value within a specified range. If the user's input is not parseable to the type T,
		///     it checks if the input is within the allowed values. If it is, it returns the input value and null for parsed
		///     value.
		///     If it's not within the allowed values, it prompts the user again.
		/// </summary>
		/// <typeparam name="T">The type of the value to parse. Must be a struct, implement IParsable, and IComparable.</typeparam>
		/// <param name="prompt">The prompt message to display to the user.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <param name="allowedValues">An array of allowed string values.</param>
		/// <param name="err">The error message to display if the user's input is not within the range or the allowed values.</param>
		/// <returns>
		///     A tuple containing the user's input value and the parsed value. If the input is not parseable to the type T
		///     but is within the allowed values, the parsed value is null.
		/// </returns>
		public static (string value, T? parsed) PromptRange<T>
		(
			string prompt,
			T min,
			T max,
			string[] allowedValues,
			string err = "Waarde buiten range, probeer opnieuw."
		) where T : struct, IParsable<T>, IComparable<T>
		{
			T parsed;
			string value;
			bool initial = true;
			do
			{
				if (!initial) Console.WriteLine(err);

				value = Prompt($"{prompt} [{min}-{max}]: ");

				bool parseable = TryParse(value, out parsed, "");
				switch (parseable)
				{
					case false when allowedValues.Length > 0 && !allowedValues.Contains(value):
						Console.WriteLine(err);
						continue;
					case false when allowedValues.Contains(value):
						return (value, null);

					default:
						initial = false;
						break;
				}
			} while (parsed.CompareTo(min) < 0 || parsed.CompareTo(max) > 0);

			return (value, parsed);
		}

		/// <summary>
		///     Prompts the user for a boolean value.
		///     If the user's input is "y", "yes", "n", or "no", it returns the corresponding boolean value.
		///     If the user's input is not recognized, it returns the default value.
		/// </summary>
		/// <param name="prompt">The prompt message to display to the user.</param>
		/// <param name="def">The default value to return if the user's input is not recognized.</param>
		/// <returns>The boolean value based on the user's input.</returns>
		public static bool PromptBool(string prompt, string def = "y")
		{
			prompt += def == "n" ? " (y/N):" : " (Y/n):";
			Console.Write(prompt);
			string input = Console.ReadLine() ?? def;
			return input.ToLower() switch
			{
				"y" => true,
				"yes" => true,
				"n" => false,
				"no" => false,
				_ => def == "y"
			};
		}

		// TODO: make cancelable
		/// <summary>
		///     Prompts the user for an ISBN value.
		///     If the user's input is not a valid ISBN, it prompts the user again.
		/// </summary>
		/// <returns>The parsed ISBN value.</returns>
		public static string? PromptIsbn()
		{
			bool exit = false;
			string parsed = "";

			while (!exit)
			{
				parsed = Prompt("ISBN: ");

				(exit, string? errorMsg) = Book.IsValidIsbn(parsed);
				if (!exit) Console.WriteLine(errorMsg);
			}

			return parsed;
		}

		/// <summary>
		///     Prompts the user to press any key to continue.
		/// </summary>
		public static void PromptKey()
		{
			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey();
			Console.Clear();
		}

		/// <summary>
		///     Prompts the user for a file path.
		///     If the default path is provided and the user chooses to use it, the default path is returned.
		///     If the default path is not provided or the user chooses not to use it, the user is prompted to enter a new file
		///     path.
		///     If the file does not exist, the user is prompted to create a new file.
		/// </summary>
		/// <param name="defaultPath">The default file path to use.</param>
		/// <returns>The file path entered by the user or the default path if chosen.</returns>
		public static string? PromptPath(string? defaultPath = null)
		{
			string? path = null;

			if (!string.IsNullOrEmpty(defaultPath))
				path = PromptBool($"Gebruik standaard CSV? ({Path.GetFullPath(defaultPath)})")
					? defaultPath
					: null;

			path ??= Prompt("Pad van CSV: ");
			path += path.EndsWith(".csv") ? "" : ".csv";

			if (File.Exists(path)) return path;

			bool createFile =
				PromptBool($"Bestand niet gevonden, wil je een nieuw bestand aanmaken? ({Path.GetFullPath(path)})");
			if (!createFile)
			{
				Console.WriteLine("IO operatie geannuleerd");
				PromptKey();
				return null;
			}

			Console.WriteLine("Nieuwe file aangemaakt");
			File.Create(path).Close();

			return path;
		}
	}
}