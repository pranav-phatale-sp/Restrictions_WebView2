using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls.Crypto;
using Google.Protobuf;


namespace WebView2_Project_4
{
    public partial class Form1 : Form
    {
        private WebView2 webview;
        public Form1()
        {
            InitializeComponent();
            InitialiseWebView2();
            InitialiseForm();
        }


        // Code for updating and getting language selected by user

        static string connectionString = "Server=localhost;Database=pranav;User ID=root;Password=1234;";
        static string GetCurrentLang()
        {
            string lang = string.Empty;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT lang FROM settings LIMIT 1";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        var result = cmd.ExecuteScalar();
                        lang = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return lang;
        }
        static void UpdateLang(string newLang)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "UPDATE settings SET lang = @newLang WHERE id = 1";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@newLang", newLang);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }


        // initialisation of form with combobox and some other settings

        private void InitialiseForm()
        {
            this.comboBox1.Items.Add("Hindi");
            this.comboBox1.Items.Add("English");
            this.comboBox1.Items.Add("Spanish");
            this.comboBox1.Items.Add("French");
            this.comboBox1.Items.Add("Arabic");
            this.comboBox1.Items.Add("Marathi");
            this.comboBox1.SelectedIndexChanged += changeLanguage;
            this.MinimizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;

        }


        // change language code, where we can upadte lang in DB also restart the application to reset environment
        private void changeLanguage(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Hindi")
            {

                UpdateLang("hi-IN");
                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "English")
            {
                UpdateLang("en-GB");
                Application.Exit();
                this.Close();
                Application.Restart();  
             
            }
            else if (comboBox1.SelectedItem.ToString() == "Spanish")
            {
               
                UpdateLang("es-ES");
                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "French")
            { 
            
                UpdateLang("fr-FR");
                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "Arabic")
            {
               
                UpdateLang("ar-SA");
                Application.Exit();
                this.Close();
                Application.Restart();
            }
            else if (comboBox1.SelectedItem.ToString() == "Marathi")
            {

                UpdateLang("mr-IN");
                this.Close();
                Application.Exit();
                
                Application.Restart();
            }


        }

        // initialising webview2 with lifecycles, setting env language and some restrictions 
        private async void InitialiseWebView2()
        {

            webview = new WebView2();
            webview.Dock = DockStyle.Fill;
            this.Controls.Add(webview);

            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions(null);

            string ss = GetCurrentLang();

            options.Language = ss;

            var env = await CoreWebView2Environment.CreateAsync(null, null, options);

            await webview.EnsureCoreWebView2Async(env);

            webview.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webview.CoreWebView2.Settings.AreDevToolsEnabled = false;


            webview.CoreWebView2.Navigate("https://chatgpt.com/");

            webview.CoreWebView2.NavigationStarting += (sender, args) =>
            {
                string getUri = "";
                int cnt = 0;

                for (int i = 0; i < args.Uri.Length; i++)
                {
                    if (args.Uri[i] == '/') cnt++;
                    getUri += args.Uri[i];
                    if (cnt == 3) break;

                }

                if (getUri != "https://chatgpt.com/")
                {
                    args.Cancel = true;
                    MessageBox.Show("Navigation is restricted.");
                }

                if (args.Uri.Contains("target=_blank"))
                {
                    args.Cancel = true;
                    MessageBox.Show("Navigation is restricted");
                }



            };

            webview.CoreWebView2.NavigationCompleted += webViewNavigationCompleted;

            webview.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

            webview.KeyDown += (sender, e) =>
            {
                if (
                     (e.Control && e.KeyCode == Keys.P) ||
                     (e.Control && e.KeyCode == Keys.S) ||
                     (e.Control && e.KeyCode == Keys.U) ||
                     (e.KeyCode == Keys.F12) ||
                     (e.Control && e.KeyCode == Keys.C) ||
                     (e.Control && e.KeyCode == Keys.V) ||
                     (e.Control && e.Shift && e.KeyCode == Keys.C) ||
                     (e.Alt) ||
                     (e.Control) || (e.Shift)
                 )
                {
                    e.SuppressKeyPress = true;
                    MessageBox.Show("This action is disabled!");
                }
            };


        }

        protected override void OnSizeChanged(EventArgs e)
        {
            
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            string selectedLanguage = Lang.currentlang;
            selectedLanguage = "hi-IN";
            MessageBox.Show("hii");

            var requestHeaders = e.Request.Headers;
            requestHeaders.SetHeader("Accept-Language", selectedLanguage);

            // Optionally, you can print the modified headers for debugging
            Console.WriteLine($"New Accept-Language Header: {selectedLanguage}");
        }

        private void CoreWebView2_NewWindowRequested(object sender,CoreWebView2NewWindowRequestedEventArgs e)
        {
            
            e.Handled = true;
        }


        private async void webViewNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {



            //await disablePrint();
            //await disableInspect();
            //await disableRefreshing();
            //await disableCopyText();
            //await disablePaste();
            //await disableDialogBox();
            //await disableSavingPage();
            //await disableHtmlPage();
            //await disableAltKey();



            await webview.CoreWebView2.ExecuteScriptAsync(@"
                window.print = function() {
                    console.log('Print is disabled.');
                };
            ");

        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }
        

        // js code ->
        private async Task disablePrint()
        {
           
        }

        private async Task disableInspect()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if ((event.ctrlKey && event.shiftKey && event.key.toLowerCase() === 'c') ||
                         ( event.key === 'F12')
) {
                        event.preventDefault();
                        alert('Developer tools are disabled!');
                    }
                });
            ");
        }

        private async Task disableRefreshing()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'r') {
                        event.preventDefault();
                        alert('Refreshing Page is disabled!');
                    }
                });
            ");
        }

        private async Task disableCopyText()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'c') {
                        event.preventDefault();
                        alert('copying is disabled!');
                    }
                });
            ");
        }

        private async Task disablePaste()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'v') {
                        event.preventDefault();
                        alert('Pasting is disabled!');
                    }
                });
            ");
        }

        private async Task disableDialogBox()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'j') {
                        event.preventDefault();
                        alert('Dialog box is disabled!');
                    }
                });
            ");
        }

        private async Task disableSavingPage()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 's') {
                        event.preventDefault();
                        alert('saving page is disabled!');
                    }
                });
            ");
        }

        private async Task disableHtmlPage()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'u') {
                        event.preventDefault();
                        alert('viewing html is disabled!');
                    }
                });
            ");
        }

        private async Task disableAltKey()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.altKey) {
                        event.preventDefault();
                        alert('Alt key is disabled!');
                    }
                });
            ");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
    }
}
