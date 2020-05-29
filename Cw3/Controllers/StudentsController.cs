using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.DTOs.Requests;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]

    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }

        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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
        [Authorize (Roles = "employee")]

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

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier,"1"),
                new Claim(ClaimTypes.Name, "Jan"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
           (
               issuer: "Gakko",
               audience: "Students",
               claims: claims,
               expires: DateTime.Now.AddMinutes(10),
               signingCredentials: creds
           );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
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