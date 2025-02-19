using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
//using Microsoft.Web.WebView2.Wpf;
//using Microsoft.Web.WebView2.Wpf;


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

        private void InitialiseForm()
        {
            this.comboBox1.Items.Add("Hindi");
            this.comboBox1.Items.Add("English");
            this.comboBox1.Items.Add("Spanish");
            this.comboBox1.Items.Add("French");
            this.comboBox1.Items.Add("Arabic");
            this.comboBox1.SelectedIndexChanged += changeLanguage;
            this.MinimizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;

        }

        private void changeLanguage(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedItem.ToString() == "Hindi")
            {
                webview.CoreWebView2.Navigate("https://www.bbc.com/hindi");
            }
            else if (comboBox1.SelectedItem.ToString() == "English")
            {
                webview.CoreWebView2.Navigate("https://www.bbc.com/");
            }
            else if(comboBox1.SelectedItem.ToString() == "Spanish")
            {
                webview.CoreWebView2.Navigate("https://www.bbc.com/spanish");
            }
            else if (comboBox1.SelectedItem.ToString() == "French")
            {
                webview.CoreWebView2.Navigate("https://www.bbc.com/afrique");
            }
            else if (comboBox1.SelectedItem.ToString() == "Arabic")
            {
                webview.CoreWebView2.Navigate("https://www.bbc.com/arabic");
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private async void InitialiseWebView2()
        {

            webview = new WebView2();
            webview.Dock = DockStyle.Fill;
            this.Controls.Add(webview);
            await webview.EnsureCoreWebView2Async(null);
            webview.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webview.CoreWebView2.Settings.AreDevToolsEnabled = false;
            var cookieManager = webview.CoreWebView2.CookieManager;
            cookieManager.DeleteAllCookies();




            webview.CoreWebView2.Navigate("https://www.bbc.com/");
            webview.CoreWebView2.NavigationStarting += (sender, args) =>
            {
                string getUri = "";
                int cnt = 0;

                for(int i = 0; i < args.Uri.Length; i++)
                {
                    if (args.Uri[i] == '/') cnt++;
                    getUri += args.Uri[i];
                    if (cnt == 3) break;
                    
                }
               
                if (getUri!= "https://www.bbc.com/")  
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

        private void CoreWebView2_NewWindowRequested(object sender,CoreWebView2NewWindowRequestedEventArgs e)
        {
            
            e.Handled = true;
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    MessageBox.Show("Key Pressed: " + keyData.ToString());

        //    if ((keyData == (Keys.Control | Keys.P)) ||  
        //        (keyData == (Keys.Control | Keys.S)) ||  
        //        (keyData == (Keys.Control | Keys.U)) ||  
        //        (keyData == (Keys.Control | Keys.J)) ||  
        //        (keyData == (Keys.Control | Keys.Shift | Keys.C)) ||  
        //        (keyData == Keys.F12) ||  
        //        (keyData == (Keys.Control | Keys.C)) ||  
        //        (keyData == (Keys.Control | Keys.V)) ||  
        //        (keyData == (Keys.Control | Keys.R)) ||  
        //        (keyData == Keys.Alt))  
        //    {
        //        MessageBox.Show("This action is disabled!");
        //        return true; 
        //    }

        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

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

        private async Task disablePrint()
        {
            await webview.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('keydown', function (event) {
                    if (event.ctrlKey && event.key.toLowerCase() === 'p') {
                        event.preventDefault();
                        alert('Printing is disabled!');
                    }
                });

                window.print = function() { alert('Printing is disabled!'); };
            ");
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
    }
}
