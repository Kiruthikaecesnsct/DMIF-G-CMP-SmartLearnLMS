using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Interfaces
{
    public interface IEnrollable
    {
        int MaxStudents { get; }
        bool CanEnroll(Student student);
        void Enroll(Student student);
        void Drop(Student student);
        }

}
