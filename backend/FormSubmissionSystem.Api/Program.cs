using Microsoft.EntityFrameworkCore;
using FormSubmissionSystem.Application.UseCases;
using FormSubmissionSystem.Domain.Repositories;
using FormSubmissionSystem.Infrastructure.Data;
using FormSubmissionSystem.Infrastructure.Repositories;
using FormSubmissionSystem.Api.Middleware;
using FormSubmissionSystem.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FormSubmissionSystem"));

builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<GetSubmissionsUseCase>();
builder.Services.AddScoped<GetSubmissionByIdUseCase>();
builder.Services.AddScoped<CreateSubmissionUseCase>();

builder.Services.AddCorsConfiguration(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(CorsConfiguration.PolicyName);
app.MapControllers();

app.Run();
