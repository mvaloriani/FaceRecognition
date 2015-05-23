using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.Data
{
    public enum SessionStatus { 
        Running,
        Completed,
        Interrupted
    }

    public class Session
    {
        public int numberOfUsers { get; private set; }
        public Guid sessionID { get; private set; }
        public DateTime startTime { get; private set; }
        public DateTime endTime {get;set;}

        public SessionStatus status { get; set; }

        public List<UserProfile> users {get; set;}
        public List<float> usersInterest;

        public Session(List<UserProfile> users)
        {
            sessionID = Guid.NewGuid();
            startTime = DateTime.Now;
            this.users = users;
            numberOfUsers = users.Count();
            usersInterest = new List<float>();
            status = SessionStatus.Running;
        }

    }
}
