using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureLoginApp1.Data;
using SecureLoginApp1.Models;
using SecureLoginApp1.Models.Events;
using SecureLoginApp1.Services;
using SecureLoginApp1.Services.EventHandlers;
using SecureLoginApp1.Services.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
var smtpHost = builder.Configuration["Smtp:Host"];
if (builder.Environment.IsDevelopment() || string.IsNullOrWhiteSpace(smtpHost))
{
    builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
}

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddScoped<IEventHandler<UserLoggedInEvent>, UserLoggedInActivityHandler>();
builder.Services.AddScoped<IEventHandler<PasswordChangedEvent>, PasswordChangedActivityHandler>();
builder.Services.AddScoped<IEventHandler<ProfileUpdatedEvent>, ProfileUpdatedActivityHandler>();

builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
