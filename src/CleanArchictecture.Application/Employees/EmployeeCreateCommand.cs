using CleanArchictecture.Domain.Employees;
using FluentValidation;
using GenericRepository;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using TS.Result;

namespace CleanArhictecture_2025.Application.Employees;
public sealed record EmployeeCreateCommand(
    string FirstName,
    string LastName,
    DateOnly BirthOfDate,
    decimal Salary,
    PersonelInformation PersonelInformation,
    Address? Address) : IRequest<Result<string>>;

internal sealed class EmployeeCreateCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork, ILogger<EmployeeCreateCommandHandler> logger) : IRequestHandler<EmployeeCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("📌 Employee ekleme işlemi başladı: TCNo={TCNo}", request.PersonelInformation.TCNo);

        try
        {
            // Employee zaten var mı kontrolü
            var isEmployeeExists = await employeeRepository.AnyAsync(p => p.PersonelInformation.TCNo == request.PersonelInformation.TCNo, cancellationToken);

            if (isEmployeeExists)
            {
                logger.LogWarning("❌ Bu TC numarası daha önce kaydedilmiş: TCNo={TCNo}", request.PersonelInformation.TCNo);
                return Result<string>.Failure("Bu TC numarası daha önce kaydedilmiş");
            }

            // Yeni Employee oluşturma
            Employee employee = request.Adapt<Employee>();
            employeeRepository.Add(employee);

            // Veritabanı değişikliklerini kaydetme
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("✅ Employee başarıyla eklendi: EmployeeId={EmployeeId}", employee.Id);
            return Result<string>.Succeed("Personel kaydı başarıyla tamamlandı");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Employee ekleme işlemi sırasında hata oluştu: TCNo={TCNo}", request.PersonelInformation.TCNo);
            return Result<string>.Failure("Bir hata oluştu, lütfen tekrar deneyin.");
        }
    }
}