using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17722;Integrated Security=True";

        private readonly IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [HttpPost]

        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            _service.EnrollStudent(request);

            if (request.Studies == null)
            {
                return BadRequest("Studia nie istnieja");
            }

            if (request.IndexNumber == null)
            {
                return BadRequest("Indeks nie ma unikatowego numeru");
            }

            var response = new EnrollStudentResponse();

            response.LastName = request.LastName;
            response.Semester = 1;
            response.StartDate = DateTime.Now;

            return Ok(response);
        }
    }
}