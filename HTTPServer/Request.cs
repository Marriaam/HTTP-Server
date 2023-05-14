using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        public string filename;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
           
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] ss = new string[] { "\r\n" };
            requestLines = requestString.Split(ss, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            if (!ParseRequestLine())
            {
                return false;
            }
            if (!ValidateBlankLine())
            {
                return false;
            }
            if (!LoadHeaderLines())
            {
                return false;
            }
            else
            {
                filename = relativeURI.Trim('/');
                relativeURI = "http://" + headerLines["Host"] + '/'+filename;
            }
            if (!ValidateIsURI(relativeURI))
            {
                return false;
            }
            return true;

        }

        private bool ParseRequestLine()
        {
            string[] line = requestLines[0].Split(' ');
            //method = line[0];
            relativeURI = line[1];
            //httpVersion = line[2];
            if (line[0] == "GET")
            {
                if (line[2] == "HTTP/1.1")
                {
                    return true;
                }
            }
            return false;
            //throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            try
            {

                headerLines = new Dictionary<string, string>();
                int i = 1;
                string[] ss2 = new string[] { ": " };
                while (!string.IsNullOrEmpty(requestLines[i]))
                {
                    string headerContent = requestLines[i];
                    string[] data = headerContent.Split(ss2, StringSplitOptions.None);
                    headerLines.Add(data[0], data[1]);
                    i++;
                }

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (string.IsNullOrEmpty(requestLines.Last()))
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }

    }
}