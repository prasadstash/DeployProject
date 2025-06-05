using Azure.Storage.Blobs;
using DeployProject.Interfaces;
using DeployProject.Models;
using DeployProject.Models.Entities;
using DeployProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace DeployProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IBlobService _blobService;
        //private readonly BlobServiceClient _accountclient;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogger<EmployeesController> logger,
            IBlobService blobService)
        {
            _employeeService = employeeService;
            _logger = logger;
            _blobService = blobService;
            //_connectionString = configuration["AzureStorage:ConnectionString"];
            //_accountclient  = new BlobServiceClient(connectionString);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var employees = _employeeService.GetAll();
                return employees.Any() ? Ok(employees) : NotFound("No employees found.");
            }
            catch (Exception ex)
            {
                return HandleError("fetching all employees", ex);
            }
        }
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var emp = _employeeService.GetById(id);

                if (emp == null)
                    return NotFound($"Employee with ID {id} not found.");

                if (!string.IsNullOrEmpty(emp.ProfileImageUrl))
                {
                    var fileName = Path.GetFileName(emp.ProfileImageUrl);
                    var tempUrl = _blobService.GetTemporaryAccessUrl(fileName);
                    emp.ProfileImageUrl = tempUrl;
                }

                return Ok(emp);
            }
            catch (Exception ex)
            {
                return HandleError($"fetching employee with ID: {id}", ex);
            }
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Add([FromBody] AddEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = new Employee
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Salary = dto.Salary,
                };
                _employeeService.Add(employee);
                return Ok("Employee added successfully");
            }
            catch (Exception ex)
            {
                return HandleError("adding new employee", ex);
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Update(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _employeeService.Update(id, dto);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return HandleError($"updating employee with ID: {id}", ex, true);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _employeeService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return HandleError($"deleting employee with ID: {id}", ex, true);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] AddEmployeeDto dto, IFormFile profileImage)
        {
            try
            {
                string imageUrl = "";
                if (profileImage != null)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
                    imageUrl = await _blobService.UploadFileAsync(profileImage, fileName);
                    //var x = _blobService.get(,)
                }

                var employee = new Employee
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Salary = dto.Salary,
                    ProfileImageUrl = imageUrl
                };

                _employeeService.Add(employee);  


                return Ok(employee);
            }
            catch (Exception ex)
            {
                return HandleError("uploading employee profile", ex);
            }
        }

        private IActionResult HandleError(string action, Exception ex, bool notFoundOnCustomEx = false)
        {
            _logger.LogError(ex, "Error occurred while {Action}", action);

            // Custom logic: convert known messages into NotFound
            if (notFoundOnCustomEx && ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(ex.Message);

            return StatusCode(500, "Internal server error");
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
