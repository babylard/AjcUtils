using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using AsarSharp;
using System.Net;
class AjcUtils
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. | Dev Tools                |");
            Console.WriteLine("2. | Cache clear on startup   |");
            Console.WriteLine("3. | Client auto-update       |");
            Console.WriteLine("4. | Clear Cache              |");
            Console.WriteLine("#  |--------------------------|");
            Console.WriteLine("5. | Reset all                |");

            string answer = Console.ReadLine();

            if (answer == "1")
            {
                Console.Clear();
                Console.WriteLine("1. | Enable Dev Tools    |");
                Console.WriteLine("2. | Disable Dev Tools   |");
                string answer2 = Console.ReadLine();

                if (answer2 == "1")
                {
                    Console.Clear();
                    ReplaceAsar();
                    EnableDevTools();
                }

                else if (answer2 == "2")
                {
                    Console.Clear();
                    ReplaceAsar();
                    DisableDevTools();
                }


            }

            else if (answer == "2")
            {
                Console.Clear();
                Console.WriteLine("1. | Enable Cache auto-clear  |");
                Console.WriteLine("2. | Disable Cache auto-clear |");
                string answer3 = Console.ReadLine();

                if (answer3 == "1")
                {
                    Console.Clear();
                    ReplaceAsar();
                    AutoClearEnable();
                }

                else if (answer3 == "2")
                {
                    Console.Clear();
                    ReplaceAsar();
                    AutoClearDisable();
                }

            }

            else if (answer == "3")
            {
                Console.Clear();
                Console.WriteLine("1. | Enable Client auto-update  |");
                Console.WriteLine("2. | Disable Client auto-update |");
                string answer4 = Console.ReadLine();

                if (answer4 == "1")
                {
                    Console.Clear();
                    ReplaceAsar();
                    EnableUpdates();
                }

                else if (answer4 == "2")
                {
                    Console.Clear();
                    ReplaceAsar();
                    DisableUpdates();
                }
            }

            else if (answer == "4")
            {
                Console.Clear();
                ReplaceAsar();
                ClearCache();
            }

            else if (answer == "5")
            {
                // Different method of modifying and repacking the asar if both the
                // unpacked, and packed version are present for some reason.
                if (Directory.Exists(archive) && File.Exists(asar))
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine("Deleting asar.");
                        File.Delete(asar);
                        DisableDevTools();
                        EnableUpdates();
                        AutoClearDisable();
                        ClearCache();
                        RePack();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex}");
                    }
                }
                else if (Directory.Exists(archive))
                {
                    try
                    {
                        Console.WriteLine("Resetting all to defaults.");
                        DisableDevTools();
                        EnableUpdates();
                        AutoClearDisable();
                        ClearCache();
                        RePack();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex}");
                    }
                }

                // If nothing has been modified previously, only clear Electron cache.
                else if (File.Exists(asar))
                {
                    Console.WriteLine("Asar hasn't been unpacked, only clearing cache.");
                    ClearCache();
                }
            }

            Console.WriteLine("-------------------------------\nDone! Press Enter to Exit, or M to return to menu.");
            string input = Console.ReadLine().ToLower();
            if (input.ToLower() != "m")
                break; // Exit the loop and end the program
        }
    }

    // Commonly needed paths that will be used
    static string username = Environment.UserName;
    static string asar = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app.asar";
    static string archive = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app";
    static string config = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

    static void ReplaceAsar()
    {
        if (Directory.Exists($@"C:\Users\{username}\AppData\Local\Programs\aj-classic"))
        {

            Console.WriteLine("Ajc is installed. Checking for app.asar");

            if (File.Exists(@$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app.asar"))
            {
                Console.WriteLine("App.asar exists");
                try
                {
                    Console.WriteLine("Unpacking app.asar");
                    using AsarExtractor extractor = new(asar, archive);
                    extractor.Extract();
                    extractor.Dispose();
                    if (File.Exists(asar))
                    {
                        File.Delete(asar);
                    }
                    Console.WriteLine("Unpacked app.asar");
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured: {ex.Message}");
                }

            }

            else if (Directory.Exists(archive))
            {
                Console.WriteLine("App folder already Unpacked, skipping.");
            }
            else
            {
                Console.WriteLine("No Packed/Unpacked asar detected. (is your install broken?)");
            }
        }

        else
        {
            Console.WriteLine("Aj Classic is not installed. Exiting...");
            Environment.Exit(0);
        }
    }

    static void RePack()
    {
        Console.WriteLine(" Attempting to RePack asar archive.");
        try
        {
            using AsarArchiver archiver = new(archive, asar);
            archiver.Archive();
            if (Directory.Exists(archive))
            {
                DeleteDirectory(archive);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
        }
    }
    static void EnableDevTools()
    {
        try
        {
            // Read all lines of the JavaScript file
            string[] lines = File.ReadAllLines(config);

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
            File.WriteAllLines(config, lines);

            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void DisableDevTools()
    {
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

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
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

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
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

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
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

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
        string username = Environment.UserName;
        string filePath = @$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app\config.js";

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
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
}