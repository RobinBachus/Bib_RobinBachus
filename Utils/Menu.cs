// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

using System.Diagnostics.CodeAnalysis;
using static Bib_RobinBachus.Utils.UserInput;

namespace Bib_RobinBachus.Utils
{
	internal class Menu
	{
		private readonly Dictionary<string, Action> menuItems = new();

		private string description;

		private string[] exitOptions;

		private string prompt;
		public Dictionary<string, Action> MenuItems => menuItems;

		public string Description
		{
			get => description;
			set => description = value;
		}

		public string Prompt
		{
			get => prompt;
			set => prompt = value;
		}

		public Menu(string description, string prompt, string[] exitOptions)
		{
			this.description = description;
			this.prompt = prompt;
			this.exitOptions = exitOptions;
		}


		/// <summary>
		/// Adds a menu item to the menu.
		/// </summary>
		/// <param name="name">The name of the menu item.</param>
		/// <param name="action">The action to be performed when the menu item is selected.</param>
		/// <exception cref="ArgumentException">Thrown when the menu item already exists.</exception>
		public void AddMenuItem(string name, Action action)
		{
			if (menuItems.ContainsKey(name)) throw new ArgumentException("Menu item already exists", nameof(name));
			menuItems.Add(name, action);
		}

		public void AddExitOption(string option)
		{
			exitOptions = exitOptions.Append(option).ToArray();
		}

		public bool ShowMenu(bool promptKey = false)
		{
			Console.WriteLine(Description);
			int i = 1;
			foreach ((string? option, _) in menuItems) Console.WriteLine($"{i++}. {option}");

			string exitsStrings = string.Join(", ", exitOptions);
			Console.WriteLine($"\nGebruik {exitsStrings} om te stoppen\n");

			(_, int? parsed) = PromptRange(Prompt, 1, menuItems.Count, exitOptions);
			Console.Clear();
			if (parsed is null) return false;
			
			menuItems.Values.ElementAt(parsed.Value - 1).Invoke();

			if (!promptKey) return true;

			PromptKey();
			Console.Clear();

			return true;
		}


		// ReSharper disable once RedundantAssignment
		// CancellationToken's initial value is not important, but it can be changed by the Action invoked by the menu item
		public bool ShowMenu(ref bool cancellationToken, bool promptKey = false)
		{
			cancellationToken = true;

			Console.WriteLine(Description);
			int i = 1;
			foreach ((string? option, _) in menuItems) Console.WriteLine($"{i++}. {option}");

			string exitsStrings = string.Join(", ", exitOptions);
			Console.WriteLine($"\nGebruik {exitsStrings} om te stoppen\n");

			(_, int? parsed) = PromptRange(Prompt, 1, menuItems.Count, exitOptions);
			Console.Clear();
			if (parsed is null) return false;

			menuItems.Values.ElementAt(parsed.Value - 1).Invoke();

			if (!promptKey) return cancellationToken;

			PromptKey();
			Console.Clear();

			return cancellationToken;
		}
	}
}