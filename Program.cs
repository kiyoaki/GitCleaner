using static Chell.Exports;

var app = ConsoleApp.Create(args);
app.AddCommands<GitCleaner>();
app.Run();

public class GitCleaner : ConsoleAppBase
{
    public async Task<int> Branch([Option(0)] string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.Error.WriteLine("path is empty");
                return 1;
            }

            if (!Directory.Exists(path))
            {
                Console.Error.WriteLine("path not exists");
                return 1;
            }

            Directory.SetCurrentDirectory(path);

            foreach (var line in await Run("git fetch --prune"))
            {
                Console.WriteLine(line);
            }

            foreach (var line in await Run("git remote set-head origin -a"))
            {
                Console.WriteLine(line);
            }

            var defaultBranch = "";
            var remoteBranches = new List<string>();
            var localBranches = new List<string>();

            var isRemoteBranch = false;
            var isLocalBranch = false;

            foreach (var line in await Run("git remote show origin"))
            {
                Console.WriteLine(line);

                if (!string.IsNullOrEmpty(line) && line.Contains("HEAD branch:"))
                {
                    defaultBranch = line.Replace("HEAD branch:", "").Trim();
                }
                else if (!string.IsNullOrEmpty(line) && line.Contains("Remote branches"))
                {
                    isRemoteBranch = true;
                }
                else if (isRemoteBranch)
                {
                    if (!string.IsNullOrEmpty(line) && line.Contains("Local branches"))
                    {
                        isLocalBranch = true;
                        isRemoteBranch = false;
                    }
                    else
                    {
                        var branchName = line.Split(' ').Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                        if (!string.IsNullOrEmpty(branchName) && !remoteBranches.Contains(branchName))
                        {
                            remoteBranches.Add(branchName);
                        }
                    }
                }
                else if (isLocalBranch)
                {
                    if (!string.IsNullOrEmpty(line) && line.Contains("Local refs"))
                    {
                        isLocalBranch = false;
                        break;
                    }
                    else
                    {
                        var branchName = line.Split(' ').Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                        if (!string.IsNullOrEmpty(branchName) && !localBranches.Contains(branchName))
                        {
                            localBranches.Add(branchName);
                        }
                    }
                }
            }

            foreach (var remoteBranch in remoteBranches)
            {
                if (!localBranches.Contains(remoteBranch))
                {
                    foreach (var line in await Run($"git checkout {remoteBranch}"))
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            foreach (var localBranch in localBranches)
            {
                if (!remoteBranches.Contains(localBranch))
                {
                    foreach (var line in await Run($"git branch -D {localBranch}"))
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            foreach (var line in await Run($"git checkout {defaultBranch}"))
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }

        return 0;
    }

    public async Task<int> Code([Option(0)] string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.Error.WriteLine("path is empty");
                return 1;
            }

            if (!Directory.Exists(path))
            {
                Console.Error.WriteLine("path not exists");
                return 1;
            }

            Directory.SetCurrentDirectory(path);

            foreach (var line in await Run("git reset --hard"))
            {
                Console.WriteLine(line);
            }

            foreach (var line in await Run("git clean -fxd"))
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }

        return 0;
    }
}