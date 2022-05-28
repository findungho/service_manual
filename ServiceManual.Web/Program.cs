using ServiceManual.ApplicationCore.Entities;
using ServiceManual.ApplicationCore.Interfaces;
using ServiceManual.ApplicationCore.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FactoryDeviceContext>
    (opt => opt.UseInMemoryDatabase("ServiceManual"));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFactoryDeviceService, FactoryDeviceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

//app.UseSwagger();

app.MapControllers();
//app.MapGet("/", () => "Hello World!");

app.UseSwaggerUI();

app.Run();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IFactoryDeviceService, FactoryDeviceService>();
//builder.Services.AddDbContext<FactoryDeviceContext>
//    (opt => opt.UseInMemoryDatabase("ServiceManual"));
//builder.Services.AddDbContext<IssueDbContext>(
//    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

//var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.EnsureDatabaseCreated();
//app.Run();