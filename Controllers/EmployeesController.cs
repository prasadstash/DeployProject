using DeployProject.Data;
using DeployProject.Models;
using DeployProject.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeployProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                var employees = _dbContext.Employees.ToList();
                if (employees == null || !employees.Any())
                {
                    return NotFound("No employees found.");
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                // _logger.LogError(ex, "An error occurred while retrieving employees.");

                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult AddEmployee(AddEmployeeDto addEmployeeDto)
        {
            var employeeEntity = new Employee()
            {
                Name = addEmployeeDto.Name,
                Email = addEmployeeDto.Email,
                Phone = addEmployeeDto.Phone,
                Salary = addEmployeeDto.Salary
            };

            _dbContext.Employees.Add(employeeEntity);
            _dbContext.SaveChanges();
            return Ok(addEmployeeDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            var Emp = _dbContext.Employees.Find(id);
            if (Emp is null)
            {
                return NotFound();
            }
            return Ok(Emp);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
        {
            var updateEmployee = _dbContext.Employees.Find(id);
            if (updateEmployee is null)
            {
                return NotFound();
            }
            else
            {
                updateEmployee.Name = updateEmployeeDto.Name;
                updateEmployee.Email = updateEmployeeDto.Email;
                updateEmployee.Salary = updateEmployeeDto.Salary;
                updateEmployee.Phone = updateEmployeeDto.Phone;

                _dbContext.SaveChanges();
                return Ok(updateEmployee);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var deleteEmployee = _dbContext.Employees.Find(id);
            if (deleteEmployee is null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Employees.Remove(deleteEmployee);
                _dbContext.SaveChanges();
                return Ok();
            }
        }




    }
}

/*
 Debug
Information
Warning
Error
Critical 
these are the message of the logggeer method
 
 
 */
