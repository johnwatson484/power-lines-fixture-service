using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Messaging;
using Microsoft.EntityFrameworkCore;
using PowerLinesFixtureService.Analysis;
using PowerLinesFixtureService.Fixtures;
using System;

namespace PowerLinesFixtureService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PowerLinesFixtureService"), options =>
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null))
                );

            var messageConfig = Configuration.GetSection("Message").Get<MessageConfig>();
            services.AddSingleton(messageConfig);

            var analysisUrl = Configuration.GetSection("AnalysisUrl").Get<AnalysisUrl>();
            services.AddSingleton(analysisUrl);

            services.AddSingleton<IAnalysisApi, AnalysisApi>();
            services.AddScoped<IFixtureService, FixtureService>();        
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ApplyMigrations(dbContext);
        }

        public void ApplyMigrations(ApplicationDbContext dbContext)
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
