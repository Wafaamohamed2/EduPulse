using Microsoft.AspNetCore.Authentication.JwtBearer;
using EduPulse.Models;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceAPI.Services;
using SW_Project.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EduPulse.Models.Service_Registration;
using Microsoft.OpenApi.Models;

namespace EduPulse
{
    public class Program
    {
        public static void Main(string[] args)
        {



        var builder = WebApplication.CreateBuilder(args);

          

          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // تعريف الـ API
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EduPulse API",
                    Version = "v1"
                });

                // إرغام خادم واحد فقط (HTTP://localhost:5249)
                c.AddServer(new OpenApiServer
                {
                    Url = "http://localhost:5249"
                });
            });


            builder.Services.AddDbContext<SW_Entity>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // السماح بجميع المصادر (لتطوير فقط - غير مناسب للإنتاج)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllRoutes", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });

            builder.Services.AddControllers();



            // email service
            builder.Services.AddTransient<EmailService>();



            // Adding authentication
            #region
            // اقرأ الإعدادات أولاً وتحقق منها
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];
            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtIssuer) ||
                string.IsNullOrWhiteSpace(jwtAudience) ||
                string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing in appsettings.json. " +
                    "Ensure Jwt:Issuer, Jwt:Audience, and Jwt:Key are all set.");
            }

            // ثم استعمل القيم بعد التأكد منها
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = signingKey
                    };
                });

            #endregion



            // Registering the services
            #region
            builder.Services.AddScoped<StudentRegistrationService>();
            builder.Services.AddScoped<TeacherRegistrationService>();
            builder.Services.AddScoped<ParentRegistrationService>();
            #endregion



            builder.Services.AddScoped<AuthService>();

            var app = builder.Build();



            app.UseRouting();
            app.UseCors("AllRoutes");




            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // مسار نسبي لملف JSON
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduPulse API v1");
                c.RoutePrefix = "swagger";
            });


            //   app.UseHttpsRedirection();


            //app.Use((context, next) =>
            //{
            //    context.Request.Scheme = "https";
            //    return next();
            //});


            app.UseAuthentication();
            app.UseAuthorization();
          


            app.MapControllers();
      

            app.Run();
        }
    }
}
