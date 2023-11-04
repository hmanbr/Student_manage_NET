using System.Text.Json.Serialization;
using NGitLab;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<SWPContext>(options => new SWPContext());

builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IHashService, HashService>();
//builder.Services.AddSingleton<IGitLabService, GitLabService>();
builder.Services.AddSingleton<IGitLabClient>(new GitLabClient("https://gitlab.com", "glpat-KyJ2VzkHoppTR7e5_J2E"));
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SessionCookie";
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IFileUploadService, LocalFileUploadService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
