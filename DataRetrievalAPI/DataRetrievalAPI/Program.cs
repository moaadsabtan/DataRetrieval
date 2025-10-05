using DataRetrievalAPI.Persistence;
using DataRetrievalAPI.Persistence.Entities;
using DataRetrievalAPI.Repositories;
using DataRetrievalAPI.Services;
using DataRetrievalAPI.Storage;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var redisEnabled = builder.Configuration.GetValue<bool>("Redis:Enabled");
if (redisEnabled)
{
    builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration["Redis:Configuration"]; });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}
builder.Services.AddScoped<IRepository<DataItem>, DataRepository>();


builder.Services.AddScoped<DataRepository>();
builder.Services.AddSingleton<FileStorage>();
builder.Services.AddScoped<DbStorage>();
builder.Services.AddScoped<StorageFactory>();
builder.Services.AddScoped<IDataService, DataService>();

// AutoMapper & FluentValidation
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddFluentValidationAutoValidation();

// HttpClient with Polly sample policy (used internally if needed)
builder.Services.AddHttpClient("retryClient").AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)));



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
