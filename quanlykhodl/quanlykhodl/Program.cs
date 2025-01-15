using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using quanlykhodl.Clouds;
using quanlykhodl.Models;
using quanlykhodl.QuartzService;
using Quartz;
using System.Text;

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
    options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
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
    q.AddTrigger(otps => otps.ForJob(jobKey).WithIdentity("WeeklyTrigger") /* Tên của "Trigger", đặt tên Trigger để dễ dàng quản lý, Ở đây tên Trigger là "WeeklyTrigger"
    * Tên này sẽ được sử dụng khi bạn cần truy xuất hoặc quản lý trigger (ví dụ: khởi động, dừng hoặc xóa trigger).
    * Có thể đặt tên Trigger như này: ".WithIdentity("TenSecondTrigger", "Group1")" // Tên của Trigger "TenSecondTrigger" và nhóm của trigger "Group1"
      Khi cần dừng, xóa hoặc kiểm tra trạng thái của một trigger cụ thể, sẽ sử dụng tên (Name) và nhóm (Group).
      * Ví dụ: Xóa một trigger theo tên: "var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                                          await scheduler.UnscheduleJob(new TriggerKey("TenSecondTrigger", "Group1"));"
                                                                            */
    .StartNow()
    /*
     * "WithSimpleSchedule": Khoảng thời gian cố định: Bạn chỉ cần chỉ định khoảng thời gian giữa các lần chạy (ví dụ: 10 giây, 5 phút, v.v).
        Lặp lại liên tục: Công việc sẽ được thực thi với khoảng cách thời gian đều đặn và không cần điều kiện phức tạp.
        
     */
    //.WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));

    /* "WithCronSchedule": Lịch trình linh hoạt hơn: Có thể định nghĩa lịch trình với Cron expression,
       cho phép lên lịch theo giây, phút, giờ, ngày trong tuần, ngày tháng, hoặc năm.
       Cron expression: Là một chuỗi ký tự mô tả một lịch trình cụ thể. Ví dụ:
        "0/10 * * * * ?": Lặp lại mỗi 10 giây.
        "0 0 12 * * ?": Chạy vào lúc 12:00 mỗi ngày.
        "0 0 0 1 * ?": Chạy vào lúc 00:00 ngày đầu tiên của mỗi tháng.
    */
    .WithCronSchedule("0/10 * * * * ?"));
});

// Đăng ký HostedService cho Quartz.NET
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
var connection = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<DBContext>(option =>
{
	option.UseSqlServer(connection); // "ThuongMaiDienTu" đây là tên của project, vì tách riêng model khỏi project sang 1 lớp khác nên phải để câu lệnh này "b => b.MigrationsAssembly("ThuongMaiDienTu")"
});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
