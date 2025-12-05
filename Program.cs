using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SecureFileUploadDemo.Data;
using SecureFileUploadDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MVC
builder.Services.AddControllersWithViews();

// 3. Configure large file upload (e.g. 500MB)
long maxBytes = (long)(builder.Configuration.GetSection("FileUpload")["MaxSizeMB"] is string maxStr
    && long.TryParse(maxStr, out var mb)
        ? mb
        : 500) * 1024L * 1024L;

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxBytes; // For IIS / Kestrel
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxBytes;
});

// 4. Register our services
builder.Services.AddScoped<IAntivirusScanner, DummyAntivirusScanner>();
builder.Services.AddScoped<ICloudStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IAntivirusScanner, VirusTotalScanner>();


var app = builder.Build();

// Migrate DB on startup (dev-friendly)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Files}/{action=Index}/{id?}");

app.Run();
