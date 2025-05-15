using EduPulse.Models.Service_Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EduPulse.Models;
using System.Text;
using EduPulse.Models.Services;
using EduPulse.Models.Exam_Sub;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Serilog;
using Microsoft.OpenApi.Models;
using EduPulse.Services;


public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
        builder.Host.UseSerilog();


        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // Configure DbContext
        builder.Services.AddDbContext<SW_Entity>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        // Configure Firebase
        var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];
        if (string.IsNullOrEmpty(firebaseCredentialsPath))
        {
            throw new InvalidOperationException("Firebase credentials path is not configured in appsettings.json");
        }
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), firebaseCredentialsPath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Firebase credentials file not found at: {fullPath}");
        }

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(fullPath)
        });





        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllRoutes", builder =>
            {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
                       
            });
        });

       



        // email service
        builder.Services.AddScoped<IEmailService>(provider => {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<EmailService>>();
            return new EmailService(configuration, logger);
        });
        
        // Add email service factory (if needed elsewhere in the application)
        builder.Services.AddScoped<IEmailServiceFactory, EmailServiceFactory>();

        builder.Services.AddScoped<NotificationService>();

        builder.Services.AddSingleton<IExamSystem>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var googleFormUrl = config["ExamSystem:GoogleFormUrl"];


            if (string.IsNullOrWhiteSpace(googleFormUrl))
                throw new InvalidOperationException("GoogleFormUrl configuration is missing.");

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
        builder.Services.AddScoped<ExamService>();
        builder.Services.AddScoped<EduPulse.Data.ManualMigration>();
        #endregion


        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.Services.AddScoped<AuthService>();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduPulse API", Version = "v1" });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            // Add JWT Authentication support in Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

       


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
                c.RoutePrefix = string.Empty; 
            });

        }


        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Add port configuration
        app.Urls.Add("http://localhost:5000");
        
        app.Run();


    }
}
