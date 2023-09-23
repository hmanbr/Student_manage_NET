using G3;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SWPContext>(options => new SWPContext());
builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IHashService, HashService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthorization();
app.MapControllers();

app.Run();
