using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using SimpleHttpServer;
using TranscubeApp.Properties;

namespace TranscubeApp
{
    public partial class Form1 : Form
    {
        private HttpServer _server;

        public Form1()
        {
            InitializeComponent();
            Start();
        }

        public void Start()
        {
            var resHandle = new ResponseHandle();
            _server = new HttpServer("", 6789, resHandle.getRequests, resHandle.postRequests);

            CefSettings settings = new CefSettings
            {
                CachePath = "cache"
            };

            settings.CefCommandLineArgs.Add("ppapi-flash-path", "pepflashplayer.dll");
            settings.CefCommandLineArgs.Add("ppapi-flash-version", "20.0.0.306");
            settings.CefCommandLineArgs.Add("plugin-policy", "allow");
            settings.CefCommandLineArgs.Add("enable-npapi", "1"); //Enable NPAPI plugs which were disabled by default in Chromium 43 (NPAPI will be removed completely in Chromium 45)
            //settings.CefCommandLineArgs.Add("disable-web-security", "1");
           //custom/Game/index.html
            Cef.Initialize(settings);
            //// Create a browser component

            String page = "http://localhost:6789/game/index.html";

            var chromeBrowser = new ChromiumWebBrowser(page);
            chromeBrowser.ConsoleMessage += ChromeBrowserOnConsoleMessage;

            // Add it to the form and fill it to the form window.
            gamePanel.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
        }

        private void ChromeBrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs consoleMessageEventArgs)
        {
            Console.WriteLine("Chromium : " + consoleMessageEventArgs.Message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OnFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            _server.Stop();
        }

    }



    public class ResponseHandle
    {
        public List<UrlRequest> getRequests = new List<UrlRequest>();
        public List<UrlRequest> postRequests = new List<UrlRequest>();

        public ResponseHandle()
        {
            getRequests.Add(new UrlRequest("getTest", inputs => "this is get Test"));
            getRequests.Add(new UrlRequest("getTest2", GetTest2));
            postRequests.Add(new UrlRequest("postTest", PostTest));
        }

        string GetTest2(HttpListenerRequest req)
        {
            return "this is get test 2";
        }

        string PostTest(HttpListenerRequest req)
        {
            var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            var text = reader.ReadToEnd();

            Console.WriteLine("ContentType " + req.ContentType);
            Console.WriteLine("Content " + text);

            //need to decode content to use 
            return "this is Post test";
        }


    }

    
}
