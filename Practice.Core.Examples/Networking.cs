using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice.Core.Examples
{
    public class MockHandler : HttpMessageHandler
    {
        Func<HttpRequestMessage, HttpResponseMessage> _responseGenerator;
        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> responseGenerator)
        {
            _responseGenerator = responseGenerator;
        }

        protected override Task<HttpResponseMessage> SendAsync
        (HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = _responseGenerator(request);
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }

    public class Networking
    {
        public async void DoSomeStuff()
        {
            IPAddress a1 = new IPAddress(new byte[] { 101, 102, 103, 104 });
            IPAddress a2 = IPAddress.Parse("101.102.103.104");

            Console.WriteLine(a1.Equals(a2)); // True
            Console.WriteLine(a1.AddressFamily); // InterNetwork

            IPAddress a3 = IPAddress.Parse("[3EA0:FFFF:198A:E4A3:4FF2:54fA:41BC:8D31]");
            Console.WriteLine(a3.AddressFamily); // InterNetworkV6

            IPAddress a = IPAddress.Parse("101.102.103.104");
            IPEndPoint ep = new IPEndPoint(a, 222); // Port 222

            Console.WriteLine(ep.ToString()); // 101.102.103.104:222

            var wc = new WebClient();


            wc.DownloadProgressChanged += (sender, args) =>
            Console.WriteLine(args.ProgressPercentage + "% complete");
            Task.Delay(5000).ContinueWith(ant => wc.CancelAsync());

            await wc.DownloadFileTaskAsync("http://oreilly.com", "webpage.htm");

            WebRequest req = WebRequest.Create("http://www.albahari.com/nutshell/code.html");
            req.Proxy = null;

            using (WebResponse res = req.GetResponse())
            using (Stream rs = res.GetResponseStream())
            using (FileStream fs = File.Create("code.html"))
                rs.CopyTo(fs);

            string html = await new HttpClient().GetStringAsync("http://linqpad.net");

            var client = new HttpClient();
            var task1 = client.GetStringAsync("http://www.linqpad.net");
            var task2 = client.GetStringAsync("http://www.albahari.com");
            Console.WriteLine(await task1);
            Console.WriteLine(await task2);

            WebClient wcTest = new WebClient { Proxy = null };
            var data = new NameValueCollection();
            data.Add("Name", "Nick Patsaris");
            data.Add("Height", "6'5");

            byte[] result = wc.UploadValues("http://www.samplewebpage.com/EchoPost.aspx", "POST", data);

            string testUrl = "http://www.samplewebpage.com/EchoPost.aspx";
            var testClient = new HttpClient();
            var col = new Dictionary<string, string>
        {
            { "Name", "Nick Patsaris" },
            { "Company", "A cool company" }
        };

            var values = new FormUrlEncodedContent(col);
            var response = await client.PostAsync(testUrl, values);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            try
            {
                string s = wc.DownloadString("http://www.albahari.com/notthere");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    Console.WriteLine("Bad domain name");
                else if (ex.Status == WebExceptionStatus.ProtocolError)
                {

                    HttpWebResponse tresponse = (HttpWebResponse)ex.Response;
                    Console.WriteLine(tresponse.StatusDescription);
                    if (response.StatusCode == HttpStatusCode.NotFound) // "Not Found"
                        Console.WriteLine("Not there!");
                }
                else throw;
            }
        }

        public static void CookieWork()
        {
            var container = new CookieContainer();

            var request = (HttpWebRequest)WebRequest.Create("http://www.google.com");
            request.Proxy = null;
            request.CookieContainer = container;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                foreach (Cookie c in response.Cookies)
                {
                    Console.WriteLine("Name: " + c.Name);
                    Console.WriteLine("Value " + c.Value);
                    Console.WriteLine("Path: " + c.Path);
                    Console.WriteLine("Domain: " + c.Domain);
                }

                // response stream
            }
        }

        public static void Main()
        {
            ListenAsync(); // Start server
            WebClient wc = new WebClient(); // Make a client request.
            Console.WriteLine(wc.DownloadString("http://localhost:51111/MyApp/Request.txt"));
        }

        public async static void ListenAsync()
        {
            var req = (FtpWebRequest)WebRequest.Create("ftp://ftp.albahari.com");
            req.Proxy = null;
            req.Credentials = new NetworkCredential("nutshell", "oreilly");
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            using (WebResponse resp = req.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                Console.WriteLine(reader.ReadToEnd());

            //RESULT:
            //.
            //..
            //guestbook.txt
            //tempfile.txt
        }

        // OUTPUT: You asked for: /MyApp/Request.txt
    }
}
