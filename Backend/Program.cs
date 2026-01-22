using _1001;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;

namespace TrackListApp{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable legacy timestamp behavior to handle DateTime kinds automatically
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    // Configure view discovery to look in Frontend/Views
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("../Frontend/Views/{1}/{0}.cshtml");
                    options.ViewLocationFormats.Add("../Frontend/Views/Shared/{0}.cshtml");
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            var app = builder.Build();
            
            // Configure static file serving from Frontend/wwwroot
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Frontend", "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(wwwrootPath),
                RequestPath = ""
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use HTTPS redirect for production (App Services expects this)
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.MapControllers();

            app.Run();
        }
    }
}
        