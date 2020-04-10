using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]

    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17722;Integrated Security=True";
        //private readonly IDbService _dbService;

        /*
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
         */

        private readonly IStudentsDal _dbStudentService;

        public StudentsController(IStudentsDal dbService)
        {
            _dbStudentService = dbService;
        }

        //ZAD 4.2
        [HttpGet]
        public IActionResult GetStudentByEnrollment()
        {
            return Ok(_dbStudentService.GetStudentsByEnrollment());
        }

        //ZAD 4.3
        [HttpGet("{indexNumber}")]
        public IActionResult GetStudentBySemester(string indexNumber)
        {
            return Ok(_dbStudentService.GetStudentBySemester(indexNumber));
        }


        /*
        [HttpGet]
        public IActionResult GetStudents([FromServices] IStudentsDal _dbStudentService)
        {
            return Ok(_dbStudentService.GetStudents());

        }
        

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            return Ok(_dbStudentService.GetStudent(indexNumber));
        }


        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(Student s, int id)
        {
            _dbService.UpdateStudent(s, id);
            return Ok("Aktualizacja ukonczona");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(int id)
        {

            _dbService.RemoveStudent(id);

            return Ok("Usuwanie zakonczono");
        }
        */


    }
}