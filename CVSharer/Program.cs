using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.EntityFramework;
using DataAccessLayer.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
