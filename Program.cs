namespace file_emailer;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

class Config
{

    public string smtpServer = "";
    public int smtpPort = 0;
    public string watchFolder = "";
    public string archiveFolder = "";
    public string senderEmail = "";
    public string receiverEmail = "";
    public string senderEmailToken = "";


    public static Config fromJsonFile(string filePath)
    {

        if (!Path.Exists(filePath))
        {
            throw new ArgumentException("File Path does not exist");

        }
        string text = File.ReadAllText(filePath);
        var values = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
        if (values == null)
        {
            throw new ArgumentException("Config File is not deserializable");
        }
        return new Config
        {
            smtpServer = values["smtpServer"],
            smtpPort = int.Parse(values["smtpPort"]),
            watchFolder = values["watchFolder"],
            archiveFolder = values["archiveFolder"],
            senderEmail = values["senderEmail"],
            receiverEmail = values["receiverEmail"],
            senderEmailToken = values["senderEmailToken"],
        };
    }
}
class Program
{
    static Config? config;
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please specify the config json file");
            return;
        }
        string configFilePath = args[0];
        if (!File.Exists(configFilePath))
        {
            Console.WriteLine("The specified config file path does not exist");
            return;
        }
        config = Config.fromJsonFile(configFilePath);

        using var watcher = new FileSystemWatcher(config.watchFolder);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        watcher.Changed += OnChanged;
        watcher.Error += OnError;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (config == null)
        {
            return;
        }

        Console.WriteLine($"New File {e.FullPath}");
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(config.senderEmail);
        mailMessage.To.Add(config.receiverEmail);
        mailMessage.Subject = Path.GetFileName(e.FullPath);
        Attachment attachment = new Attachment(e.FullPath);
        mailMessage.Attachments.Add(attachment);

        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = config.smtpServer;
        smtpClient.Port = config.smtpPort;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(config.senderEmail, config.senderEmailToken);
        smtpClient.EnableSsl = true;

        try
        {
            smtpClient.Send(mailMessage);
            Console.WriteLine("Email Sent Successfully.");
            File.Move(e.FullPath, Path.Join(config.archiveFolder, DateTime.Now.Ticks.ToString() + "_" + Path.GetFileName(e.FullPath)));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

    }


    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
}
