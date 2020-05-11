using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SSH_Launcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string SShHostPattern = @"(Host\s[\w\*]+)(\n\s+\w+\s.*)+";

        public MainPage()
        {
            Console.WriteLine("New MainPage");
            this.InitializeComponent();

            this.SshList.Items.Add("hello world");
            this.debugLine.Text = "debug output goes here";
            Console.WriteLine("Getting SSH hosts");
            //this.GetSshHosts();
            Console.WriteLine("..got em!");
        }

        public List<String> GetSshHosts()
        {
            var task = GetSshConfigContentsAsync();
            //task.Wait();
            string ssh_config_text = task.Result;
            List<string> ssh_hosts = new List<string>();

            //foreach (Match m in Regex.Matches(ssh_config_text, SShHostPattern))
            //{
            //    Console.WriteLine("new ssh host found: " + m.Value);
            //}

            Console.WriteLine(ssh_config_text);

            return ssh_hosts;
        }

        public async Task<string> GetSshConfigContentsAsync()
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string ssh_config_path = Path.Combine(home, ".ssh", "config");
            StorageFile ssh_config = await StorageFile.GetFileFromPathAsync(ssh_config_path);
            string text = await FileIO.ReadTextAsync(ssh_config);
            return text;
        }
    }
}
