using CleanArchictecture.Domain.Employees;
using CleanArchictecture.Infrastructure.Context;
using CleanArchictecture.Infrastructure.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace CleanArchictecture.Infrastructure
{
    public static  class InfrastructureRegistrar
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("SqlServer")!;
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());
            
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // AddScoped<IEmployeeRepository, EmployeeRepository>() gibi kayıtları tek tek yapmanıza gerek  kalmaz.Scrutor,otomatik olarak EmployeeRepository sınıfını IEmployeeRepository ile ilişkilendirir.

            //AsImplementedInterfaces() metodu sayesinde, bir sınıfın implement ettiği tüm arayüzler DI konteynerine eklenir.

            //Tüm sınıflar için belirli bir yaşam döngüsü(Scoped, Singleton, Transient) belirleyebilirsiniz.
            services.Scan(opt => opt
            .FromAssemblies(typeof(InfrastructureRegistrar).Assembly)
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            );
           
            return services;
        }
       
    }
}
