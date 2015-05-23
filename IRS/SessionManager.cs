using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using IRS.Data;
using Microsoft.Kinect;

namespace IRS
{
    class SessionManager : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private Session _actualSession;
        internal Session ActualSession
        {
            get { return _actualSession; }
            set
            {
                _actualSession = value;
                NotifyPropertyChanged("ActualSession");
            }
        }

        private readonly List<int> activeList = new List<int>();
        private IEnumerator<UserProfile> usersIterator;

        private TimeSpan maxTime4User = new TimeSpan(0, 1, 0);
        private DispatcherTimer _userTimer;
        public DispatcherTimer UserTimer
        {
            get
            {
                if (_userTimer == null)
                {
                    _userTimer = new DispatcherTimer();
                    _userTimer.Tick += new EventHandler(dispatcherTimerUserTick);
                    _userTimer.Interval = maxTime4User;
                }
                return _userTimer;
            }
            set
            {
                if (value == null && _userTimer != null)
                {
                    _userTimer.Tick -= new EventHandler(dispatcherTimerUserTick);
                    _userTimer.Stop();
                }
                _userTimer = value;
            }
        }


        private TimeSpan maxSessionTime = new TimeSpan(0, 5, 0);
        private DispatcherTimer _sessionTimer;
        public DispatcherTimer SessionTimer
        {
            get
            {
                if (_sessionTimer == null)
                {
                    _sessionTimer = new DispatcherTimer();
                    _sessionTimer.Tick += new EventHandler(dispatcherTimerSessionTick);
                    _sessionTimer.Interval = maxSessionTime;
                }
                return _sessionTimer;
            }
            set
            {
                if (value == null && _userTimer != null)
                {
                    _sessionTimer.Tick -= new EventHandler(dispatcherTimerSessionTick);
                    _sessionTimer.Stop();
                }
                _sessionTimer = value;
            }
        }


        private KinectSensor sensor;
        private Analizer _analizer;
        public Analizer Analizer
        {
            get { return _analizer; }
            set { _analizer = value; }
        }

        public SessionManager(KinectSensor sensor, Analizer analizer)
        {
            this._analizer = analizer;
            this.sensor = sensor;

        }



        public void RefreshUser(IEnumerable<Skeleton> skeletonDataValue)
        {
            int res = UpdateUsersList(skeletonDataValue);

            if (res > 0)
            {
                if (ActualSession != null)
                    stopSession();

                startNewSession();

            }
            if (res < 0)
            {
                stopSession();

                if (activeList.Count > 0)
                    startNewSession();
            }
        }

        private void ChooseNextActiveSkeletons()
        {
            if (Analizer.User != null && usersIterator.Current != null)
            {
                CopyDataUser(Analizer.User, usersIterator.Current);
                usersIterator.Current.EndTime = DateTime.Now;
            }

            if (usersIterator.MoveNext())
            {
                Analizer.User = usersIterator.Current;

                sensor.SkeletonStream.ChooseSkeletons(usersIterator.Current.UserTrakingID);
                NotifyPropertyChanged("UserTimer");

            }
            else
            {
                if (ActualSession.users.Count > 0)
                {
                    usersIterator.Reset();
                    ChooseNextActiveSkeletons();
                }

            }
        }

        private void CopyDataUser(UserProfile from, UserProfile to)
        {
            if (from != null && to != null && from.AttentionDatas.Count>0)
            {
                to.UserAge = from.UserAge;
                to.UserGender = from.UserGender;
                to.staticAnalisysPerformed = from.staticAnalisysPerformed;
                to.UserInterest = from.AttentionDatas.Average();
                to.AttentionDatas = from.AttentionDatas;
                to.MovementDatas = from.MovementDatas;
            }
        }

        private int UpdateUsersList(IEnumerable<Skeleton> skeletonDataValue)
        {
            List<int> newList = (from s in skeletonDataValue
                                 where s.TrackingState != SkeletonTrackingState.NotTracked
                                 select s.TrackingId).ToList();

            List<int> exitUsers = activeList.Where(k => !newList.Contains(k)).ToList();
            List<int> newUsers = newList.FindAll(k => !this.activeList.Contains(k)).ToList();

            // Remove all elements from the active list that are not currently present
            activeList.RemoveAll(k => exitUsers.Contains(k));
            // Add all elements that aren't already in the activeList
            this.activeList.AddRange(newUsers);

            if (exitUsers.Count > 0)
            {
                Debug.WriteLine("users exit");
                return exitUsers.Count * -1;
            }
            else if (newUsers.Count > 0)
            {
                Debug.WriteLine("users arrive");
                return newUsers.Count;
            }
            return 0;
        }



        void stopSession()
        {
            CopyDataUser(Analizer.User, usersIterator.Current);
            usersIterator.Current.EndTime = DateTime.Now;


            ActualSession.endTime = DateTime.Now;
            ActualSession.status = (ActualSession.endTime - ActualSession.startTime) >= maxSessionTime ?
                SessionStatus.Completed : SessionStatus.Interrupted;
            SerializerHelper.Serialize(_actualSession, typeof(Session), "C:/temp/" + _actualSession.sessionID.ToString() + ".txt", 1);

            ActualSession = null;
            usersIterator = null;
            UserTimer = null;
            SessionTimer = null;
        }

        void startNewSession()
        {
            List<UserProfile> users = new List<UserProfile>();
            foreach (var id in activeList)
            {
                users.Add(new UserProfile(id));
            }

            ActualSession = new Session(users.ToList());
            usersIterator = ActualSession.users.GetEnumerator();

            ChooseNextActiveSkeletons();

            UserTimer.Start();
            SessionTimer.Start();
            NotifyPropertyChanged("SessionTimer");



        }



        private void dispatcherTimerUserTick(object sender, EventArgs e)
        {
            ChooseNextActiveSkeletons();
        }

        private void dispatcherTimerSessionTick(object sender, EventArgs e)
        {
            stopSession();
            startNewSession();

        }



    }
}
