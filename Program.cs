var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<mvc.Repositories.IRepositorioPropietario, mvc.Repositories.RepositorioPropietario>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInquilino, mvc.Repositories.RepositorioInquilino>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioInmueble, mvc.Repositories.RepositorioInmueble>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioTipoInmueble, mvc.Repositories.RepositorioTipoInmueble>();
builder.Services.AddScoped<mvc.Repositories.IRepositorioReserva, mvc.Repositories.RepositorioReserva>();
builder.Services.AddScoped<mvc.Repositorios.IRepositorioUsuario, mvc.Repositorios.RepositorioUsuario>();var app = builder.Build();
app.UseSession();

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
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
