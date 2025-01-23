using CleanArhictecture_2025.Application.Employees;
using FluentValidation;
namespace CleanArchictecture.Application.Employees;

public sealed class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
{
    public EmployeeCreateCommandValidator()
    {
        RuleFor(x => x.FirstName).MinimumLength(3).WithMessage("Ad alanı en az 3 karakter olcak.");
        RuleFor(x => x.LastName).MinimumLength(3).WithMessage("Soyad alanı en az 3 karakter olcak.");
        RuleFor(x => x.PersonelInformation.TCNo)
            .Length(11).WithMessage("TC No 11 Karakter olmalıdır");
    }
}
