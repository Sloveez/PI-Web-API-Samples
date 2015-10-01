/***************************************************************************
   Copyright 2015 OSIsoft, LLC.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pi_web_api_cs_helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pi_web_api_cs_readwritetag
{
    class Program
    {
        static string baseUrl = "https://dng-code.osisoft.int/piwebapi";

        static void Main(string[] args)
        {
            Console.Write("Enter username: ");
            string userName = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = GetPassword();
            PIWebAPIClient client = new PIWebAPIClient(userName, password);

            try
            {
                GetKST(client);
                do
                {
                    string tagPath = GetTagPath();
                    if (IsRead())
                    {
                        GetTagValue(client, tagPath);
                    }
                    else
                    {
                        WriteTagValue(client, tagPath);
                    }
                    Console.WriteLine("Press any key to continue (esc to exit)...");
                } while (Console.ReadKey().Key != ConsoleKey.Escape);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }
            }
            finally
            {
                client.Dispose();
                Console.ReadKey();
            }
        }

        // Obtain password input from console
        private static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                }
                else if (!string.IsNullOrEmpty(password))
                {
                    password = password.Substring(0, password.Length - 1);
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }

        // Display the list of PI Data Archives
        private static void GetKST(PIWebAPIClient client)
        {
            Console.WriteLine("Getting list of PI Data Archives...");
            string url = baseUrl + "/dataservers";
            JObject jobj = client.GetRequest(url);
            if (jobj["Items"] != null)
            {
                for (int i = 0, num = jobj["Items"].Count(); i < num; i++)
                {
                    Console.WriteLine(jobj["Items"][i]["Name"]);
                }
            }
        }

        // Get the name of the PI Data Archive and name of the PI tag.
        private static string GetTagPath()
        {
            Console.Write("Please type in name of the PI Data Archive: ");
            string myPI = Console.ReadLine();
            Console.Write("Please type in name of the tag: ");
            string myTag = Console.ReadLine();
            return string.Format("\\\\{0}\\{1}", myPI, myTag);
        }

        // Get the most recent value of the PI tag.
        private static void GetTagValue(PIWebAPIClient client, string path)
        {
            Console.WriteLine("Getting tag value...");
            string url = GetTagUrl(client, path);
            if (!url.Equals("Error"))
            {
                JObject jobj = client.GetRequest(url);
                if (jobj["Value"] != null)
                {
                    Console.WriteLine("Current value = {0}", jobj["Value"]);
                }
            }
        }

        // Write value to the tag at current time.
        private static void WriteTagValue(PIWebAPIClient client, string path)
        {
            Console.Write("Value to write? ");
            string valueToWrite = Console.ReadLine();
            string url = GetTagUrl(client, path);
            if (!url.Equals("Error"))
            {
                object payload = new
                {
                    value = valueToWrite
                };
                string data = JsonConvert.SerializeObject(payload);
                client.PostRequest(url, data);
                Console.WriteLine("Value written.");
            }
        }

        // Get the PI Web API endpoint from the tag path.
        private static string GetTagUrl(PIWebAPIClient client, string path)
        {
            string url = baseUrl + "/points?path=" + path;
            JObject jobj = client.GetRequest(url);
            if (jobj["Links"]["Value"] != null)
            {
                return jobj["Links"]["Value"].ToString();
            }
            else
            {
                return "Error";
            }
        }

        // Return true if user enters r or R, false if user enters w or W.
        private static bool IsRead()
        {
            string rw;
            bool isRead = true;
            do
            {
                Console.Write("Do you wish to read or write to the tag (r/w)? ");
                rw = Console.ReadLine();
                if (rw.Equals("r") || rw.Equals("R"))
                {
                    isRead = true;
                }
                else if (rw.Equals("w") || rw.Equals("W"))
                {
                    isRead = false;
                }
                else
                {
                    Console.Write("Invalid input. ");
                }
            } while (!rw.Equals("r") && !rw.Equals("R") && !rw.Equals("w") && !rw.Equals("W"));
            return isRead;
        }

    }
}
