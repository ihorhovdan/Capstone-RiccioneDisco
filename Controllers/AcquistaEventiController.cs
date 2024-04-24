using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RiccioneDisco.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

public class AcquistaEventiController : Controller
{
    private readonly string _connectionString = DB.connectionString;

    [Authorize]
    public async Task<IActionResult> Index(string discoteca)
    {
        List<Eventi> eventiList = new List<Eventi>();
        // Conserviamo il filtro attuale nella ViewData per poterlo riutilizzare nella view
        ViewData["SelectedDiscoteca"] = discoteca;

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT IdEvento, Immagine, Data, Titolo, Luogo, Prezzo FROM Eventi" +
                            (!string.IsNullOrEmpty(discoteca) ? " WHERE Luogo = @Luogo" : "");

                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(discoteca))
                    {
                        command.Parameters.AddWithValue("@Luogo", discoteca);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            eventiList.Add(new Eventi
                            {
                                IdEvento = (int)reader["IdEvento"],
                                Immagine = reader["Immagine"].ToString(),
                                Data = reader["Data"].ToString(),
                                Titolo = reader["Titolo"].ToString(),
                                Luogo = reader["Luogo"].ToString(),
                                Prezzo = (decimal)reader["Prezzo"]
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel { Message = ex.Message });
        }

        return View(eventiList);
    }



    [HttpGet]
    public IActionResult Login()
    {
        // Se l'utente è già autenticato, reindirizzalo alla pagina Index del controller AcquistaEventi
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("AcquistaEventi", "Index"); // Specifichi controller e action
        }
        return View();
    }

    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "AcquistaEventi");
        }

        returnUrl ??= Url.Action("Index", "AcquistaEventi");
        // Fornisce un fallback per il returnUrl

        var user = await ValidateUserCredentials(email, password);
        if (user != null)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUtente.ToString()),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            // altri claims necessari
        };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                // Puoi aggiungere altre proprietà di autenticazione qui, come tempi di scadenza, ecc.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Verifica se l'URL di ritorno è sicuro da utilizzare per il reindirizzamento
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                // Reindirizza all'index di AcquistaEventi dopo il login riuscito
                return RedirectToAction("Index", "AcquistaEventi");
            }
        }

        ViewBag.ErrorMessage = "Credenziali non valide.";
        return View();
    }


    private async Task<Utente> ValidateUserCredentials(string email, string password)
    {
        Utente user = null;
        try
        {
            using (var connection = new SqlConnection(DB.connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT IdUtente, Nome, Cognome, Email, Password FROM Utenti WHERE Email = @Email AND Password = @Password", connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);  // Confronta direttamente la password in chiaro
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new Utente
                        {
                            IdUtente = (int)reader["IdUtente"],
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Errore durante la verifica delle credenziali: " + ex.Message);
        }
        return user;
    }



    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string nome, string cognome, string email, string password)
    {
        // Controlla se l'utente esiste già
        var userExists = false;
        using (var connection = new SqlConnection(DB.connectionString))
        {
            await connection.OpenAsync();
            var checkUserCmd = new SqlCommand("SELECT COUNT(1) FROM Utenti WHERE Email = @Email", connection);
            checkUserCmd.Parameters.AddWithValue("@Email", email);
            userExists = (int)await checkUserCmd.ExecuteScalarAsync() > 0;
        }

        if (userExists)
        {
            ViewBag.ErrorMessage = "Un utente con questa email esiste già.";
            return View();
        }

        // Inserisci l'utente nel database
        using (var connection = new SqlConnection(DB.connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("INSERT INTO Utenti (Nome, Cognome, Email, Password) VALUES (@Nome, @Cognome, @Email, @Password)", connection);
            command.Parameters.AddWithValue("@Nome", nome);
            command.Parameters.AddWithValue("@Cognome", cognome);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password); // Salva la password in chiaro

            var result = await command.ExecuteNonQueryAsync();
            if (result > 0)
            {
                return RedirectToAction("Login");
            }
            else
            {
                // Qualcosa è andato storto con l'inserimento nel DB
                ViewBag.ErrorMessage = "Si è verificato un errore durante la registrazione. Riprova.";
                return View();
            }
        }
    }









}
