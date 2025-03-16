using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load configuration
var config = builder.Configuration;

// ✅ Add services to the container
builder.Services.AddControllers(); // API Controllers
builder.Services.AddEndpointsApiExplorer();

// ✅ Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "API for managing tasks and authentication."
    });

    // 🔹 Enable JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your_token}' to authenticate."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ✅ Database Context (Entity Framework Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// ✅ Authentication Service
builder.Services.AddScoped<AuthService>();

// ✅ CORS Policy (Allows frontend to connect)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000") // 🔹 Update with your frontend URL
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ✅ JWT Authentication
var jwtKey = config["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is missing in configuration. Check your appsettings.json file.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// ✅ Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// ✅ Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 🔹 Improved error handling for development
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // 🔹 HSTS only in production
}

app.UseHttpsRedirection();

// ✅ Enable CORS
app.UseCors("AllowFrontend");

// ✅ Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers for API routes
app.MapControllers();

// ✅ Run the application
app.Run();
