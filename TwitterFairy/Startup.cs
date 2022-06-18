using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Reflection;
using TwitterFairy.Services.Twitter;

namespace NXA.SC.Caas
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<ITwitterService, TwitterService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Twitter Fairy", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString("/api");
                return next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Twitter Fairy v1"));
            app.UsePathBase("/api");
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                var lf = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = lf?.CreateLogger("exceptionHandlerLogger");
                logger?.LogDebug(exception?.StackTrace);
                await context.Response.WriteAsJsonAsync(exception?.Message);
            }));
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseFileServer();
            //ApplicationLogging.LoggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
        }
    }
}
