using GameZone.Repository.Interceptors;
using GameZone.Services.Helper;
using GameZone.Services.Services.CategoriesServices;
using GameZone.Services.Services.DevicesServices;
using GameZone.Services.Services.GamesServices;

namespace GameZone.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var constr = builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("No Connection String");

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(constr)
                .AddInterceptors(new SoftDeleteInterceptor());
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IGamesServices, GamesServices>();
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
