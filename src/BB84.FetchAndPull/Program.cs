﻿using System.Diagnostics;

internal sealed class Program
{
	private const int ERROR_BAD_ARGUMENTS = 0xA0;

	private static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Please provide a path to some git repositories.");
			Environment.Exit(ERROR_BAD_ARGUMENTS);
		}

		string pathToUse = args[0];

		if (Directory.Exists(pathToUse).Equals(false))
		{
			Console.WriteLine("Please provide a path to some git repositories.");
			Environment.Exit(ERROR_BAD_ARGUMENTS);
		}

		string[] directories = Directory.GetDirectories(pathToUse, "*.*", SearchOption.TopDirectoryOnly);

		foreach (string directory in directories)
		{
			WriteLineColored($"Working with: '{directory}'", ConsoleColor.Red);
			WriteLineColored($"Start fetching..", ConsoleColor.Red);
			RunGit(directory, "fetch");
			WriteLineColored($"Done fetching..", ConsoleColor.Red);
			WriteLineColored($"Start pulling..", ConsoleColor.Red);
			RunGit(directory, "pull");
			WriteLineColored($"Done pulling..", ConsoleColor.Red);
		}

		Environment.Exit(0);
	}

	private static void RunGit(string workingDirectory, string argument)
	{
		Process process = new();
		process.StartInfo.WorkingDirectory = workingDirectory;
		process.StartInfo.FileName = "git.exe";
		process.StartInfo.Arguments = argument;
		process.Start();
		process.WaitForExit();
	}

	private static void WriteLineColored(string text, ConsoleColor color)
	{
		ConsoleColor colorBefore = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.WriteLine(text);
		Console.ForegroundColor = colorBefore;
	}
}