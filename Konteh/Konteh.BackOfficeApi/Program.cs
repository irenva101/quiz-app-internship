using FluentValidation;
using Konteh.BackOfficeApi.Infrastructure;
using Konteh.Domain;
using Konteh.Infrastructure;
using Konteh.Infrastructure.Behaviors;
using Konteh.Infrastructure.ExceptionHandlers;
using Konteh.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Reflection;

namespace Konteh.BackOfficeApi;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddOpenApiDocument(options => options.SchemaSettings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator());

        var connectionString = builder.Configuration.GetConnectionString("KontehDB");

        builder.Services.AddDbContext<KontehContext>(options =>
                        options.UseSqlServer(connectionString));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        builder.Services.AddScoped<IRepository<Question>, QuestionRepository>();
        builder.Services.AddScoped<IRepository<Answer>, AnswerRepository>();
        builder.Services.AddScoped<IRepository<Exam>, ExamRepository>();
        builder.Services.AddScoped<IRepository<Candidate>, CandidateRepository>();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

        builder.Services.AddProblemDetails();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddCors(o =>
        {
            o.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors("AllowAll");

        app.UseExceptionHandler();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.UseOpenApi();
        app.UseSwaggerUi();

        app.Run();
    }
}