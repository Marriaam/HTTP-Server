using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();

            //Start server
            // 1) Make server object on port 1000
            Server s1 = new Server(1000, "redirectionRules.txt");
            // 2) Start Server
            s1.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2


             FileStream fs = File.Create("redirectionRules.txt");
             StreamWriter fw = new StreamWriter(fs);

             fw.WriteLine(@"aboutus.html,aboutus2.html");

            fw.Close();
        }

    }
}
