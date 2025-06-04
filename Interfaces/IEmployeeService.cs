using DeployProject.Models;
using DeployProject.Models.Entities;

namespace DeployProject.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetAll();
        Employee? GetById(Guid id);
        void Add(AddEmployeeDto employeeDto);
        void Addd(Employee employee);
        void Update(Guid id, UpdateEmployeeDto employeeDto);
        void Delete(Guid id);
    }
}
