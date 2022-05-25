using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joels_emulation_tools.Services
{
    internal class LoggingService
    {
        private List<Action<string>> _onLogWrite;

        private LoggingService() { 
        _onLogWrite = new List<Action<string>>();
        }
        
        private static readonly Lazy<LoggingService> lazy = new Lazy<LoggingService>(() => new LoggingService());
        
        public static LoggingService Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void Log(string message) {
            _onLogWrite.ForEach(x=> x.Invoke(message));   
        }

        public void Register(Action<string> onLogWrite) {
            _onLogWrite.Add(onLogWrite);
        }
    }
}
