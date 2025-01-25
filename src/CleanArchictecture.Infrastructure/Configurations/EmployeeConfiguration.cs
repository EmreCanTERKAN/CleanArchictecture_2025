using CleanArchictecture.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CleanArchictecture.Infrastructure.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    //Veri tabanında bu verilerin karşılığı için tabloya ekliyoruz.
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.OwnsOne(p => p.PersonelInformation, builder =>
        {
            builder.Property(i => i.TCNo).HasColumnName("TCNO");
            builder.Property(i => i.Phone1).HasColumnName("Phone1");
            builder.Property(i => i.Phone2).HasColumnName("Phone2");
            builder.Property(i => i.Email).HasColumnName("Email");
        });

        builder.OwnsOne(p => p.Address, builder =>
        {
            builder.Property(i => i.Country).HasColumnName("Country");
            builder.Property(i => i.City).HasColumnName("City");
            builder.Property(i => i.Town).HasColumnName("Town");
            builder.Property(i => i.FullAddress).HasColumnName("FullAddress");
        });

        //decimal içinde warning vermemesi için bunu yapmamız gerekmektedir.

        builder.Property(i => i.Salary).HasColumnType("money");
    }
}
