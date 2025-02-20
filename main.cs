using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using AsarSharp;
using System.Net;
class AjcUtils
{
    static void Main(string[] args)
    {
        string[] mainMenuOptions = {
            "Dev Tools",
            "Client auto-update",
            "Clear Cache",
            "Reset all",
            "Unpack",
            "Repack"
        };

        while (true)
        {
            int selectedIndex = DisplayMenu("     Select an option and press Enter", mainMenuOptions);
            HandleSelection(selectedIndex);
            Console.WriteLine("════════════════════════════════════════\nDone! Press Enter to Exit, or M to return to menu.");
            if (Console.ReadLine().ToLower() != "m")
                break; // Exit the loop and end the program
        }
    }

    static int DisplayMenu(string prompt, string[] options)
    {
        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            int menuWidth = 40; // Width of the menu box

            // Calculate padding to center the menu
            int paddingLeft = (windowWidth - menuWidth) / 2;

            string padding = new string(' ', paddingLeft);

            Console.WriteLine(padding + "╔════════════════════════════════════════╗");
            Console.WriteLine(padding + "║           AJ Classic Utils             ║");
            Console.WriteLine(padding + "╚════════════════════════════════════════╝");
            Console.WriteLine(padding + prompt);
            Console.WriteLine(padding + "╔════════════════════════════════════════╗");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.WriteLine(padding + $"║ [*] {options[i].PadRight(34)} ║");
                }
                else
                {
                    Console.WriteLine(padding + $"║ [ ] {options[i].PadRight(34)} ║");
                }
            }

            Console.WriteLine(padding + "╚════════════════════════════════════════╝");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // true to intercept the key press
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return selectedIndex;
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                return -1; // value to indicate Esc was pressed
            }
        }
    }

    static void HandleSelection(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                HandleDevTools();
                break;
            case 1:
                HandleAutoUpdate();
                break;
            case 2:
                HandleCacheClear();
                break;
            case 3:
                ResetAll();
                break;
            case 4:
                ReplaceAsar();
                break;
            case 5:
                RePack();
                break;
        }
    }

    static void HandleDevTools()
    {
        string[] devToolsOptions = {
            "Enable Dev Tools",
            "Disable Dev Tools"
        };
        int selectedIndex = DisplayMenu("Select an option for Dev Tools:", devToolsOptions);

        if (selectedIndex == -1) return; // Exit submenu if Esc was pressed

        Console.Clear();
        ReplaceAsar();
        if (selectedIndex == 0)
        {
            EnableDevTools();
        }
        else if (selectedIndex == 1)
        {
            DisableDevTools();
        }
    }

    static void HandleAutoUpdate()
    {
        string[] autoUpdateOptions = {
            "Enable Client auto-update",
            "Disable Client auto-update"
        };
        int selectedIndex = DisplayMenu("Select an option for Client auto-update:", autoUpdateOptions);

        if (selectedIndex == -1) return; // Exit submenu if Esc was pressed

        Console.Clear();
        ReplaceAsar();
        if (selectedIndex == 0)
        {
            EnableUpdates();
        }
        else if (selectedIndex == 1)
        {
            DisableUpdates();
        }
    }

    static void HandleCacheClear()
    {
        string[] cacheClearOptions = {
            "Clear Aj Classic Cache",
            "Clear Jam Cache",
            "Clear Both Caches",
            "Auto-Clear Enable",
            "Auto-Clear Disable"
        };
        int selectedIndex = DisplayMenu("Select an option for Cache Clear:", cacheClearOptions);

        if (selectedIndex == -1) return; // Exit submenu if Esc was pressed

        Console.Clear();
        switch (selectedIndex)
        {
            case 0:
                ClearCache(1);
                break;
            case 1:
                ClearCache(2);
                break;
            case 2:
                ClearCache(3);
                break;
            case 3:
                AutoClearEnable();
                break;
            case 4:
                AutoClearDisable();
                break;
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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
                    Console.WriteLine(ex.Message);
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
        if (Directory.Exists($@"C:\Users\{username}\AppData\Local\Programs\aj-classic"))
        {
            Console.WriteLine("Ajc is installed. Checking for app.asar");

            if (File.Exists(@$"C:\Users\{username}\AppData\Local\Programs\aj-classic\resources\app.asar"))
            {
                Console.WriteLine("Already archived");
                Environment.Exit(0);
            }
            else if (Directory.Exists(archive))
            {
                Console.WriteLine("RePacking asar archive.");
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
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No Packed/Unpacked asar detected. (is your install broken?)");
            }
        }
        else
        {
            Console.WriteLine("Ajc is not installed. Exiting...");
            Environment.Exit(0);
        }
    }

    // Simplified functions because I aint typing allat
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
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
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
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
            Console.WriteLine(ex.Message);
        }
    }
}

