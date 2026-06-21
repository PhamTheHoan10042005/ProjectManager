using CommentNotifyService.Consumers;
using CommentNotifyService.Data;
using CommentNotifyService.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? builder.Configuration["Jwt:Secret"]
    ?? "ProjectManagementSystem-JWT-Secret-Key-2024-Min32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectManagementSystem";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectManagementSystem";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

/*builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TaskStatusChangedConsumer>();
    x.AddConsumer<TaskAssignedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ConfigureEndpoints(context);
    });
});*/

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CommentNotifyService", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập chuỗi Token của bạn theo cấu trúc: Bearer [chuỗi_token]",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("--- Đã tự động cập nhật Database thành công! ---");

        var admin = context.Users.FirstOrDefault(u => u.Username == "admin@gmail.com");
        if (admin == null)
        {
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123@"),
                FullName = "Hệ Thống Quản Trị",
                Role = "Admin"
            });
        }
        else
        {
            admin.Role = "Admin";
            admin.FullName = "Hệ Thống Quản Trị";
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123@");
        }

        context.SaveChanges();
        Console.WriteLine("--- Tài khoản Admin cố định: admin@gmail.com / Admin123@ ---");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi tự động cập nhật DB hoặc Seed dữ liệu: {ex.Message}");
    }
}

app.Run();
