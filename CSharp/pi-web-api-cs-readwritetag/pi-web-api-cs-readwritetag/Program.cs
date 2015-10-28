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
using pi_web_api_cs_helper;

namespace pi_web_api_cs_readwritetag
{
    class Program
    {
        static string baseUrl = "https://myserver/piwebapi";

        static void Main(string[] args)
        {
            Console.Write("Enter username: ");
            string userName = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = GetPassword();
            PIWebAPIClient client = new PIWebAPIClient(baseUrl, userName, password);

            try
            {
                Console.WriteLine("Getting list of PI Data Archives...");
                Dictionary<string, string> serversWebId = client.GetDataServersWebIdAsync().Result;
                foreach (var server in serversWebId)
                {
                    Console.WriteLine(server.Key);
                }
                do
                {
                    Console.Write("Please type in name of the PI Data Archive: ");
                    string myPI = Console.ReadLine();
                    Console.Write("Please type in name of the tag: ");
                    string myTag = Console.ReadLine();

                    if (IsRead())
                    {
                        dynamic value = client.GetTagValueAsync(serversWebId[myPI], myTag).Result;
                        Console.WriteLine("Current value = {0}", value);
                    }
                    else
                    {
                        Console.Write("Value to write? ");
                        string valueToWrite = Console.ReadLine();
                        client.WriteTagValueAsync(serversWebId[myPI], myTag, valueToWrite).Wait();
                        Console.WriteLine("Value written.");
                    }
                    Console.WriteLine("Press any key to continue (esc to exit)...");
                } while (Console.ReadKey().Key != ConsoleKey.Escape);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("The specified PI Data Archive cannot be found.");
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
