var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<mvc.Repositories.IRepositorioPropietario, mvc.Repositories.RepositorioPropietario>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInquilino, mvc.Repositories.RepositorioInquilino>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInmueble, mvc.Repositories.RepositorioInmueble>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
