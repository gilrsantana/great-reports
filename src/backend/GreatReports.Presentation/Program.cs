using GreatReports.Presentation.Extensions;
using GreatReports.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.UsePresentationPipeline();

app.Run();

public partial class Program { }
