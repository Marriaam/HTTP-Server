using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt", true);
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            //FileStream fs = new FileStream(@"C: \Users\Malak.User - PC\Desktop\Year2 S1\Computer Networks\Lab 8 + project template\Template[2021 - 2022]\HTTPServer\bin\Debug\log.txt", FileMode.OpenOrCreate);
            //StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine("Date time:" + DateTime.Now.ToString());
            sr.WriteLine("Message : " + ex.Message);
            sr.WriteLine("stack  " + ex.StackTrace);
            sr.WriteLine("   ");
            Console.WriteLine("Exception recorded and saved in file");
            sr.Flush();
        }
    }
}