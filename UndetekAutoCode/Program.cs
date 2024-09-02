using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    private const string BASE_URL = "https://icheat.io/free-cs2-cheats/";
    private const string CODE_URL = "https://icheat.io/files/ic/getpin-53478634576234987435.php";
    private const string FILE_URL = "https://icheat.io/files/download.php";


    private static HttpClient hClient = new HttpClient();

    public static void Main()
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("    ______\n   / ____/___  ____  ________  _________ ___  ____  _____\n  / / __/ __ \\/ __ \\/ ___/ _ \\/ ___/ __ `__ \\/ __ \\/ ___/\n / /_/ / /_/ / / / / /__/  __/ /  / / / / / / /_/ / /\n \\____/\\____/_/ /_/\\___/\\___/_/  /_/ /_/ /_/\\____/_/\n");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Title = "Undetek Crack (No Auth Code)";

        // Get the process handle
        Process? CSprocess = Process.GetProcessesByName("cs2").FirstOrDefault();
#if !DEBUG
        // Check if the game is open, otherwise close
        if (CSprocess == null)
        {
            FancyWriteLine("Counter-Strike 2 Process not found. Exiting...", "ERROR", ConsoleColor.Magenta);
            Thread.Sleep(3000);
            return;
        }
#endif
        // Set the referer (API bypass)
        hClient.DefaultRequestHeaders.Referrer = new Uri(BASE_URL);

        FancyWriteLine("Downloading Undetek FREE", "DOWNLOAD", ConsoleColor.Blue);
        DownloadZip();

        FancyWriteLine("Extracting files", "EXTRACTION", ConsoleColor.DarkCyan);
        ExtractZip();

        FancyWriteLine("Copying files", "COPY", ConsoleColor.Green);
        if (!ReOrganizeFolder())
        {
            FancyWriteLine("File copy failed. Exiting...", "ERROR", ConsoleColor.Red);
            Thread.Sleep(3000);
            return;
        }

        string? FilePath = Directory.GetFiles("undetek").FirstOrDefault(); // Get path to the undetek exe file

        FancyWriteLine("Requesting Code (Referer Bypass)", "CODE", ConsoleColor.DarkYellow);
        short Code = GetCode(); // Get the activation code
        FancyWriteLine($"Got code: {Code.ToString("0000")}", "CODE", ConsoleColor.DarkYellow);

        Process UndetekProcess = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FilePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            },
            EnableRaisingEvents = true,

        };
        UndetekProcess.Start();
        FancyWriteLine("Passing Code.", "CODE", ConsoleColor.DarkYellow);
        UndetekProcess.StandardInput.WriteLine(Code);
        FancyWriteLine("Waiting for injection.", "PROCESS", ConsoleColor.Magenta);
        UndetekProcess.WaitForExit();
        FancyWriteLine("Injection completed sucessfully. Exiting...", "SUCCESS", ConsoleColor.Green);
        Thread.Sleep(3000);

#if !DEBUG
        // Focus CSGO 2 Window
        ShowWindow(CSprocess.MainWindowHandle, 9);
#endif
        return;
    }

    public static short GetCode() // Function to get the activation code
    {
        using (HttpResponseMessage Request = hClient.GetAsync(CODE_URL).Result)
        {
            Request.EnsureSuccessStatusCode();
            return Convert.ToInt16(Request.Content.ReadAsStringAsync().Result);
        }
    }

    public static void DownloadZip() // Download the undetek package
    {
        using (HttpResponseMessage Request = hClient.GetAsync(FILE_URL).Result)
        {
            Request.EnsureSuccessStatusCode();
            Stream contentStream = Request.Content.ReadAsStreamAsync().Result;
            if (File.Exists("undetek.new.zip")) File.Delete("undetek.new.zip");
            using (FileStream fs = new FileStream("undetek.new.zip", FileMode.CreateNew, FileAccess.Write))
                Request.Content.CopyTo(fs, null, CancellationToken.None);
        }
    }

    public static void ExtractZip() // Extract the undetek package
    {
        if (Directory.Exists("undetek")) Directory.Delete("undetek", true);
        System.IO.Compression.ZipFile.ExtractToDirectory("undetek.new.zip", "undetek");
        File.Delete("undetek.new.zip");
    }

    public static bool ReOrganizeFolder() // Re-Organizes the folder of undetek
    {
        string? Dir = Directory.GetDirectories("undetek").FirstOrDefault();
        if (Dir == null) return false;
        string? ExecFile = Directory.GetFiles(Dir).Where(x => x.StartsWith("undetek") && x.EndsWith(".exe")).FirstOrDefault();
        if (ExecFile == null) return false;
        File.Move(ExecFile, $"undetek{ExecFile.Replace(Dir, string.Empty)}");
        Directory.Delete(Dir, true);
        return true;
    }


    public static void FancyWriteLine(string data, string label, ConsoleColor labelColor) // Write logs to the console in a fancy way
    {

        Console.Write(" [");
        Console.ForegroundColor = labelColor;
        Thread.Sleep(10);
        foreach (char Char in label)
        {
            Console.Write(Char);
            Thread.Sleep(5);
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("] - ");
        foreach (char Char in data)
        {
            Console.Write(Char);
            Thread.Sleep(5);
        }
        Console.Write("\n");
    }

    // SetForegroundWindow from WinAPI
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

}