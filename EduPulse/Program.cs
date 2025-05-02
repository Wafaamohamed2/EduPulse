using EduPulse.Models.Service_Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SW_Project.Models;
using System.Text;
using EduPulse.Services;
using EduPulse.Models.Exam_Sub;
using EduPulse.Models.Services;
using SW_Project.Migrations;

public class Program
{
    public static void Main(string[] args)
    {



        var builder = WebApplication.CreateBuilder(args);




        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<SW_Entity>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

       
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
        builder.Services.AddSingleton<IEmailService, EmailService>();

        builder.Services.AddSingleton<NotificationService>();

        builder.Services.AddSingleton<IExamSystem>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var googleFormUrl = config["ExamSystem:GoogleFormUrl"];
            return new GoogleFormsExamAdapter(googleFormUrl);
        });




        // Adding authentication
        #region
        var jwtIssuer = builder.Configuration["Jwt:Issuer"]
      ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");
        var jwtAudience = builder.Configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing in configuration.");
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduPulse API v1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });

        }

       


        app.UseAuthentication();
        app.UseAuthorization();



        app.MapControllers();

        
        app.Urls.Add("http://127.0.0.1:5000");
        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.Run();


    }
}
