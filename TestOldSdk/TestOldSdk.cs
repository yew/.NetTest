using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.IO;
using System.Text;

namespace TestSftpSdk
{
    class TestSftpSdk
    {
        static JObject Jhost;

        private static ConnectionInfo ExtractSshConnectionInfo(string host, int port, string userName, string password)
        {
            return new ConnectionInfo(host, port, userName, new PasswordAuthenticationMethod(userName, password));
        }

        private static string GetFileAttribures(SftpClient client, String fileName)
        {
            var fileAttributes = client.GetAttributes(fileName);

            StringBuilder permission = new StringBuilder();
            permission.Append("    Permission / User id / Group id:\n");
            permission.Append("    ");
            permission.Append(fileAttributes.IsDirectory ? "d" : "-");
            permission.Append(fileAttributes.OwnerCanRead ? "r" : "-");
            permission.Append(fileAttributes.OwnerCanWrite ? "w" : "-");
            permission.Append(fileAttributes.OwnerCanExecute ? "x" : "-");
            permission.Append(fileAttributes.GroupCanRead ? "r" : "-");
            permission.Append(fileAttributes.GroupCanWrite ? "w" : "-");
            permission.Append(fileAttributes.GroupCanExecute ? "x" : "-");
            permission.Append(fileAttributes.OthersCanRead ? "r" : "-");
            permission.Append(fileAttributes.OthersCanWrite ? "w" : "-");
            permission.Append(fileAttributes.OthersCanExecute ? "x" : "-");
            permission.Append(" " + fileAttributes.UserId + " " + fileAttributes.GroupId + " " + "\n");

            permission.Append("\n    File Size:\n");
            permission.Append("    " + fileAttributes.Size + "\n");

            permission.Append("\n    Last Write Time:\n");
            permission.Append("    " + fileAttributes.LastWriteTime + "\n");

            permission.Append("\n    Last Access Time:\n");
            permission.Append("    " + fileAttributes.LastAccessTime + "\n");

            return permission.ToString();
        }

        private static string GetFileName(string path)
        {
            int idx = path.LastIndexOf("/") + 1;
            return path.Substring(idx, path.Length - idx);
        }

        static void Main(string[] args)
        {
            Jhost = JObject.Parse(File.ReadAllText(@"../host.json"));
            var host = Jhost["host"].ToString();
            var port = int.Parse(Jhost["port"].ToString());
            var userName = Jhost["username"].ToString();
            var password = Jhost["password"].ToString();
            var filePath = Jhost["filepath"].ToString();

            var connectionInfo = ExtractSshConnectionInfo(host, port, userName, password);

            using (SftpClient client = new SftpClient(connectionInfo))
            {
                Console.WriteLine("\nConnect");
                client.Connect();

                Console.WriteLine("\nGet attributes\n");
                Console.WriteLine(GetFileAttribures(client, filePath));

                Console.WriteLine("\nStart to download");
                FileStream fs = new FileStream("./" + GetFileName(filePath), FileMode.Create);
                client.DownloadFile(filePath, fs);
                Console.WriteLine("Download complete");

                Console.WriteLine("\nDisconnect");
                client.Disconnect();
            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.Read();
        }
    }
}
