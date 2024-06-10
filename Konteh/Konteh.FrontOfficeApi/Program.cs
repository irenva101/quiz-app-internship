using FluentValidation;
using Konteh.Domain;
using Konteh.FrontOfficeApi.Infrastructure;
using Konteh.Infrastructure;
using Konteh.Infrastructure.Behaviors;
using Konteh.Infrastructure.ExceptionHandlers;
using Konteh.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options => options.SchemaSettings.SchemaNameGenerator = new CustomSwaggerSchemaNameGenerator());

builder.Services.AddDbContext<KontehContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("KontehDB")));

builder.Services.AddScoped<IRepository<Question>, QuestionRepository>();
builder.Services.AddScoped<IRepository<Answer>, AnswerRepository>();
builder.Services.AddScoped<IRepository<Exam>, ExamRepository>();
builder.Services.AddScoped<IRepository<Candidate>, CandidateRepository>();
builder.Services.AddScoped<IRepository<ExamQuestion>, ExamQuestionRepository>();

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

app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
