using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataContext;
using DataAccess.Repositories;
using Presentation.ActionFilter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Domain.Models;
using Presentation.Controllers;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace MiguelCassarEPSolution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<PollDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<PollDbContext>();

            builder.Services.AddScoped<IPollRepository, PollRepository>();
            builder.Services.AddScoped<IPollRepository, PollFileRepository>();
            builder.Services.AddScoped<VoteLogRepository>();
            builder.Services.AddScoped<OneVotePerUserFilter>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Logging.AddConsole();

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(5000); // Always run on 5000
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                // Trust headers from Nginx (so redirect loop doesn't happen)
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                    KnownProxies = { IPAddress.Parse("locahost") }
                });

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection(); // Enable HTTPS in prod only
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}