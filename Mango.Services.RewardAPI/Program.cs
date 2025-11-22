
using Mango.MessageBus;
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Extensions;
using Mango.Services.RewardAPI.Messaging;
using Mango.Services.RewardAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add a singelton implementation for the AppDbContext in order to use it in ASBC

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();

optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddSingleton(new RewardService(optionBuilder.Options));


builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddControllers();

builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigrations();
app.UseAzureServiceBusConsumer();
app.Run();


void ApplyMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}
