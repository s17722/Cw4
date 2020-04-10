using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentsDal
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string indexNumber);

        //Zad 4.2
        public IEnumerable<Zad4> GetStudentsByEnrollment();

        //Zad 4.3
        public Zad4 GetStudentBySemester(string indexNumber);
    }
}
