using Microsoft.EntityFrameworkCore;
using MyApi.Data.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Scan(scan => scan.FromAssemblyOf<Program>()
                                  .AddClasses(classes => classes.InNamespaces("MyApi.Services"))
                                  .AsMatchingInterface()
                                  .WithScopedLifetime());

builder.Services.AddCors(options =>
{
    options.AddPolicy("public_policy", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin();
    });

    options.AddPolicy("private_policy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000"


        )
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("public_policy");
app.UseAuthorization();

app.MapControllers();

app.Run();
