using eventify_backend.Data;
using eventify_backend.Hubs;
using eventify_backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AuthenticationService>(); 
builder.Services.AddSignalR();


// Retrieve connection string from configuration.
var connectionString = builder.Configuration.GetConnectionString("EventifyDbConnectionString");

// Configure Entity Framework Core to use MySQL.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// CORS configuration
// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        builder =>
        {
            // Allow requests from http://localhost:4200
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Allow credentials

        });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files

app.UseRouting();

// Use CORS
app.UseCors("AllowLocalhost4200");

app.UseAuthorization();

// Configure controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

// Run the application
app.Run();
