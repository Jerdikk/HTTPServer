// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)     
/*
  
 This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org/>
 
 */


using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
    partial class Program
    {
        class HttpServer
        {
            public static HttpListener listener;
            public static string url = "http://localhost:8000/";
            public static int pageViews = 0;
            public static int requestCount = 0;
            public static string pageData =
                "<!DOCTYPE>" +
                "<html>" +
                "  <head>" +
                "    <title>HttpListener Example</title>" +
                "  </head>" +
                "  <body>" +
                "    <p>Page Views: {0}</p>" +
                "    <form method=\"post\" action=\"shutdown\">" +
                "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
                "    </form>" +
                "  </body>" +
                "</html>";


            public static async Task HandleIncomingConnections()
            {
                bool runServer = true;

                // While a user hasn't visited the `shutdown` url, keep on handling requests
                while (runServer)
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = await listener.GetContextAsync();

                    // Peel out the requests and response objects
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    // Print out some info about the request
                    Console.WriteLine("Request #: {0}", ++requestCount);
                    Console.WriteLine(req.Url.ToString());
                    Console.WriteLine(req.HttpMethod);
                    Console.WriteLine(req.UserHostName);
                    Console.WriteLine(req.UserAgent);
                    Console.WriteLine();

                    // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                    if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                    {
                        Console.WriteLine("Shutdown requested");
                        runServer = false;
                    }

                    // Make sure we don't increment the page views counter if `favicon.ico` is requested
                    if (req.Url.AbsolutePath != "/favicon.ico")
                        pageViews += 1;

                    // Write the response info
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
            }


            public void Run()
            {
                // Create a Http server and start listening for incoming connections
                listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                Console.WriteLine("Listening for connections on {0}", url);

                // Handle requests
                Task listenTask = HandleIncomingConnections();
                listenTask.GetAwaiter().GetResult();

                // Close the listener
                listener.Close();
            }
        }
    }

}
