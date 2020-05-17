using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Cw3.Services
{
    public class SqlServerDbDal : IStudentsDal
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17722;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {
            var list = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    list.Add(st);
                }
            }
            return list;
        }

        //ZAD. 4.2

        public IEnumerable<Cw4> GetStudentsByEnrollment()
        {
            var list = new List<Cw4>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Student.FirstName, Student.LastName, Student.BirthDate, Studies.Name, Enrollment.Semester from Student join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment join Studies on Enrollment.IdStudy = Studies.IdStudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var zad = new Cw4();

                    zad.FirstName = dr["FirstName"].ToString();
                    zad.LastName = dr["LastName"].ToString();
                    zad.BirthDate = dr["BirthDate"].ToString();
                    zad.Name = dr["Name"].ToString();
                    zad.Semester = dr.GetInt32(4);

                    list.Add(zad);
                }
            }
            return list;
        }


        //Zad 4.3
        public Cw4 GetStudentBySemester(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Student.FirstName, Student.LastName, Student.BirthDate, Studies.Name, Enrollment.Semester from Student join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment join Studies on Enrollment.IdStudy = Studies.IdStudy where indexNumber=@index";

                SqlParameter par = new SqlParameter();
                par.Value = indexNumber;
                par.ParameterName = "index";
                com.Parameters.Add(par);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var zad = new Cw4();

                    zad.FirstName = dr["FirstName"].ToString();
                    zad.LastName = dr["LastName"].ToString();
                    zad.BirthDate = dr["BirthDate"].ToString();
                    zad.Name = dr["Name"].ToString();
                    zad.Semester = dr.GetInt32(4);

                    return zad;
                }
            }
            return null;

        }



        //Zad 4.4 i 4.5
        public Student GetStudent(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                //                com.CommandText = "select * from Student where indexNumber='"+indexNumber+"'";

                /*
                ZAD. 4.4 i 4.5
                Dla powyzszego sposobu przekazania paramatru mozna wykonac atak typu SQL-Injection:
                zamiast adresu URL można przekazać:
                localhost:5000/api/students/x';DROP TABLE Student; --

                Aby zabezpieczyc te luke, nalezy uzyc parametryzacji:
                */

                com.CommandText = "select * from Student where indexNumber=@index";

                SqlParameter par = new SqlParameter();
                par.Value = indexNumber;
                par.ParameterName = "index";
                com.Parameters.Add(par);

                con.Open();

                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    /*
                    if (dr["IndexNumber"] == DBNull.Value)
                    {

                    }
                    */

                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    return st;
                }
            }
            return null;
        }
    }
}
