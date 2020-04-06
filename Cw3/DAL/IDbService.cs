using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;

namespace Cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();

        public void AddStudent(Student s);

        public void RemoveStudent(int id);

        public void UpdateStudent(Student s, int id);

        public Student GetStudent(int id);

        public int ListLength();
    }
}
