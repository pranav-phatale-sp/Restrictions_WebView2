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
using System.Globalization;
using System.Threading;



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

        private void InitialiseForm()
        {
            this.comboBox1.Items.Add("hi-IN");
            this.comboBox1.Items.Add("en-GB");
            this.comboBox1.Items.Add("es-ES");
            this.comboBox1.Items.Add("fr-FR");
            this.comboBox1.Items.Add("ar-SA");
            this.comboBox1.Items.Add("mr-IN");
            this.comboBox1.SelectedIndexChanged += changeLanguage;
            this.MinimizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;

        }

        private async void InitialiseWebView2()
        {

            webview = new WebView2();
            webview.Dock = DockStyle.Fill;
            this.Controls.Add(webview);

            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions(null);

            string ss = GetCurrentLang();

           
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string language = currentCulture.TwoLetterISOLanguageName;


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

        private async void changeLanguage(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "hi-IN")
            {

                UpdateLang("hi-IN");

                CultureInfo cultureInfo = new CultureInfo("hi-IN");
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string language = currentCulture.TwoLetterISOLanguageName;

                MessageBox.Show(language);



                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "en-GB")
            {
                UpdateLang("en-GB");
                Application.Exit();
                this.Close();
                Application.Restart();  
             
            }
            else if (comboBox1.SelectedItem.ToString() == "es-ES")
            {
               
                UpdateLang("es-ES");
                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "fr-FR")
            { 
            
                UpdateLang("fr-FR");
                Application.Exit();
                this.Close();
                Application.Restart();

            }
            else if (comboBox1.SelectedItem.ToString() == "ar-SA")
            {
               
                UpdateLang("ar-SA");
                Application.Exit();
                this.Close();
                Application.Restart();
            }
            else if (comboBox1.SelectedItem.ToString() == "mr-IN")
            {

                UpdateLang("mr-IN");
                this.Close();
                Application.Exit();
                
                Application.Restart();
            }


        }

        protected override void OnSizeChanged(EventArgs e)
        {
            
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
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
