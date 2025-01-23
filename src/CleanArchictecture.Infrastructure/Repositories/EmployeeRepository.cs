using CleanArchictecture.Domain.Employees;
using CleanArchictecture.Infrastructure.Context;
using GenericRepository;

namespace CleanArchictecture.Infrastructure.Repositories
{
    internal sealed class EmployeeRepository : Repository<Employee, ApplicationDbContext>, IEmployeeRepository
    {
        //Repositoryde ana işlemler yapılır. 
        //Service'de gelen işlemleri takip eder, uniq mi kontrol eder .
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
