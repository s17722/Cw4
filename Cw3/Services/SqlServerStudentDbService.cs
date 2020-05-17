using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17722;Integrated Security=True";

        public void EnrollStudent(EnrollStudentRequest request)
        {

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    //1. Czy studia istnieja
                    com.CommandText = "select IdStudy from studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    com.Transaction = tran;

                    SqlDataReader dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        request.Studies = null;
                        tran.Rollback();
                    }
                    else
                    {
                        int idStudies = (int)dr["IdStudy"];
                        dr.Close();

                        com.CommandText = "select IdStudy from enrollment where idstudy=@idstudies AND semester=1";
                        com.Parameters.AddWithValue("idstudies", idStudies);
                        dr = com.ExecuteReader();

                        if (!dr.Read())
                        {
                            dr.Close();
                            com.CommandText = "select TOP 1 * from Enrollment ORDER BY IdEnrollment DESC";
                            dr = com.ExecuteReader();

                            if (dr.Read())
                            {
                                int nextIdEnrollment = (int)dr["IdEnrollment"];
                                nextIdEnrollment += 1;
                                dr.Close();
                                com.CommandText = "Insert into Enrollment values(@idEnrollment,@semester,@idStudy,@startDate)";
                                com.Parameters.AddWithValue("idEnrollment", nextIdEnrollment);
                                com.Parameters.AddWithValue("semester", 1);
                                com.Parameters.AddWithValue("idStudy", idStudies);
                                com.Parameters.AddWithValue("startDate", DateTime.Now);

                                com.ExecuteNonQuery();
                            }
                        }

                        dr.Close();

                        com.CommandText = "select IndexNumber from Student where IndexNumber=@indexNumber";
                        com.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                        dr = com.ExecuteReader();
                        if (dr.Read())
                        {
                            request.IndexNumber = null;
                            dr.Close();
                            tran.Rollback();
                        }

                        dr.Close();
                        //INSERT STUDENT
                        //CHECK IDENROLLMENT
                        if (request.IndexNumber != null)
                        {
                            com.CommandText = "select TOP 1 * from Enrollment where semester = 1 ORDER BY IdEnrollment DESC";
                            dr = com.ExecuteReader();
                            int lastIdEnrollment = 1;
                            if (dr.Read())
                            {
                                lastIdEnrollment = (int)dr["IdEnrollment"];
                            }
                            Console.WriteLine("req ind num = " + request.IndexNumber);
                            dr.Close();
                            //INSERT INTO STUDENT TABLE
                            DateTime date = Convert.ToDateTime(request.BirthDate);
                            com.CommandText = "Insert into Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment)values(@index,@firstName,@lastName,@birthDate,@idEnrollmentStudent)";
                            com.Parameters.AddWithValue("index", request.IndexNumber);
                            com.Parameters.AddWithValue("firstName", request.FirstName);
                            com.Parameters.AddWithValue("lastName", request.LastName);
                            com.Parameters.AddWithValue("birthDate", date);
                            com.Parameters.AddWithValue("idEnrollmentStudent", lastIdEnrollment);

                            com.ExecuteNonQuery();

                            tran.Commit();
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine(exc.Message);
                    tran.Rollback();
                }
            }

        }

        public StudentCw5 GetStudent(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student where indexNumber=@index";

                SqlParameter par = new SqlParameter();
                par.Value = indexNumber;
                par.ParameterName = "index";
                com.Parameters.Add(par);

                con.Open();

                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new StudentCw5();

                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    DateTime date = Convert.ToDateTime(dr["BirthDate"].ToString());
                    st.BirthDate = date;
                    st.Studies = dr["Studies"].ToString();
                    return st;
                }
            }
            return null;
        }

        public Enrollment PromoteStudents(int semester, int studies)
        {

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                try
                {
                    com.CommandText = "CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT" +
                        " AS" +
                        " BEGIN" +
                        " SET XACT_ABORT ON;" +
                        " BEGIN TRAN" +
                        " DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name = @Studies);" +
                        " IF @IdStudies IS NULL" +
                        " BEGIN" +
                        " RAISERROR('404 Not Found', 15, 1);" +
                        " RETURN;" +
                        " END" +
                        " DECLARE @IdStudiesFromEnrollment INT = (SELECT IdStudy FROM Enrollment WHERE IdStudy = @IdStudies);" +
                        " IF @IdStudiesFromEnrollment IS NULL" +
                        " BEGIN" +
                        " RAISERROR('404 Not Found', 15, 1);" +
                        " RETURN;" +
                        " END" +
                        " DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester = @Semester);" +
                        " IF @IdEnrollment IS NULL" +
                        " BEGIN" +
                        " RAISERROR('404 Not Found', 15, 1);" +
                        " RETURN;" +
                        " END" +
                        " DECLARE @IdNextSemester INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1));" +
                        " IF @IdNextSemester IS NULL" +
                        " BEGIN" +
                        " DECLARE @index AS INT = (SELECT MAX(IdEnrollment) FROM Enrollment) +1;" +
                        " INSERT INTO Enrollment VALUES(@index, @IdNextSemester, @IdStudies, GETDATE())" +
                        " END" +
                        " UPDATE Student SET IdEnrollment = @index" +
                        " WHERE IdEnrollment = @IdEnrollment" +
                        " COMMIT" +
                        " END;"; ;

                    SqlParameter parStudies = new SqlParameter();
                    parStudies.Value = semester;
                    parStudies.ParameterName = "Studies";
                    com.Parameters.Add(parStudies);

                    SqlParameter parSemester = new SqlParameter();
                    parSemester.Value = semester;
                    parSemester.ParameterName = "Semester";
                    com.Parameters.Add(parSemester);

                    com.ExecuteNonQuery();


                    com.CommandText = "select * from Enrollment where Semester=@nextSemester AND IdStudy=@idStudies";
                    com.Parameters.AddWithValue("nextSemester", semester + 1);
                    com.Parameters.AddWithValue("idStudies", studies);
                    var dr = com.ExecuteReader();

                    if (dr.Read())
                    {
                        var enr = new Enrollment();

                        enr.IdEnrollment = dr.GetInt32(0);
                        enr.Semester = dr.GetInt32(1);
                        enr.IdStudy = dr.GetInt32(2);
                        enr.StartDate = dr["StartDate"].ToString();

                        return enr;
                    }
                } catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
                return null;
            }


        }
    }
}
