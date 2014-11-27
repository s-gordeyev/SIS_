using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLib
{
    public enum Tasks { ARCHIVESITE};

    abstract public class Task
    {
        public List<SubTask> subtasks = new List<SubTask>();
        public object result = null;
        public int id;
        public string url;
        public Tasks type { get; private set; }

        public Task()
        {
            this.id = -1;
            this.url = "";
        }

        public Task(string url, int id, Tasks t)
        { 
            this.id = id;
            this.url = url;
            this.type = t;
        }

        public static Task CreateTask(Tasks t, string url, int numberOfParall, int id, string destination)
        {
            if (numberOfParall < 1)
                numberOfParall = 1;

            Task ans = null;

            switch (t) 
            {
                case Tasks.ARCHIVESITE:
                    ans = new TaskArchiveSite(url, id, t, destination);
                    break;
            }

            ans.CreateSubTasks(numberOfParall);

            return ans;
        }

        abstract public void group();

        abstract protected void CreateSubTasks(int numberOfParall);

        public bool IsAllSubTasksExecuted()
        {
            return (this.subtasks.Where<SubTask>(x => x.answer != null).Count<SubTask>() == this.subtasks.Count);
        }

        public void SetResultOfSubTask(SubTask st)
        {
            this.subtasks.FirstOrDefault<SubTask>(x => (x.url == st.url && x.id == st.id)).answer = st.answer;
        }
    }
}
