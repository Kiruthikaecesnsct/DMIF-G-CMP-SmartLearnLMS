using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Interfaces;

namespace Week_1.Services
    {
    public class NotificationService
        {
        public void NotifyEnrollment(INotifiable user, string courseTitle)
            {
            user.SendNotification($"You've been enrolled in {courseTitle}!");
            }

        public void NotifyProgress(INotifiable user, double progress)
            {
            if (progress >= 100)
                user.SendNotification("🎉 Congratulations! You completed the course!");
            else if (progress >= 50)
                user.SendNotification($"Great work! You're {progress}% complete!");
            }

        public void NotifyAll(List<INotifiable> users, string message)
            {
            foreach (var user in users)
                user.SendNotification(message);

            Console.WriteLine($"📢 Sent to {users.Count} users");
            }
        }
    }
