using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
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
        public MainPage()
        {

            System.Diagnostics.Debug.WriteLine("new mainpage");
            this.InitializeComponent();

            List<string> ssh_hosts = new List<string>();
            try
            {
                ssh_hosts = this.GetSshHosts();
            }
            catch (AggregateException ae)
            {
                ae.Handle((ex) =>
                {
                    if (ex is UnauthorizedAccessException)
                    {
                        var task = Task.Run(async () => {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                DisplayNoAccessDialog();
                            });
                        });
                        task.Wait();
                        Application.Current.Exit();
                        return true;
                    }
                    return false;
                });
            }

            foreach (string host in ssh_hosts)
            {
                this.SshList.Items.Add(host);
            }
        }

        public List<String> GetSshHosts()
        {
            var task = Task.Run(async () => {
                return await GetSshConfigContentsAsync();
            });
            string ssh_config_text = task.Result;
            List<string> ssh_hosts = new List<string>();

            foreach (string line in ssh_config_text.Split(new char[] { '\r', '\n' }))
            {
                if (line.StartsWith("host", StringComparison.OrdinalIgnoreCase))
                {
                    List<string> hosts_in_line = line.Trim().Split(' ').ToList();
                    hosts_in_line.RemoveAt(0); // Delete the first item in the list, "host"
                    foreach (string host in hosts_in_line)
                    {
                        System.Diagnostics.Debug.WriteLine("new ssh host found: " + host);
                        ssh_hosts.Add(host);
                    }
                }
            }

            return ssh_hosts.Distinct().ToList();
        }

        private async Task<string> GetSshConfigContentsAsync()
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string ssh_config_path = Path.Combine(home, ".ssh", "config");
            StorageFile ssh_config = await StorageFile.GetFileFromPathAsync(ssh_config_path);
            string text = await FileIO.ReadTextAsync(ssh_config);
            return text;
        }

        private async void DisplayNoAccessDialog()
        {
            ContentDialog noAccessDialog = new ContentDialog
            {
                Title = "No file access",
                Content = "This application requires file access to function. Please allow it access under Settings > Privacy > File Access.",
                CloseButtonText = "Ok"
            };

            // Use this code to associate the dialog to the appropriate AppWindow by setting
            // the dialog's XamlRoot to the same XamlRoot as an element that is already present in the AppWindow.
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                noAccessDialog.XamlRoot = this.SshList.XamlRoot;
            }

            ContentDialogResult result = await noAccessDialog.ShowAsync();
        }
    }
}
