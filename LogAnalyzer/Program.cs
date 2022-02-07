using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
// using Linq to select what i need from list
using System.Linq;
// using Net for DNS Lookup
using System.Net;
using System.Text;
using System.Threading.Tasks;
//installed IISLogParser library  im using it to pars log files from filesystem to list so i can use linq to select what i need
using IISLogParser;

namespace LogAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)

        {
            string Folder;
            string FileName;

            //Get folder name and file name
            Console.WriteLine("Assumption log files are in C:\\inetpub\\logs\\LogFiles");
            Console.Write("Enter name of the folder in LogFiles folder: ");
            Folder =Console.ReadLine();
            Console.Write($"Enter name of the log file in {Folder} folder: ");
            FileName = Console.ReadLine();


            //string FolderAndFile = "W3SVC1\\u_ex220207.log"; - folder and file on my local server

            //Assumption log files are in C:\inetpub\logs\LogFiles
            //Also using key in app settings, so the path can be used in evry part of the app
            //If not you can change log file folder in app setting key name is PathToLogFiles
            string PathToLogFile = ConfigurationManager.AppSettings["PathToLogFiles"]+ "\\" + Folder + "\\" + FileName;


            //Parsing file and puting it in List
            List<IISLogEvent> logs = new List<IISLogEvent>();
            using (ParserEngine parser = new ParserEngine(PathToLogFile))
        {
                while (parser.MissingRecords)
                {
                    logs = parser.ParseLog().ToList();
                }
            }

            //Query to select log, group it by ip address,count how many hits that ip adress has
            var queryForIp = from log in logs
                             group log by log.cIp into ipCount
                             select new
                             {
                                 IpAdress = ipCount.Key,
                                 Count = ipCount.Count(),
                             };
            foreach (var ipCount in queryForIp)
            {
                //using System.Net to get hostname
                IPHostEntry host = Dns.GetHostEntry(ipCount.IpAdress);

                //Outputing result to console output
                Console.WriteLine($"{host.HostName} ({ipCount.IpAdress}) - {ipCount.Count}");
            }
            Console.Read();
        }
    }
}
