using FindingPets;
using FindingPets.Business.Services.AuthenUserServices;
using FindingPets.Business.Services.EmailServices;
using FindingPets.Business.Services.ImageServices;
using FindingPets.Business.Services.PostServices;
using FindingPets.Data.Entities;
using FindingPets.Data.PostgreEntities;
using FindingPets.Data.Repositories.ImplementedRepositories.AuthenUserRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostImagesRepositories;
using FindingPets.Data.Repositories.ImplementedRepositories.PostRepositories;
using FindingPets.Data.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FindingPets.API",
        Description = "APIs for FindingPets System"
    });

    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                        "\n\nEnter 'Bearer' [space] and then your token in the text input below. " +
                          "\n\nExample: 'Bearer 12345abcde'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference()
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        securityScheme,
                        new string[]{ }
                    }
                });

});
// Add DBContext
//builder.Services.AddDbContext<FindingPetsDbContext>();
//builder.Services.AddDbContext<FindingPetsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddDbContext<D8hclhg7mplh6sContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Add Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthenUserService, AuthenUserService>();
// Add Repos
builder.Services.AddTransient<IPostRepo, PostRepo>();
builder.Services.AddTransient<IPostImagesRepo, PostImageRepo>();
builder.Services.AddTransient<IAuthenUserRepo, AuthenUserRepo>();

// Add authen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    //opt.Authority = builder.Configuration["Jwt:Firebase:ValidIssuer"];
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "api://findingpets",
        ValidAudience = "api://findingpets",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("findingpets16062000"))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        b => b.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .WithExposedHeaders(new string[] { "Authorization", "authorization" }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("AllowOrigin");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/minimal-api/test", async (IUnitOfWork context) =>
{
    return await context.PostRepo.GetPosts();
});

app.Run();
