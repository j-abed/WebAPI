﻿using Keyvault_WebAPI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Quotes.Contracts;
using Quotes.Implementations.Mocks;

namespace Keyvault_WebAPI
{
    public class Startup
    {
        private const string ApiTitle = "Azure Keyvault Example WebApi";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register Contracts and Implementations
            // Register automapper classes
            RegisterOwnServices(ref services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    TermsOfService = new Uri("https://creativecommons.org/share-your-work/public-domain/freeworks"),
                    Title = ApiTitle,
                    Version = Configuration["currentVersion"],
                    Contact = new OpenApiContact
                    {
                        Email = "boletus151@gmail.com",
                        Name = "boletus151"
                    }
                });
            });

            services.AddVersionedApiExplorer(op =>
            {
                op.GroupNameFormat = "VV";
            });

            services.AddApiVersioning(op =>
            {
                op.DefaultApiVersion = new ApiVersion(1, 0);
                op.ApiVersionReader = new HeaderApiVersionReader() { HeaderNames = { "api-version" } };
                op.AssumeDefaultVersionWhenUnspecified = true;
                op.ReportApiVersions = true;
            });

            // add wide-open CORS.
            // this should be changed before deploying to production
            // to ensure that only your accepted origins can call the API.

            services.AddCors(o => o.AddPolicy("default", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // customize swagger ui entry point
                //c.RoutePrefix = "swagger/ui/index.html";

                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiTitle);
            });
            //}
            //else
            //{
            //    app.UseHsts();
            //}

            app.UseCors("default");
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //.RequireAuthorization(); // adds auth to ALL endpoints of the API
            });
        }

        private static void RegisterOwnServices(ref IServiceCollection services)
        {
            services.AddScoped<IMyAppSettings, MyAppSettings>();
            services.AddScoped<IQuotesRepository, QuotesRepositoryMockData>();

            //services.AddMemoryCache();
            //services.AddHttpContextAccessor();
        }
    }
}