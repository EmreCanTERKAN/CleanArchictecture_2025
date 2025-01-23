using CleanArchictecture.Domain.Abstractions;
using CleanArchictecture.Domain.Employees;
using MediatR;

namespace CleanArchictecture.Application.Employees;
public sealed record EmployeeGetAllQuery() : IRequest<IQueryable<EmployeeGetAllQueryResponse>>;

public sealed class EmployeeGetAllQueryResponse : EntityDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateOnly BirthOfDate { get; set; } = default!;
    public decimal Salary { get; set; }
    public string TcNo { get; set; } = default!;
}


internal sealed class EmployeeGetAllQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<EmployeeGetAllQuery, IQueryable<EmployeeGetAllQueryResponse>>
{
    public Task<IQueryable<EmployeeGetAllQueryResponse>> Handle(EmployeeGetAllQuery request, CancellationToken cancellationToken)
    {
        var response = employeeRepository.GetAll()
            .Select(s => new EmployeeGetAllQueryResponse
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
                Salary = s.Salary,
                BirthOfDate = s.BirthOfDate,
                DeleteAt = s.DeleteAt,
                Id = s.Id,
                IsDeleted = s.IsDeleted,
                TcNo = s.PersonelInformation.TCNo,
            })
            .AsQueryable();
        return Task.FromResult(response);
    }
}
