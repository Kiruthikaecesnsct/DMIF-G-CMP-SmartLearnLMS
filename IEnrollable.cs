using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public interface IEnrollable
    {
        void Enroll(Student student);
        void Drop(Student student);
        bool CanEnroll(Student student);
        int GetAvailableSeats();
    }

}
