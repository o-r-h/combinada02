using System.Collections.Concurrent;

namespace Veritrade2018.Helpers
{
    public static class ActiveSessions
    {
        private static ConcurrentDictionary<string, OnlineUsers> _sessionInfo;
        private static readonly object padlock = new object();

        public static ConcurrentDictionary<string, OnlineUsers> Sessions
        {
            get
            {
                lock (padlock)
                {
                    if (_sessionInfo == null)
                    {
                        _sessionInfo = new ConcurrentDictionary<string, OnlineUsers>();
                    }
                    return _sessionInfo;
                }
            }
        }

        public static int Count
        {
            get
            {
                lock (padlock)
                {
                    if (_sessionInfo == null)
                    {
                        _sessionInfo = new ConcurrentDictionary<string, OnlineUsers>();
                    }
                    return _sessionInfo.Count;
                }
            }
        }
    }

    public class OnlineUsers
    {
        public string IdSesion { get; set; }
        public string IdUsuario { get; set; }

        public OnlineUsers()
        {
            IdSesion = "";
            IdUsuario = null;
        }
    }
}