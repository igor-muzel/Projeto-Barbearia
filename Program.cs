using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// LÊ A STRING DE CONEXÃO E INJETA A CLASSE DATA NO SISTEMA
var connectionString = builder.Configuration.GetConnectionString("ConexaoMySQL");
builder.Services.AddScoped<ProjectBarber.Data.UsuarioData>(provider => new ProjectBarber.Data.UsuarioData(connectionString));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Se o cara tentar acessar algo bloqueado, manda pra cá
        options.AccessDeniedPath = "/Auth/Login"; // Caminho caso ele não tenha acesso
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Quem é você?
app.UseAuthorization();  // Você tem permissão?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
