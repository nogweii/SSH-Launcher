using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Environment;

namespace SshConfigParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string ssh_config_text = GetSshContents();

            foreach (string line in ssh_config_text.Split(new char[] { '\r', '\n' }))
            {
                //Console.WriteLine(line);
                if (line.StartsWith("host", StringComparison.OrdinalIgnoreCase))
                {
                    List<string> hosts_in_line = line.Trim().Split(' ').ToList();
                    hosts_in_line.RemoveAt(0); // Delete the first item in the list, "host"
                    foreach (string host in hosts_in_line)
                    {
                        Console.WriteLine("new ssh host found: " + host);
                    }
                }
            }
        }

        static public string GetSshContents()
        {
            var home = Environment.GetFolderPath(SpecialFolder.UserProfile);
            var text = File.ReadAllText(Path.Join(home, ".ssh", "config"));
            return text;
        }
    }
}
