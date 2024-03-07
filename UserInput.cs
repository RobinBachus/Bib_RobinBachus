using static Bib_RobinBachus.Utils;

namespace Bib_RobinBachus
{
	internal static class UserInput
	{
		public static T Prompt<T>(string prompt, string err = "Ongeldige waarde, probeer opnieuw.") where T : struct, IParsable<T>
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

		public static T PromptRange<T>(string prompt, T min, T max, string err = "Waarde buiten range, probeer opnieuw.") where T : struct, IParsable<T>, IComparable<T>
		{
			T parsed;
			bool initial = true;
			do
			{	
				if (!initial) Console.WriteLine(err);
				parsed = Prompt<T>($"{prompt} [{min}-{max}]: ");
				initial = false;
			}
			while (parsed.CompareTo(min) < 0 || parsed.CompareTo(max) > 0);

			return parsed;
		}

		public static (string value, T? parsed) PromptRange<T>(string prompt, T min, T max, string[] allowedValues, string err = "Waarde buiten range, probeer opnieuw.") 
			where T : struct, IParsable<T>, IComparable<T>
		{
			// TODO
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
                    case false when (allowedValues.Length > 0 && !allowedValues.Contains(value)):
                        Console.WriteLine(err);
                        continue;
                    case false when allowedValues.Contains(value):
                        return (value, null);
                    default:
                        initial = false;
                        break;
                }
            }
			while (parsed.CompareTo(min) < 0 || parsed.CompareTo(max) > 0);

			return (value, parsed);
			
		}

		public static bool PromptBool(string prompt, string def = "y")
		{
			prompt += def == "n" ? " (y/N):" : " (Y/n):";
			Console.Write(prompt);
			string input = Console.ReadLine() ?? def;
			return input switch
			{
				"y" => true,
				"yes" => true,
				"n" => false,
				"no" => false,
				_ => def == "y"
			};
		}

		public static string PromptIsbn()
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

		public static void PromptKey()
		{
			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey();
			Console.Clear();
		}
	}
}
