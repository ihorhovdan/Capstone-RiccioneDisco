using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using RiccioneDisco.Models; // Aggiungi l'importazione dello spazio dei nomi dove si trova EmailService

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore.
builder.Services.AddControllersWithViews();

// Registra EmailService come singleton
builder.Services.AddSingleton<EmailService>();

// Aggiungi i servizi di autenticazione e configura l'autenticazione dei cookie per i clienti
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/AcquistaEventi/Login"; // Percorso di login per i clienti
    options.LogoutPath = "/AcquistaEventi/Logout"; // Percorso di logout per i clienti
    options.AccessDeniedPath = "/Home/AccessDenied"; // Percorso per la vista di accesso negato
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Imposta il tempo di scadenza del cookie per i clienti
})
// Aggiungi autenticazione dei cookie per gli amministratori
.AddCookie("AdminCookieAuthentication", options =>
{
    options.LoginPath = "/Amministratore/Login"; // Percorso di login per gli amministratori
    options.LogoutPath = "/Amministratore/Logout"; // Percorso di logout per gli amministratori
    options.Cookie.Name = "RiccioneDiscoAdminAuth"; // Nome del cookie distinto per evitare conflitti
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Puoi regolare il tempo di scadenza come necessario per gli amministratori
});

var app = builder.Build();

// Configura la pipeline di richiesta HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Assicurati di chiamare UseAuthentication e UseAuthorization nel giusto ordine
app.UseAuthentication(); // Attiva il middleware di autenticazione
app.UseAuthorization(); // Attiva il middleware di autorizzazione

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
