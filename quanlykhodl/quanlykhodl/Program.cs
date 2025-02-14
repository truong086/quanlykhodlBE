using CloudinaryDotNet;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using quanlykhodl.ChatHub;
using quanlykhodl.Clouds;
using quanlykhodl.EmailConfigs;
using quanlykhodl.FunctionAuto;
using quanlykhodl.Models;
using quanlykhodl.QuartzService;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;
using Quartz;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region CORS
var corsBuilder = new CorsPolicyBuilder();
corsBuilder.AllowAnyHeader();
corsBuilder.AllowAnyMethod();
corsBuilder.AllowAnyOrigin();
corsBuilder.WithOrigins("http://localhost:8080"); // Đây là Url bên frontEnd
corsBuilder.AllowCredentials();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", corsBuilder.Build());
});

#endregion

#region JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
#endregion

// Đăng ký Firebase Key
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("/home/tsustedu2025_ADMIN/quanlykhodlBE/quanlykhodl/quanlykhodl/notification-bdf14-firebase-adminsdk-fbsvc-87b7743b04.json")
    //Credential = GoogleCredential.FromFile("C:\\Users\\ASUS\\OneDrive\\Desktop\\VueJs\\SpringBoot\\notification-bdf14-firebase-adminsdk-fbsvc-87b7743b04.json")
});

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoreJwtExample",
        Version = "v1",
        Description = "Hello Anh Em",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        {
            Name = "Thanh Toán Online",
            Url = new Uri("https://localhost:44316/")
        }
    });



    // Phần xác thực người dùng(JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer asddvsvs123'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    //var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var path = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    //c.IncludeXmlComments(path);
});

builder.Services.AddSignalR();
// Đọc cấu hình Cloudinary từ appsettings.json
var cloudinaryAccount = new CloudinaryDotNet.Account(
    builder.Configuration["Cloud:Cloudinary_Name"],
    builder.Configuration["Cloud:Api_Key"],
    builder.Configuration["Cloud:Serec_Key"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);

// Đăng ký Cloudinary làm một dịch vụ Singleton
builder.Services.AddSingleton(cloudinary);

builder.Services.Configure<Cloud>(builder.Configuration.GetSection("Cloud"));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("TuDongMoiTuan");
    q.AddJob<TuDongMoiTuan>(Otp => Otp.WithIdentity(jobKey));
    q.AddTrigger(otps => otps.ForJob(jobKey).WithIdentity("WeeklyTrigger")
    .StartNow()
    .WithCronSchedule("0 0 0/1 * * ?"));
    //.WithCronSchedule("0 0/1 * * * ?")); // "0/1" là chạy mỗi phút, để "1" là chỉ chạy 1 phút lần đầu
});

var connection = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<DBContext>(option =>
{
    option.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 31))); // "ThuongMaiDienTu" đây là tên của project, vì tách riêng model khỏi project sang 1 lớp khác nên phải để câu lệnh này "b => b.MigrationsAssembly("ThuongMaiDienTu")"
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
);

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
});
builder.Services.AddSwaggerGen();
// Đăng ký HostedService cho Quartz.NET
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IShelfService, ShelfService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IImportformService, ImportformService>();
builder.Services.AddScoped<IDeliverynoteService, DeliverynoteService>();
builder.Services.AddScoped<IPrepareToExportService, PrepareToExportService>();
builder.Services.AddScoped<IUserOnlineService, UserOnlineService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IStatisticalService, StatisticalService>();
builder.Services.AddScoped<IUserTokenAppService, UserTokenAppService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<UserTokenAppService>();
builder.Services.AddScoped<onlineUser>();
builder.Services.AddScoped<SendEmais>();
builder.Services.AddSingleton<VerificationTaskWorker>();
builder.Services.AddHostedService(p => p.GetRequiredService<VerificationTaskWorker>());
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<KiemTraBase64>();

builder.WebHost.UseUrls("http://0.0.0.0:5000");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    app.MapHub<NotificationHub>("/notificationHub");
//});
app.MapHub<NotificationHub>("/notificationHub")
    .RequireAuthorization(); // Đảm bảo rằng chỉ các user đã xác thực mới có thể kết nối;

app.Run();