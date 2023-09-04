using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using CVSharer.Services;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.EntityFramework;
using DataAccessLayer.Contexts;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight;});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "CVSharerCookie";
        option.LoginPath = "/Session/Login";
        option.AccessDeniedPath = "/Session/Login";
        //option.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

//PDF Dependency Injection
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<PdfGenerator>();

//Certificate Dependency Injection
builder.Services.AddScoped<ICertificateService, CertificateManager>();
builder.Services.AddScoped<ICertificateDal,EfCertificateDal>();

//Education Dependency Injection
builder.Services.AddScoped<IEducationService, EducationManager>();
builder.Services.AddScoped<IEducationDal, EfEducationDal>();

//Experience Dependency Injection
builder.Services.AddScoped<IExperienceService, ExperienceManager>();
builder.Services.AddScoped<IExperienceDal, EfExperienceDal>();

//Hobby Dependency Injection
builder.Services.AddScoped<IHobbyService, HobbyManager>();
builder.Services.AddScoped<IHobbyDal, EfHobbyDal>();

//Language Dependency Injection
builder.Services.AddScoped<ILanguageService, LanguageManager>();
builder.Services.AddScoped<ILanguageDal, EfLanguageDal>();

//Link Dependency Injection
builder.Services.AddScoped<ILinkService, LinkManager>();
builder.Services.AddScoped<ILinkDal, EfLinkDal>();

//Skill Dependency Injection
builder.Services.AddScoped<ISkillService, SkillManager>();
builder.Services.AddScoped<ISkillDal, EfSkillDal>();

//User Dependency Injection
builder.Services.AddScoped<IUserService,UserManager >();
builder.Services.AddScoped<IUserDal, EfUserDal>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseNotyf(); //toast 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
