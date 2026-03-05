using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Interfaces
    {
    public interface INotifiable
        {
        void SendNotification(string message);
        List<string> GetNotificationHistory();
        int UnreadNotificationCount { get; }
        }
    }
