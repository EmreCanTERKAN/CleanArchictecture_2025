using CleanArchictecture.Domain.Employees;
using CleanArchictecture.Infrastructure.Context;
using CleanArchictecture.Infrastructure.Repositories;
using CleanArhictecture_2025.Domain.Users;
using GenericRepository;
using Microsoft.AspNetCore.Identity;
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

            //identity (usermanager)
            services
                .AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
                {
                    opt.Password.RequiredLength = 1;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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
