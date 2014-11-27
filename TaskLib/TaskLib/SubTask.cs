using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLib
{
    [Serializable]
    public class SubTask
    {
        public string url { get; set; }
        public int id { get; set; }
        public string[] answer { get; set; }
        public string type {get; set; }
        public string destination { get; set; }

        public SubTask(string url)
        {
            this.url = url;
            this.answer = null;
            this.type = "";
        }

        public SubTask() {
            this.url = "";
            this.answer = null;
            this.type = "";
        }

        public void Run()
        {
            Type t = Type.GetType(this.type);
            t.GetMethod("run").Invoke(null, new object[] { this });
        }
    }
}
