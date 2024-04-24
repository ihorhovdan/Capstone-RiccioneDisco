using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RiccioneDisco.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiccioneDisco.Controllers
{
    [Authorize] // Assicura che solo gli utenti loggati possano accedere a questa funzione
    public class ProfiloController : Controller
    {

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int parsedUserId))
            {
                var utente = await GetUtenteAsync(parsedUserId);
                var eventiAcquistati = await GetEventiAcquistatiAsync(parsedUserId);

                var viewModel = new ProfiloViewModel
                {
                    Utente = utente,
                    EventiAcquistati = eventiAcquistati ?? new List<Eventi>() // Assicura che non sia mai null
                };
                return View(viewModel);
            }
            else
            {
                return View(new ProfiloViewModel
                {
                    Utente = new Utente(), // Assicurati di inizializzare anche Utente se necessario
                    EventiAcquistati = new List<Eventi>() // Assicura che non sia mai null
                });
            }
        }



        private async Task<Utente> GetUtenteAsync(int userId)
        {
            Utente utente = null;
            using (var connection = new SqlConnection(DB.connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT IdUtente, Nome, Cognome, Email FROM Utenti WHERE IdUtente = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            utente = new Utente
                            {
                                IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente")),
                                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }
            return utente;
        }

        private async Task<List<Eventi>> GetEventiAcquistatiAsync(int userId)
        {
            var eventi = new List<Eventi>();
            using (var connection = new SqlConnection(DB.connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT e.IdEvento, e.Titolo, e.Data, e.Luogo, e.Prezzo, e.Immagine
                    FROM Eventi e
                    JOIN DettaglioOrdine do ON e.IdEvento = do.EventoId
                    JOIN Ordini o ON do.IdOrdine = o.IdOrdine
                    WHERE o.UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            eventi.Add(new Eventi
                            {
                                IdEvento = reader.GetInt32(reader.GetOrdinal("IdEvento")),
                                Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
                                Data = reader.GetString(reader.GetOrdinal("Data")),
                                Luogo = reader.GetString(reader.GetOrdinal("Luogo")),
                                Prezzo = reader.GetDecimal(reader.GetOrdinal("Prezzo")),
                                Immagine = reader.GetString(reader.GetOrdinal("Immagine"))
                            });
                        }
                    }
                }
            }
            return eventi;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Ottieni l'ID dell'utente autenticato
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            Utente user = null;
            using (var connection = new SqlConnection(DB.connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Nome, Cognome, Email FROM Utenti WHERE IdUtente = @IdUtente", connection);
                command.Parameters.AddWithValue("@IdUtente", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Utente
                        {
                            IdUtente = int.Parse(userId),
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Email = reader["Email"].ToString(),
                        };
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangeProfileViewModel
            {
                UserId = user.IdUtente.ToString(),
                Nome = user.Nome,
                Cognome = user.Cognome,
                Email = user.Email
            };

            return View(model);
        }





        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ChangeProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            using (var connection = new SqlConnection(DB.connectionString))
            {
                await connection.OpenAsync();

                // Prima verifica la vecchia password
                var commandCheck = new SqlCommand("SELECT Password FROM Utenti WHERE IdUtente = @IdUtente", connection);
                commandCheck.Parameters.AddWithValue("@IdUtente", model.UserId);

                var currentPassword = (string)await commandCheck.ExecuteScalarAsync();
                if (currentPassword != model.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "La vecchia password non è corretta.");
                    return View("Edit", model);
                }

                // Se la vecchia password è corretta, procedi all'aggiornamento
                var commandUpdate = new SqlCommand("UPDATE Utenti SET Nome = @Nome, Cognome = @Cognome, Email = @Email, Password = @Password WHERE IdUtente = @IdUtente", connection);
                commandUpdate.Parameters.AddWithValue("@Nome", model.Nome);
                commandUpdate.Parameters.AddWithValue("@Cognome", model.Cognome);
                commandUpdate.Parameters.AddWithValue("@Email", model.Email);
                commandUpdate.Parameters.AddWithValue("@Password", model.NewPassword);
                commandUpdate.Parameters.AddWithValue("@IdUtente", model.UserId);

                int result = await commandUpdate.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Profilo aggiornato con successo.";
                    return RedirectToAction("Index", "Profilo");
                }
                else
                {
                    ModelState.AddModelError("", "Si è verificato un errore nell'aggiornamento del profilo.");
                    return View("Edit", model);
                }
            }
        }

    }

}

