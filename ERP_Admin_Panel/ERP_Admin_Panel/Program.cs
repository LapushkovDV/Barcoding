using ERP_Admin_Panel.Services.Account;
using ERP_Admin_Panel.Services.Configuration;
using ERP_Admin_Panel.Services.Database;
using ERP_Admin_Panel.Services.Interfaces;
using ERP_Admin_Panel.Services.Log;
using ERP_Admin_Panel.Services.Modal;
using ERP_Admin_Panel.Services.NavBarActions;
using ERP_Admin_Panel.Services.StateProviders;
using ERP_Admin_Panel.Services.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using ConfigurationManager = ERP_Admin_Panel.Services.Configuration.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DbConnectionString");
var dataBaseProvider = builder.Configuration.GetConnectionString("DataBase");
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


if (connectionString == string.Empty)
{
    Console.WriteLine("«апуск сервера невозможен, так как не настроена строка подключени€ дл€ базы");
    Console.ReadKey();
    Process.GetCurrentProcess().Kill();
}

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    if (connectionString == null || connectionString == string.Empty) return;

    if (dataBaseProvider.ToUpper() == "POSTGRESQL")
    {
        try
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            options.UseNpgsql(connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    else
    {
        try
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
            options.UseSqlServer(connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
});

builder.Services.AddOptions();
builder.Services.AddProtectedBrowserStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthStateProvider>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<IModalService, ModalService>();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<INavBarOperation, NavBarOperation>();
builder.Services.AddScoped<IConfigurationManager, ConfigurationManager>();
builder.Services.AddScoped<ILogManager, LogManager>();
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//Localization
var defaultCulture = new CultureInfo("en-US");

CultureInfo.CurrentCulture = defaultCulture;
CultureInfo.CurrentUICulture = defaultCulture;
//-------------------------------
//--------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
