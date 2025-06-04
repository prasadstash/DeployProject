using DeployProject.Interfaces;
using DeployProject.Models;
using DeployProject.Models.Entities;
using DeployProject.Services;
using DeployProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeployProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IBlobService _blobService;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger, IBlobService blobService)
        {
            _employeeService = employeeService;
            _logger = logger;
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            _logger.LogInformation("Received request to fetch all employees");

            try
            {
                var employees = _employeeService.GetAll();

                if (!employees.Any())
                {
                    _logger.LogWarning("No employees found in database");
                    return NotFound("No employees found.");
                }

                _logger.LogInformation("Successfully fetched {Count} employees", employees.Count());
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            _logger.LogInformation("Received request to fetch employee with ID: {Id}", id);

            try
            {
                var emp = _employeeService.GetById(id);

                if (emp == null)
                {
                    _logger.LogWarning("Employee with ID {Id} not found", id);
                    return NotFound($"Employee with ID {id} not found.");
                }

                _logger.LogInformation("Employee with ID {Id} found", id);
                return Ok(emp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddEmployee([FromBody] AddEmployeeDto dto)
        {
            _logger.LogInformation("Received request to add a new employee");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddEmployeeDto");
                return BadRequest(ModelState);
            }

            try
            {
                _employeeService.Add(dto);
                _logger.LogInformation("Successfully added employee: {Name}", dto.Name);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee: {Name}", dto.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            _logger.LogInformation("Received request to update employee with ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateEmployeeDto");
                return BadRequest(ModelState);
            }

            try
            {
                _employeeService.Update(id, dto);
                _logger.LogInformation("Successfully updated employee with ID: {Id}", id);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {Id}", id);
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeleteEmployee(Guid id)
        {
            _logger.LogInformation("Received request to delete employee with ID: {Id}", id);

            try
            {
                _employeeService.Delete(id);
                _logger.LogInformation("Successfully deleted employee with ID: {Id}", id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {Id}", id);
                return NotFound(ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadEmployee([FromForm] AddEmployeeDto dto, IFormFile profileImage)
        {
            try
            {
                string imageUrl = null;

                if (profileImage != null)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
                    imageUrl = await _blobService.UploadFileAsync(profileImage, fileName);
                }

                var employee = new Employee
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Salary = dto.Salary,
                    ProfileImageUrl = imageUrl
                };

                _employeeService.Addd(employee); // Modify your Add method to accept Employee instead of DTO

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading employee profile");
                return StatusCode(500, "Internal server error");
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
