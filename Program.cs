using System;

namespace Bib_RobinBachus
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			Library library = new("Infernal Forest");
			Book.LoadFromCsv(@"C:\Users\robin\Desktop\Projects\Bib_RobinBachus\Data\Books.csv");

			bool exit;
			do exit = ShowMenu(library);
			while (!exit);
		}

		/// <summary>
		/// Show the menu and handle the user's choice
		/// </summary>
		/// <param name="library">The current library object</param>
		/// <returns>Exit state: true for exit, otherwise false</returns>
		private static bool ShowMenu(Library library)
		{
			Console.WriteLine($"Welkom bij {library.Name}!");
			Console.WriteLine("Wat wil je doen?\n");

			Console.WriteLine("1. Voeg een boek toe");
			Console.WriteLine("2. Voeg info toe aan een boek");
			Console.WriteLine("3. Toon alle info van een boek");
			Console.WriteLine("4. Zoek een boek op");
			Console.WriteLine("5. Verwijder een boek");
			Console.WriteLine("6. Toon alle boeken");
			Console.WriteLine("0. Stoppen");

			int choice = Prompt<int>("\nKeuze: ", "Ongeldige keuze. Probeer opnieuw.");
			Console.Clear();

			switch (choice)
			{
				case 1:
					bool exit = false;
					while (!exit)
						exit = MakeBook();
					break;
				case 2:
					AddInfoToBook();
					break;
				case 3:
					Console.WriteLine(FindBook());
					break;
				case 4:
					SearchBook();
					break;
				case 5:
					ulong isbn = Prompt<ulong>("ISBN: ", "Ongeldige ISBN. Probeer opnieuw.");
					Console.WriteLine(Library.RemoveBook(isbn) ? "Boek verwijderd." : "Boek niet gevonden.");
					break;
				case 6:
					Library
						.Books
						.ForEach(b => Console.WriteLine($"{b.Title} - {b.Author}"));
					break;
				case 0:
					return true;
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
			Console.Clear();

			return false;
		}

		private static void SearchBook()
		{
			Console.WriteLine("Waarop wil je zoeken?");
			Console.WriteLine("1. ISBN");
			Console.WriteLine("2. Auteur");
			Console.WriteLine("3. Prijs range");

			if (!int.TryParse(Console.ReadLine(), out int choice))
			{
				Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
				return;
			}

			switch (choice)
			{
				case 1:
					ulong isbn = Prompt<ulong>("ISBN: ", "Ongeldige ISBN. Probeer opnieuw.");
					Book? book = Library.FindBook(isbn);
					if (book is null)
					{
						Console.WriteLine("Boek niet gevonden.");
						return;
					}

					Console.WriteLine(book.Header);
					break;

				case 2:
					string author = Prompt("Auteur: ");

					List<Book> books = Library.FindBooks(author);
					if (books.Count == 0)
					{
						Console.WriteLine("Geen boeken gevonden");
						return;
					}

					books.ForEach(ShowHeaders);
					break;

				case 3:
					double minPrice = Prompt<double>("Minimumprijs: ", "Ongeldige prijs. Probeer opnieuw.");
					double maxPrice = Prompt<double>("Maximumprijs: ", "Ongeldige prijs. Probeer opnieuw.");

					books = Library.FindBooks(minPrice, maxPrice);
					if (books.Count == 0)
					{
						Console.WriteLine("Geen boeken gevonden");
						return;
					}

					books.ForEach(ShowHeaders);
					break;

				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}


			return;
			
			static void ShowHeaders(Book b) => Console.WriteLine(b.Header);
		}

		private static void AddInfoToBook()
		{
			Book book = FindBook();
			Console.WriteLine("Wat wil je toevoegen?");
			Console.WriteLine("1. Uitgeefdatum");
			Console.WriteLine("2. Genre");
			Console.WriteLine("3. Type");

			switch (Console.ReadLine())
			{
				case "1":
					if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
					{
						Console.WriteLine("Ongeldige datum. Probeer opnieuw.");
						break;
					}

					book.PublishDate = date;
					break;
				case "2":
					Console.WriteLine("Kies een genre:");
					foreach (Genre genre in Enum.GetValues(typeof(Genre)))
						Console.WriteLine($"{(int)genre}. {genre}");

					if (!int.TryParse(Console.ReadLine(), out int choice))
					{
						Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
						break;
					}

					book.Genre = (Genre)choice;
					break;
				case "3":
					Console.WriteLine("Kies een type:");
					Console.WriteLine("1. Hardcover");
					Console.WriteLine("2. Paperback");
					Console.WriteLine("3. Kindle");

					if (!int.TryParse(Console.ReadLine(), out int type))
					{
						Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
						break;
					}

					book.Type = type switch
					{
						1 => "Hardcover",
						2 => "Paperback",
						3 => "Kindle",
						_ => "Hardcover"
					};
					break;
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}
		}

		private static bool MakeBook()
		{

			ulong isbn = Prompt<ulong>("ISBN: ", "Ongeldige ISBN. Probeer opnieuw.");
			string title = Prompt("Titel: ");
			string author = Prompt("Auteur: ");
			double price = Prompt<double>("Prijs: ", "Ongeldige prijs. Probeer opnieuw.");

			Book book = new(isbn, title, author, price); // Adds to library in the constructor

			Console.WriteLine("Book added!\n");
			Console.WriteLine(book);
			return true;
		}
		
		private static Book FindBook()
		{
			Book? book = null;
			while (book is null){
				string title = Prompt("Titel: ");
				string author = Prompt("Auteur: ");

				book = Library.FindBook(author, title);
			}

			return book;
		}

		private static bool TryParse<T>(string? value, out T result, string err = "Ongeldige waarde, probeer opnieuw.") where T : struct, IParsable<T>
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

		private static bool TryParse(string? value, out string result, string err = "Input was leeg, probeer opnieuw.")
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

		private static T Prompt<T>(string prompt, string err = "Ongeldige waarde, probeer opnieuw.") where T : struct, IParsable<T>
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

		private static string Prompt(string prompt, string err = "Input was leeg, probeer opnieuw.")
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

		private static ulong? PromptIsbn()
		{
			bool exit = false;
			ulong parsed = 0;

			while (!exit)
			{
				Console.Write("ISBN: ");
				exit = TryParse(Console.ReadLine(), out parsed);
			}

			return parsed;
		}
	}
}