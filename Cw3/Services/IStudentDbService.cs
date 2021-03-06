﻿using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentDbService
    {
        StudentCw5 GetStudent(string index);
        public void EnrollStudent(EnrollStudentRequest request);
        public Enrollment PromoteStudents(int semester, int studies);
    }
}
