using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JonkerBudgetCore.Api.Persistence;
using Serilog;
using JonkerBudgetCore.Api.Api.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using JonkerBudgetCore.Api.Api.Filters;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JonkerBudgetCore.Api.Auth.Jwt;
using System;
using JonkerBudgetCore.Api.Auth.Encrypt;
using JonkerBudgetCore.Api.Api.Providers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using JonkerBudgetCore.Api.Domain.Policy;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Policy.Users;
using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Repositories.Users;
using JonkerBudgetCore.Api.Domain.Policies.Users;
using JonkerBudgetCore.Api.Auth.ActiveDirectory;
using Microsoft.AspNetCore.Diagnostics;
using JonkerBudgetCore.Api.Api.Exceptions;
using JonkerBudgetCore.Api.Domain.Policies;
using JonkerBudgetCore.Api.Domain.Services.Roles;
using JonkerBudgetCore.Api.Domain.Services.Users;
using JonkerBudgetCore.Api.Domain.Services.Dashboards;
using JonkerBudgetCore.Api.Domain.Services.Widgets;
using JonkerBudgetCore.Api.Domain.Models.Password;
using JonkerBudgetCore.Api.Domain.Shared_Services;
using JonkerBudgetCore.Api.Domain.Services.Categories;

namespace JonkerBudgetCore.Api.Api
{
    public class Startup
    {
        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            // Setup DB            
            services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(Configuration.GetConnectionString("Database"),
                   retry => retry.EnableRetryOnFailure()));

            //Shared Services
            services.AddTransient<ICommsService, CommsService>();

            //Domain services
            services.AddTransient<IUsersService, UsersService>();            
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IDashboardsService, DashboardsService>();
            services.AddTransient<IWidgetsService, WidgetsService>();

            services.AddTransient<ICategoryService, CategoryService>();


            //Repositories
            services.AddTransient<IUsersRepository, UsersRepository>();

            //Policies            
            services.AddTransient<IPolicy<RegisterUserModel>, RegisterUserPolicy>();
            services.AddTransient<IPolicy<RegisterDomainUserModel>, RegisterDomainUserPolicy>();            
            services.AddTransient<IPolicy<UsernameModel>, UsernamePolicy>();
            services.AddTransient<IPolicy<PasswordModel>, PasswordPolicy>();
            services.AddTransient<IPolicy<UpdateUserModel>, UpdateUserPolicy>();
            services.AddTransient<IPolicy<PasswordResetRequestModel>, RequestPasswordResetPolicy>();
            services.AddTransient<IPolicy<ResetPasswordModel>, ResetPasswordPolicy>();

            //Security and user providers
            services.AddTransient<IUserInfoProvider, UserInfoProvider>();
            services.AddTransient<IJwtIssuer, JwtIssuer>();
            services.AddTransient<IEncrypter, Encrypter>();
            services.AddTransient<IUserClaimsProvider, UserClaimsProvider>();
            services.AddTransient<IActiveDirectoryProvider, ActiveDirectoryProvider>();

            //HttpContext provider for use outside of a controller
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });

            services.AddAutoMapper();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new Info { Title = "JonkerBudgetCore.Api Api", Version = "v2", Description = "JonkerBudgetCore.Api Api Documentation" });
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            
            // Configure using a sub-section of the appsettings.json file.
            services.Configure<ActiveDirectoryOptions>(Configuration.GetSection("ActiveDirectoryOptions"));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            ApplicationDbContext dbContext,
            IApplicationLifetime appLifetime)
        {
            // Use Serilog to log instead of built in logging
            loggerFactory.AddSerilog();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.RouteTemplate = @"{documentName}/api-docs";
            });

            //Enable middleware to serve swagger - ui(HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/v2/api-docs", "JonkerBudgetCore.Api Api V1");
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure Jwt Token Validation
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            // Add custom properties to the serilog logger - after jwt auth but before mvc
            app.UseCustomSerilogProperties();

            app.UseCors("CorsPolicy");

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var ex = error.Error;

                        if (error.Error is PolicyViolationException)
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync(new PolicyErrorModel()
                            {
                                Code = 400,
                                Violations = ((PolicyViolationException)error.Error).Violations 
                                // ex.Message or your custom message other custom data
                            }
                            .ToString(), Encoding.UTF8);
                        }
                        else
                        {
                            await context.Response.WriteAsync(new ErrorModel()
                            {
                                Code = 500,
                                Message = ex.Message 
                                // or your custom message other custom data
                            }.ToString(), Encoding.UTF8);
                        }
                    }
                });
            });

            app.UseMvc();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Setup data on every service start - not same as migration
            if (env.IsDevelopment())
            {
                DbInitializer.Initialize(dbContext);
            }

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
