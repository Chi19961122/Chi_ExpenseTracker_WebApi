using System.Reflection;
using System.Text;
using Chi.ExpenseTracker.Api.Mapping;
using Chi.ExpenseTracker.Api.Middleware;
using Chi.ExpenseTracker.Api.Validators;
using Chi.ExpenseTracker.Common.Options;
using Chi.ExpenseTracker.Repositories.DependencyInjection;
using Chi.ExpenseTracker.Services.DependencyInjection;
using Chi.ExpenseTracker.Services.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "frontend";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Chi.ExpenseTracker API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Swagger UI 支援輸入 Bearer Token 測試需授權的端點。
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "輸入登入取得的 JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

// 允許前端跨來源存取。來源可由設定 Cors:AllowedOrigins 覆寫。
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:8080"];
builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy => policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()));

// AutoMapper：掃描 Api 與 Services 兩個 assembly 的 Profile。
builder.Services.AddAutoMapper(
    _ => { },
    typeof(ApiMappingProfile).Assembly,
    typeof(ServiceMappingProfile).Assembly);

// FluentValidation：註冊 Api assembly 內所有 Validator。
builder.Services.AddValidatorsFromAssemblyContaining<LoginParameterValidator>();

// JWT 認證。
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 保留 JWT 原始 claim 名稱（sub/email/name），不做 inbound 對應。
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        };
    });
builder.Services.AddAuthorization();

// 應用層與資料層。
builder.Services.AddApplicationServices();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? string.Empty;
builder.Services.AddDatabaseInfrastructure(connectionString);

var app = builder.Build();

// 正式環境仍使用開發用簽章金鑰時直接拒絕啟動（fail-fast）：
// 金鑰須以環境變數 Jwt__Key（至少 32 bytes 隨機字串）覆寫，避免任何人都能偽造 JWT。
if (app.Environment.IsProduction() && jwtOptions.Key.StartsWith("dev-only", StringComparison.Ordinal))
{
    throw new InvalidOperationException(
        "Jwt:Key 仍為開發用預設金鑰。正式環境請以環境變數 Jwt__Key 設定至少 32 bytes 的隨機字串後再啟動。");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// 供整合測試（WebApplicationFactory）參考的進入點型別。
/// </summary>
public partial class Program;
