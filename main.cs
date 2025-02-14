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

            switch (answer)
            {
                case "1":
                    HandleDevTools();
                    break;
                case "2":
                    HandleCacheClear();
                    break;
                case "3":
                    HandleAutoUpdate();
                    break;
                case "4":
                    Console.Clear();
                    ReplaceAsar();
                    HandleCacheClear();
                    break;
                case "5":
                    ResetAll();
                    break;
                default:
                    break;
            }

            Console.WriteLine("-------------------------------\nDone! Press Enter to Exit, or M to return to menu.");
            if (Console.ReadLine().ToLower() != "m")
                break; // Exit the loop and end the program
        }
    }

    static void HandleDevTools()
    {
        Console.Clear();
        Console.WriteLine("1. | Enable Dev Tools    |");
        Console.WriteLine("2. | Disable Dev Tools   |");
        string answer2 = Console.ReadLine();

        Console.Clear();
        ReplaceAsar();
        if (answer2 == "1")
        {
            EnableDevTools();
        }
        else if (answer2 == "2")
        {
            DisableDevTools();
        }
    }

    static void HandleAutoUpdate()
    {
        Console.Clear();
        Console.WriteLine("1. | Enable Client auto-update  |");
        Console.WriteLine("2. | Disable Client auto-update |");
        string answer4 = Console.ReadLine();

        Console.Clear();
        ReplaceAsar();
        if (answer4 == "1")
        {
            EnableUpdates();
        }
        else if (answer4 == "2")
        {
            DisableUpdates();
        }
    }

    static void HandleCacheClear()
    {
        Console.Clear();
        Console.WriteLine("1. | Clear Aj Classic Cache |");
        Console.WriteLine("2. | Clear Jam Cache        |");
        Console.WriteLine("3. | Clear Both Caches      |");
        string answer5 = Console.ReadLine();
        Console.Clear();
        if (answer5 == "1")
        {
            ClearCache(1);
        }
        else if (answer5 == "2")
        {
            ClearCache(2);
        }
        else if (answer5 == "3")
        {
            ClearCache(3);
        }
    }

    static void ResetAll()
    {
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
                ClearCache(1);
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
                ClearCache(1);
                RePack();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }
        else if (File.Exists(asar))
        {
            Console.WriteLine("Asar hasn't been unpacked, only clearing cache.");
            ClearCache(1);
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
                    Console.WriteLine($"Error occurred: {ex.Message}");
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
        Console.WriteLine("Attempting to RePack asar archive.");
        try
        {
            using AsarArchiver archiver = new(archive, asar);
            archiver.Archive();
            archiver.Dispose();
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
        ModifyConfig("showTools", "false", "true");
    }

    static void DisableDevTools()
    {
        ModifyConfig("showTools", "true", "false");
    }

    static void AutoClearEnable()
    {
        ModifyConfig("clearCache", "false", "true");
    }

    static void AutoClearDisable()
    {
        ModifyConfig("clearCache", "true", "false");
    }

    static void DisableUpdates()
    {
        ModifyConfig("noUpdater", "false", "true");
    }

    static void EnableUpdates()
    {
        ModifyConfig("noUpdater", "true", "false");
    }

    static void ClearCache(int JamOrClassic)
    {
        string ClassicCache = @$"C:\Users\{username}\AppData\Roaming\AJ Classic";
        string JamCache = @$"C:\Users\{username}\AppData\Roaming\jam";
        Console.WriteLine("Clearing Cache");

        switch (JamOrClassic)
        {
            case 1: // Deletes Aj Classic Cache
                if (Directory.Exists(ClassicCache))
                {
                    try
                    {
                        Directory.Delete(ClassicCache, true);
                        Console.WriteLine("Cache cleared successfully");
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"An error occurred: {e}");
                    }
                }
                else
                {
                    Console.WriteLine("Ajc not installed or Cache is already cleared.");
                }
                break;

            case 2: // Deletes Jam Cache (if installed)
                if (Directory.Exists(JamCache))
                {
                    try
                    {
                        Directory.Delete(JamCache, true);
                        Console.WriteLine("Cache cleared successfully");
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"An error occurred: {e}");
                    }
                }
                else
                {
                    Console.WriteLine("Jam is not installed or Cache is already cleared.");
                }
                break;
            case 3: // Deletes both caches
                if (Directory.Exists(ClassicCache) && Directory.Exists(JamCache))
                {
                    try
                    {
                        Directory.Delete(ClassicCache, true);
                        Directory.Delete(JamCache, true);
                        Console.WriteLine("Cache cleared successfully");
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"An error occurred: {e}");
                    }
                }
                else
                {
                    Console.WriteLine("No current cache detected or Jam/Ajc is not installed");
                }
                break;

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

    static void ModifyConfig(string key, string oldValue, string newValue)
    {
        try
        {
            string[] lines = File.ReadAllLines(config);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(key))
                {
                    lines[i] = lines[i].Replace(oldValue, newValue);
                    break;
                }
            }
            File.WriteAllLines(config, lines);
            Console.WriteLine("JavaScript file modified successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
