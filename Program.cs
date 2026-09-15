var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<mvc.Repositories.IRepositorioPropietario, mvc.Repositories.RepositorioPropietario>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInquilino, mvc.Repositories.RepositorioInquilino>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInmueble, mvc.Repositories.RepositorioInmueble>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioTipoInmueble, mvc.Repositories.RepositorioTipoInmueble>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioReserva, mvc.Repositories.RepositorioReserva>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioUsuario, mvc.Repositories.RepositorioUsuario>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioPago, mvc.Repositories.RepositorioPago>();

var app = builder.Build();
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();