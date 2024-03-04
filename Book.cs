using System.Globalization;
using System.Reflection;
// ReSharper disable ConvertToAutoProperty

namespace Bib_RobinBachus
{
	internal class Book
	{

		private ulong isbnNumber;
		public ulong IsbnNumber
		{
			get => isbnNumber;
			set
			{
				if (!IsValidIsbn(value))
				{
					isbnNumber = 0;
					Console.WriteLine($"Invalid ISBN. Keeping current value: {isbnNumber}");
					return;
				}
				isbnNumber = value;
			}
		}


		private string title = "Undefined";
		public string Title
		{
			get => title;
			set => StringPropertyNotEmpty(value, nameof(Title), ref title);
		}


		private string author = "Undefined";
		public string Author
		{
			get => author;
			set => StringPropertyNotEmpty(value, nameof(Author), ref author);
		}

		private double price;
		public double Price
		{
			get => price;
			set
			{
				if (value < 0)
					Console.WriteLine("Price can't be negative");
				else price = value;
			}
		}

		private bool hasMatureRating = false;
		public bool HasMatureRating
		{
			get => hasMatureRating;
			set => hasMatureRating = value;
		}

		private DateTime publishDate = DateTime.MaxValue;
		public DateTime PublishDate
		{
			get => publishDate;
			set => publishDate = value;
		}

		private Genre genre = Genre.Misc;
		public Genre Genre
		{
			get => genre;
			set => genre = value;
		}

		private string type = "Hardcover";
		public string Type
		{
			get => type;
			set => type = value;
		}

		public string Header => $"{Title} by {Author} ({Type})";

		public Book(ulong isbn, string title, string author, double price)
		{
			IsbnNumber = isbn;
			Title = title;	
			Author = author;
			Price = price;

			Library.Books.Add(this);
		}

		public static List<Book> LoadFromCsv(string filePath)
		{
			string[] lines = File.ReadAllLines(filePath);

			List<Book> books = lines
				.Select(row => row.Split(';'))
				.Select(columnValues => new Book(
					ulong.Parse(columnValues[0]),
					columnValues[1],
					columnValues[2],
					double.Parse(columnValues[3], new CultureInfo("en-US"))
				)
				{
					PublishDate = DateTime.Parse(columnValues[4]),
					Genre = Enum.Parse<Genre>(columnValues[5].Replace(" ", string.Empty)),
					Type = columnValues[6],
					HasMatureRating = bool.Parse(columnValues[7]),
				})
				.ToList();
			return books;
		}

		public override string ToString()
		{
			return GetType().GetProperties().Aggregate(string.Empty, Predicate);

			string Predicate(string current, PropertyInfo propertyInfo)
				=> current + $"{propertyInfo.Name,-15} {"-",-2} {propertyInfo.GetValue(this)}\n";
		}

		private static void StringPropertyNotEmpty(string value, string propertyName, ref string property)
		{
			if (value is "")
				Console.WriteLine($"{propertyName} cannot be empty. Keeping current value: '{property}'");
			else property = value;
		}

		public static bool IsValidIsbn(ulong isbn)
		{
			int digits = (int)Math.Floor(Math.Log10(isbn) + 1);
			if (digits is not (10 or 13)) return false;

			return digits == 10 ? Isbn10ChecksumValid(isbn) : Isbn13ChecksumValid(isbn);
		}

		// https://en.wikipedia.org/wiki/ISBN#ISBN-13_check_digit_calculation
		public static bool Isbn13ChecksumValid(ulong isbn)
		{
			string isbnString = isbn.ToString();
			int sum = 0;

			for (int i = 0; i < 13; i++)
			{
				int digit = int.Parse(isbnString[i].ToString());
				sum += (i % 2 == 0) ? digit : digit * 3;
			}

			return sum % 10 == 0;
		}

		public static bool Isbn10ChecksumValid(ulong isbn)
		{
			string isbnString = isbn.ToString();
			int sum = 0;

			for (int i = 0; i < 10; i++)
			{
				int digit = int.Parse(isbnString[i].ToString());
				sum += (i + 1) * digit;
			}

			return sum % 11 == 0;
		}
	}
}