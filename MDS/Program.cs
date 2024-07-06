using MDS.Hubs;
using MDS.Model.Entity;
using MDS.Services;
using MDS.Services.Implement;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Core.Helper;
using MDS.Shared.Database.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//var setting = new ConnectionSettings(new Uri("http://localhost:9200/")).DefaultIndex("mds_demo");

//var client = new ElasticClient(setting);

//builder.Services.AddSingleton(client);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your token with this format: ''Bearer YOUR_TOKEN''",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});




// Configure Postgres Server
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

// Configure Email
var emailConfig = builder.Configuration.GetSection("Email").Get<EmailConfig>();

builder.Services.AddSingleton(emailConfig);


// Google
builder.Services.AddAuthentication().AddGoogle(opts =>
{
    opts.ClientId = builder.Configuration["Google:ClientId"];
    opts.ClientSecret = builder.Configuration["Google:ClientSecret"];
});


// Facebook
builder.Services.AddAuthentication().AddFacebook(opts =>
{
    opts.ClientId = builder.Configuration["Facebook:AppId"];
    opts.ClientSecret = builder.Configuration["Facebook:AppSecret"];
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", build =>
    {
        build.WithOrigins("https://localhost:8080").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        build.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// Exception
builder.Services.AddExceptionHandler<AppExceptionHandler>();


builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IFeedBackService, FeedBackService>();



builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSignalR();

// Fix lỗi DateTime PostgreSql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("MyCors");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat-hub");

app.MapIdentityApi<ApplicationUser>();

app.Run();
