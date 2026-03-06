using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Services
    {
    /// <summary>
    /// Single Responsibility: handles all notification logic.
    /// Works against INotifiable — not tied to Student or Instructor specifically.
    /// </summary>
    public class NotificationService
        {
        public void NotifyEnrollment(INotifiable notifiable, string courseTitle)
            {
            notifiable.SendNotification($"You have been enrolled in '{courseTitle}'.");
            }

        public void NotifyProgressMilestone(INotifiable notifiable, int progress)
            {
            if (progress >= 100)
                notifiable.SendNotification("🎉 Congratulations! You completed a course!");
            else if (progress >= 75)
                notifiable.SendNotification($"Great progress! You're {progress}% through your course.");
            else if (progress >= 50)
                notifiable.SendNotification($"Halfway there! Keep going — {progress}% complete.");
            }

        public void NotifyAll(IEnumerable<INotifiable> notifiables, string message)
            {
            foreach (var n in notifiables)
                n.SendNotification(message);
            }

        public void DisplayHistory(INotifiable notifiable)
            {
            var history = notifiable.GetNotificationHistory();
            if (history.Count == 0)
                {
                Console.WriteLine("  No notifications.");
                return;
                }
            Console.WriteLine($"  📬 Notification History ({history.Count}):");
            foreach (var msg in history)
                Console.WriteLine($"    {msg}");
            }
        }
    }
