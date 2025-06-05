using DeployProject.Data;
using DeployProject.Models;
using DeployProject.Models.Entities;
using DeployProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeployProject.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ApplicationDbContext context, ILogger<EmployeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Employee> GetAll()
        {
            _logger.LogInformation("Attempting to fetch all employees");

            try
            {
                var employees = _context.Employees.ToList();
                _logger.LogInformation("Successfully fetched {Count} employees", employees.Count);
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching employees");
                throw;
            }
        }

        public Employee? GetById(Guid id)
        {
            _logger.LogInformation("Fetching employee with ID: {Id}", id);

            try
            {
                var employee = _context.Employees.Find(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID: {Id} not found", id);
                }
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching employee with ID: {Id}", id);
                throw;
            }
        }

        public void Add(Employee dto)
        {
            _logger.LogInformation("Adding a new employee: {Name}", dto.Name);

            try
            {
                var employee = new Employee
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Salary = dto.Salary,
                    ProfileImageUrl = dto.ProfileImageUrl,

                };

                _context.Employees.Add(employee);
                _context.SaveChanges();

                _logger.LogInformation("Successfully added employee: {Name}", dto.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new employee: {Name}", dto.Name);
                throw;
            }
        }

        public void Update(Guid id, UpdateEmployeeDto dto)
        {
            _logger.LogInformation("Updating employee with ID: {Id}", id);

            try
            {
                var employee = _context.Employees.Find(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID: {Id} not found for update", id);
                    throw new Exception("Employee not found.");
                }

                employee.Name = dto.Name;
                employee.Email = dto.Email;
                employee.Phone = dto.Phone;
                employee.Salary = dto.Salary;

                _context.SaveChanges();

                _logger.LogInformation("Successfully updated employee with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating employee with ID: {Id}", id);
                throw;
            }
        }

        public void Delete(Guid id)
        {
            _logger.LogInformation("Attempting to delete employee with ID: {Id}", id);

            try
            {
                var employee = _context.Employees.Find(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID: {Id} not found for deletion", id);
                    throw new Exception("Employee not found.");
                }

                _context.Employees.Remove(employee);
                _context.SaveChanges();

                _logger.LogInformation("Successfully deleted employee with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting employee with ID: {Id}", id);
                throw;
            }
        }
    }
}
