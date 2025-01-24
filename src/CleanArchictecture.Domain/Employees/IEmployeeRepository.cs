using GenericRepository;

namespace CleanArchictecture.Domain.Employees;

//Ana işlemler genelde burada yapılır. Create / Update / Delete/ Get

public interface IEmployeeRepository : IRepository<Employee>
{
}
