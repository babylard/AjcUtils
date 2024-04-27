using Newtonsoft.Json.Linq;
using System.IO;

class AjcUtils
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. | Enable Dev tools         |");
            Console.WriteLine("2. | Disable Dev tools        |");
            Console.WriteLine("3. | Enable Cache auto-clear  |");
            Console.WriteLine("4. | Disable Cache auto-clear |");
            Console.WriteLine("5. | Enable Client Updates    |");
            Console.WriteLine("6. | Disable Client Updates   |");
            Console.WriteLine("7. | Clear Cache              |");

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
                AutoClearEnable();
            }

            else if (answer == "4")
            {
                AutoClearDisable();
            }

            else if (answer == "5")
            {
                EnableUpdates();
            }

            else if (answer == "6")
            {
                DisableUpdates();
            }

            else if (answer == "7")
            {
                ClearCache();
            }

            Console.WriteLine("-------------------------------\nDone! Press Enter to Exit, or M to return to menu.");
            string input = Console.ReadLine().ToLower();
            if (input != "m")
                break; // Exit the loop and end the program
        }
    }

    static void ReplaceAsar()
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
       
        if (Directory.Exists(@$"C:\\Users\\{username}\\AppData\\Local\\aj-classic-updater\\package\\resources\\app"))
        {
            Console.WriteLine("App folder already copied, skipping.");
        }
        else
        {
            try
            {
                CopyDirectory(@"app", @$"C:\\Users\\{username}\\AppData\\Local\\aj-classic-updater\\package\\resources\\", true);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    static void EnableDevTools()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";

        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("showTools"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("false", "true");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void DisableDevTools()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";

        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("showTools"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("true", "false");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void AutoClearEnable()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";
        
        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("clearCache"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("false", "true");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void AutoClearDisable()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";

        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("clearCache"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("true", "false");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void DisableUpdates()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";

        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("noUpdater"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("false", "true");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void EnableUpdates()
    {
        ReplaceAsar();
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\aj-classic-updater\package\resources\app\config.js";

        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line containing the "showTools" property
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("noUpdater"))
                {
                    // Modify the line to set showTools to true
                    lines[i] = lines[i].Replace("true", "false");
                    break;
                }
            }

            // Write the modified lines back to the file
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
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

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        // Also, i'm really fucking tired.
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}