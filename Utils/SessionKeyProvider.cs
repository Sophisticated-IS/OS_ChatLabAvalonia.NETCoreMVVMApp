using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Utils
{
    public sealed class SessionKeyProvider
    {
        private Timer timer;
        private const int expiredLoginTime = 30;
    
        
        /// Login,Key, Granted key session time
        private readonly List<Tuple<string, string, DateTime>> _sessionKeys;
        
        public SessionKeyProvider()
        {
            _sessionKeys = new List<Tuple<string, string, DateTime>>();
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Elapsed+= TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var sessionKey in _sessionKeys.Where(sessionKey 
                => DateTime.Now.Subtract(sessionKey.Item3).TotalMinutes > expiredLoginTime).ToArray())
            {
                _sessionKeys.Remove(sessionKey);
            }
        }

        public string GenerateKey(string userLogin)
        {
            if (userLogin == null) throw new ArgumentNullException(nameof(userLogin));
            
            var sessionKey = Guid.NewGuid().ToString();
            _sessionKeys.Add(new Tuple<string, string, DateTime>(userLogin,sessionKey , DateTime.Now));

            return sessionKey;
        }

        public void DeleteSessionKey(string sessionKey)
        {
            if (sessionKey == null) throw new ArgumentNullException(nameof(sessionKey));
            
            var res = _sessionKeys.Single(x => x.Item2 == sessionKey);
            _sessionKeys.Remove(res);
        }

        public string GetLoginBySessionKey(string sessionKey)
        {
            if (sessionKey == null) throw new ArgumentNullException(nameof(sessionKey));
            
            var login =_sessionKeys.Single(key => key.Item2 == sessionKey);
            return login.Item1;
        }
    }
}