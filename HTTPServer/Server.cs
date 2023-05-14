using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int portNumber;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.portNumber = portNumber;
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);
            
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"

            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New Client accepted:{0}", clientSocket.RemoteEndPoint);
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleConnection), clientSocket);
                HandleConnection(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int recivedLength;
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024];
                    // TODO: Receive request
                    recivedLength = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if(recivedLength == 0)
                    {
                        Console.WriteLine("Client:{0} ended the Connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    string clienthtml = Encoding.ASCII.GetString(data, 0, recivedLength);
                    Request request = new Request(clienthtml);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    byte[] RS = Encoding.ASCII.GetBytes(response.ResponseString);
                    // TODO: Send Response back to client
                    
                    clientSock.Send(RS);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        { 
            string content=string.Empty;
            string pagename=string.Empty;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    pagename = "BadRequest.html";
                    
                    string filepath = Configuration.RootPath + '\\' + pagename;
                    content = LoadDefaultPage(filepath);
                    Response badreq = new Response(StatusCode.BadRequest, "text/html;charset=UTF-8", content, filepath);
                    return badreq;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string uri = request.relativeURI;
               
                pagename = request.filename;
                string filename = Configuration.RootPath + '\\' + pagename;
                
                content = LoadDefaultPage(pagename);
                //Console.WriteLine(File.Exists(filename));
                
                //Console.WriteLine(GetRedirectionPagePathIFExist(pagename));
                if (GetRedirectionPagePathIFExist(pagename) != string.Empty)
                {
                    pagename = GetRedirectionPagePathIFExist(pagename);
                    //Console.WriteLine(filename);
                    
                    filename = Configuration.RootPath + '\\' + pagename;
                    content = LoadDefaultPage(pagename);
                    //Console.WriteLine(filename);
                    //Console.WriteLine(pagename);
                    //Console.WriteLine(content);
                    Response redrec = new Response(StatusCode.Redirect, "text/html;charset=UTF-8", content, pagename);
                    //Console.WriteLine(redrec.ToString());
                    return redrec;
                }

                //TODO: check file exists
                //Console.WriteLine(!File.Exists(filename));
                if (!File.Exists(filename) && pagename != string.Empty)
                {
                    pagename = "NotFound.html";
                    string filepath = Configuration.RootPath + '\\' +pagename;
                    content = LoadDefaultPage(pagename);
                    Response notfound = new Response(StatusCode.NotFound, "text/html;charset=UTF-8", content, filepath);
                    return notfound;
                }
                //TODO: read the physical file
                content = LoadDefaultPage(pagename);

                // Create OK response
                Response r;
                if (pagename == string.Empty)
                {
                    pagename = "main.html";
                    Console.WriteLine(pagename);
                    content = LoadDefaultPage(pagename);
                    string filepath = Configuration.RootPath + '\\' + pagename;
                     r = new Response(StatusCode.OK, "text / html; charset = UTF - 8", content, filepath);
                }
                else
                {
                     r = new Response(StatusCode.OK, "text / html; charset = UTF - 8", content, filename);
                }
                return r;
                
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                pagename = "InternalError.html";
                content = LoadDefaultPage(pagename);
                string file = Configuration.RootPath + '\\' + pagename;
                return new Response(StatusCode.InternalServerError, "text/html;charset=UTF-8", content, file);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach(var i in Configuration.RedirectionRules)
            {
                if(relativePath == i.Key)
                {
                    return i.Value;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            if (!File.Exists(filePath))
            {
                Logger.LogException(new FileNotFoundException("File Does Not Exist"));
                return string.Empty;
            }
            else
            {
                string filetext = File.ReadAllText(filePath);
                return filetext;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] redirectionfile = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary 
                for(int i = 0; i < redirectionfile.Length;i++)
                {
                    char d = ',';
                    string[] words = redirectionfile[i].Split(d);
                    Configuration.RedirectionRules = new Dictionary<string, string>();
                    Configuration.RedirectionRules.Add(words[0], words[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
