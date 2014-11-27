using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace TaskLib
{
    public class TaskArchiveSite:Task
    {
        public HashSet<string> downloaded = new HashSet<string>();
        public string destination = "";

        public TaskArchiveSite() : base() {}

        public TaskArchiveSite(string url, int id, Tasks t, string destination) : base(url, id, t)
        {
            this.destination = destination;
        }

        static public void run(SubTask t)
        {
            try
            {
                // Create a request for the URL. 
                WebRequest request = WebRequest.Create(t.url);
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.

                string rootHref = t.url.Substring(0, t.url.IndexOf("/", 7) + 1);

                if (t.url.EndsWith(".png") || t.url.EndsWith(".jpg") || t.url.EndsWith(".jpeg") ||
                     t.url.EndsWith(".bmp") || t.url.EndsWith(".ico"))
                {
                    t.answer = new string[0];
                    string fileName = t.destination + t.url.Replace(rootHref, "");

                    try
                    {

                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                        BinaryReader br = new BinaryReader(dataStream);
                        File.WriteAllBytes(fileName, br.ReadBytes((int)response.ContentLength));
                        br.Close();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();


                    string pattern = @"((href|link|src)(\s)*=(\s)*)(?<quote>(\\)?('|""))(?<ref>(\S)*)(\k<quote>)";

                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                    MatchCollection matches = rgx.Matches(responseFromServer);

                    List<string> newUrls = new List<string>();

                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            string newHref = match.Groups[2].Value;
                            if (newHref.IndexOf(rootHref) == -1)
                            {
                                if (newHref.IndexOf("//") == -1)
                                    newHref = rootHref + ((newHref.StartsWith("/")) ? newHref.Substring(1) : newHref);
                                else
                                    continue;
                            }

                            newUrls.Add(newHref);
                        }
                    }

                    t.answer = newUrls.ToArray<string>();
                    string fileName = t.destination + t.url.Replace(rootHref, "");
                    if (Path.GetFileName(fileName) == "")
                        fileName += "index.html";
                    
                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                        File.WriteAllText(fileName, responseFromServer);
                    }
                    catch (Exception e)
                    {

                    }
                }

                // Clean up the streams and the response.
                response.Close();
            }
            catch (Exception e)
            {
                t.answer = new string[0];
            }

            MQueue.SendSubTask(t, MQueue.ConnectionAnswer);
        }

        override protected void CreateSubTasks(int numberOfParall)
        {
            SubTask hk = new SubTask { url = this.url, id = this.id, type = this.GetType().ToString(), destination = this.destination};
            this.subtasks.Add(hk);
            downloaded.Add(url);
        }

        override public void group()
        {
            List<SubTask> newTasks = new List<SubTask>();

            foreach (SubTask st in this.subtasks)
                if (st.answer != null)
                {
                    string[] urls = st.answer;

                    foreach (string s in urls)
                        if (this.downloaded.Count<string>(x => (x == s)) == 0)
                        {
                            SubTask hk = new SubTask { url = s, id = this.id, type = this.GetType().ToString(), destination = this.destination };
                            newTasks.Add(hk);
                        }
                }

            foreach (SubTask t in newTasks)
            {
                this.subtasks.Add(t);
                MQueue.SendSubTask(t, MQueue.ConnectionTask);
                downloaded.Add(t.url);
            }


            if (IsAllSubTasksExecuted())
                this.result = (object)"Somehow it is done!";
        }

    }
}
