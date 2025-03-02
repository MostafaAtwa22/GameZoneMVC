using GameZone.Core.Models;
using GameZone.Repository.Interceptors;
using GameZone.Services.Helper;
using GameZone.Services.Services.CategoriesServices;
using GameZone.Services.Services.DevicesServices;
using GameZone.Services.Services.EmailServices;
using GameZone.Services.Services.GamesServices;
using GameZone.Services.Services.ManageServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using NToastNotify;

namespace GameZone.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Google Authentication
            builder.Services
            .AddAuthentication()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, option =>
            {
                option.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                option.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var constr = builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("No Connection String");

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddNToastNotifyToastr(new ToastrOptions()
                {
                    ProgressBar = true,
                    PositionClass = ToastPositions.TopRight,
                    PreventDuplicates = true,
                    CloseButton = true
                });

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(constr)
                .AddInterceptors(new SoftDeleteInterceptor());
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(30);  
                options.SlidingExpiration = true;              
                options.Cookie.HttpOnly = true;
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>
            (
                    option =>
                    {
                        option.Password.RequiredLength = 8;
                        option.Password.RequireUppercase = true;
                        option.Password.RequireLowercase = true;
                        option.Password.RequireNonAlphanumeric = true;
                    }
            ).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddTransient<IEmailSettings, EmailSettings>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IGamesServices, GamesServices>();
            builder.Services.AddScoped<IManageServices, ManageServices>();
            builder.Services.AddScoped<IDevicesServices, DevicesServices>();
            builder.Services.AddScoped<ICategoriesServices, CategoriesServices>();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
