using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add authentication services and configure cookie authentication for customers
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/AcquistaEventi/Login"; // Customer login path
    options.LogoutPath = "/AcquistaEventi/Logout"; // Customer logout path
    options.AccessDeniedPath = "/Home/AccessDenied"; // Path to access denied view
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set the cookie expiration time for customers
})
// Add cookie authentication for administrators
.AddCookie("AdminCookieAuthentication", options =>
{
    options.LoginPath = "/Amministratore/Login"; // Administrator login path
    options.LogoutPath = "/Amministratore/Logout"; // Administrator logout path
    options.Cookie.Name = "RiccioneDiscoAdminAuth"; // A distinct cookie name to avoid clashes
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // You can adjust the expiration time as needed for admins
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Make sure to call UseAuthentication and UseAuthorization in the correct order
app.UseAuthentication(); // This activates the authentication middleware
app.UseAuthorization(); // This activates the authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
