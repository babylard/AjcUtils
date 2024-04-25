using System.IO;

class AjcUtils
{
    static void Main(string[] args)
    {
        Console.WriteLine("1. | Enable Dev tools |");
        Console.WriteLine("2. | Disable Dev tools|");
        Console.WriteLine("3. | Clear Cache      |");

        string answer = Console.ReadLine();

        if (answer == "1")
        {
            Console.Clear();
            EnableDevTools();
        }

        else if (answer == "2")
        {
            Console.Clear();
            DisableDevTools();
        }

        else if (answer == "3")
        {
            ClearCache();
        }

        else if (answer == "4")
        {
            Console.WriteLine("Unimplemented");
        }

        Console.WriteLine("-------------------------------\nDone! Press Enter to Exit...");
        Console.ReadLine();
    }

    static void RemoveAsar()
    {
        string username = Environment.UserName;
        Console.WriteLine("Checking for app.asar");

        if (File.Exists(@$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app.asar"))
        {
            Console.WriteLine("Removing app.asar");
            try
            {
                File.Delete(@$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app.asar");
                Console.WriteLine("Removed app.asar");
            }

            catch
            {
                Console.WriteLine("Error occured.");
            }
        }

        else
        {
            Console.WriteLine("No asar file detected, skipping");
        }
    }

    static void EnableDevTools()
    {
        string username = Environment.UserName;
        RemoveAsar();
        Console.WriteLine("Copying modified asar with Dev tools Enabled");
        File.Copy(@"EnabledDevTools\app.asar", @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app.asar");
        Console.WriteLine("Copied modified asar");

    }

    static void DisableDevTools()
    {
        string username = Environment.UserName;
        RemoveAsar();
        Console.WriteLine("Copying modified asar with Dev tools Disabled");
        File.Copy(@"DisabledDevTools\app.asar", @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app.asar");
        Console.WriteLine("Copied modified asar");
    }

    static void ClearCache()
    {
        string username = Environment.UserName;
        string cache = @$"C:\\Users\\{username}\\AppData\\Roaming\\AJ Classic";
        Console.WriteLine("Clearing Cache");

        if (Directory.Exists(cache))
        {
            try
            {
                // Get the parent directory
                string parentDirectory = Directory.GetParent(cache).FullName;

                // Get the name of the directory to delete
                string directoryName = new DirectoryInfo(cache).Name;

                // Combine parent directory and directory name
                string directoryToDelete = Path.Combine(parentDirectory, directoryName);

                // Delete the specified directory
                Directory.Delete(directoryToDelete, true);

                Console.WriteLine("Cache cleared sucessfully");
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred: " + e.Message + "\n\n Contact us in the Discord server if you need any help!");
            }


        }

        else
        {
            Console.WriteLine("No current cache detected");
        }
    }
}