using System.CommandLine;
using System.Diagnostics;

using Octokit;

internal class Program
{
	private static async Task<int> Main(string[] args)
	{
		Option<string> userOption = new("--user")
		{
			Description = "The GitHub user whose repositories to process.",
			Required = true
		};

		Option<string> tokenOption = new("--token")
		{
			Description = "A GitHub personal access token.",
			Required = true
		};

		Option<string> assigneeOption = new("--assignee")
		{
			Description = "The GitHub user to assign the pull request to."
		};

		Option<string> workDirOption = new("--work-dir")
		{
			Description = "Working directory for cloned repositories.",
			DefaultValueFactory = _ => Path.Combine(Path.GetTempPath(), "slnx-migrator")
		};

		Option<string> branchOption = new("--branch")
		{
			Description = "The branch name to create for the migration.",
			DefaultValueFactory = _ => "feature/migrate-sln-to-slnx"
		};

#pragma warning disable IDE0028 // Simplify collection initialization
		RootCommand rootCommand = new(description: "Clones non-forked GitHub repositories and migrates .sln files to .slnx format.")
		{
			userOption,
			tokenOption,
			assigneeOption,
			workDirOption,
			branchOption
		};
#pragma warning restore IDE0028 // Simplify collection initialization

		rootCommand.SetAction(async (parseResult, cancellationToken) =>
		{
			string user = parseResult.GetValue(userOption)!;
			string token = parseResult.GetValue(tokenOption)!;
			string? assignee = parseResult.GetValue(assigneeOption);
			string workDir = parseResult.GetValue(workDirOption)!;
			string branch = parseResult.GetValue(branchOption)!;

			GitHubClient github = new(new ProductHeaderValue("SlnxMigrator"))
			{
				Credentials = new Credentials(token)
			};

			Console.WriteLine($"Fetching repositories for user '{user}'...");

			IReadOnlyList<Repository> repos = await github.Repository.GetAllForUser(user);
			List<Repository> ownRepos = [.. repos.Where(r => !r.Fork)];

			Console.WriteLine($"Found {ownRepos.Count} non-forked repositories.");

			Directory.CreateDirectory(workDir);

			foreach (Repository repo in ownRepos)
			{
				cancellationToken.ThrowIfCancellationRequested();

				string repoDir = Path.Combine(workDir, repo.Name);

				Console.WriteLine();
				Console.WriteLine($"--- Processing {repo.FullName} ---");

				// Clone
				if (Directory.Exists(repoDir))
				{
					Console.WriteLine($"  Directory already exists, pulling latest...");
					await RunProcess("git", "pull", repoDir);
				}
				else
				{
					Console.WriteLine($"  Cloning {repo.CloneUrl}...");
					await RunProcess("git", $"clone {repo.CloneUrl} {repoDir}");
				}

				// Find .sln files
				string[] slnFiles = Directory.GetFiles(repoDir, "*.sln", SearchOption.AllDirectories);

				if (slnFiles.Length == 0)
				{
					Console.WriteLine("  No .sln files found. Skipping.");
					continue;
				}

				Console.WriteLine($"  Found {slnFiles.Length} .sln file(s).");

				// Determine default branch
				string defaultBranch = repo.DefaultBranch ?? "main";

				// Create branch
				await RunProcess("git", $"checkout {defaultBranch}", repoDir);
				await RunProcess("git", $"checkout -b {branch}", repoDir);

				bool hasChanges = false;

				foreach (string slnFile in slnFiles)
				{
					string slnxFile = Path.ChangeExtension(slnFile, ".slnx");

					if (File.Exists(slnxFile))
					{
						Console.WriteLine($"  {Path.GetFileName(slnxFile)} already exists. Skipping.");
						continue;
					}

					Console.WriteLine($"  Migrating {Path.GetRelativePath(repoDir, slnFile)}...");
					int exitCode = await RunProcess("dotnet", $"sln \"{slnFile}\" migrate", repoDir);

					if (exitCode != 0)
					{
						Console.WriteLine($"  Migration failed for {Path.GetFileName(slnFile)}.");
						continue;
					}

					// Stage the new .slnx and the removal of .sln
					await RunProcess("git", $"add \"{Path.GetRelativePath(repoDir, slnxFile)}\"", repoDir);
					await RunProcess("git", $"rm \"{Path.GetRelativePath(repoDir, slnFile)}\"", repoDir);
					hasChanges = true;
				}

				if (!hasChanges)
				{
					Console.WriteLine("  No migrations performed. Skipping PR.");
					await RunProcess("git", $"checkout {defaultBranch}", repoDir);
					continue;
				}

				// Commit and push
				await RunProcess("git", "commit -m \"Migrate .sln to .slnx format\"", repoDir);
				await RunProcess("git", $"push origin {branch}", repoDir);

				// Create pull request
				Console.WriteLine("  Creating pull request...");

				NewPullRequest newPr = new("Migrate .sln to .slnx format", branch, defaultBranch)
				{
					Body = "This PR migrates legacy `.sln` solution files to the new `.slnx` format using `dotnet sln migrate`."
				};

				try
				{
					PullRequest pr = await github.PullRequest.Create(repo.Owner.Login, repo.Name, newPr);
					Console.WriteLine($"  Pull request created: {pr.HtmlUrl}");

					if (!string.IsNullOrWhiteSpace(assignee))
					{
						await github.Issue.Assignee.AddAssignees(repo.Owner.Login, repo.Name, pr.Number, new AssigneesUpdate([assignee]));
						Console.WriteLine($"  Assigned to '{assignee}'.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"  Failed to create PR: {ex.Message}");
				}
			}

			Console.WriteLine();
			Console.WriteLine("Done.");
		});

		return await rootCommand.Parse(args).InvokeAsync();

		static async Task<int> RunProcess(string fileName, string arguments, string? workingDirectory = null)
		{
			using Process process = new()
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = arguments,
					WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();

			string output = await process.StandardOutput.ReadToEndAsync();
			string error = await process.StandardError.ReadToEndAsync();

			await process.WaitForExitAsync();

			if (!string.IsNullOrWhiteSpace(output))
				Console.WriteLine($"    {output.TrimEnd()}");

			if (!string.IsNullOrWhiteSpace(error) && process.ExitCode != 0)
				Console.WriteLine($"    ERROR: {error.TrimEnd()}");

			return process.ExitCode;
		}
	}
}