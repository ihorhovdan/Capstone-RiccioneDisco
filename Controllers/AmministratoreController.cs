using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RiccioneDisco.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiccioneDisco.Controllers
{
    public class AmministratoreController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (email == "lol@lol.it" && password == "lol@lol.it")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Administrator")  // Add a role claim to identify as Administrator
                };

                var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToAction("Amministratore", "Amministratore");
            }

            ViewData["ErroreLogin"] = "Credenziali non valide.";
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult Amministratore()
        {
            
            List<Moduli> ardeaData = new List<Moduli>();
            List<Moduli> lasVegasData = new List<Moduli>();
            List<Moduli> modeneseData = new List<Moduli>();

            try
            {
                // Codice per ottenere i dati della tabella Ardea
                DB.conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Ardea", DB.conn);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var compilazione = new Moduli()
                        {
                            ID = (int)reader["ID"],
                            DataArrivo = (DateTime)reader["DataArrivo"],
                            DataPartenza = (DateTime)reader["DataPartenza"],
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Cellulare = reader["Cellulare"].ToString(),
                            NumPersone = (int)reader["NumPersone"]
                        };

                        ardeaData.Add(compilazione);
                    }

                    reader.Close();
                }

                // Codice per ottenere i dati della tabella LasVegas
                cmd = new SqlCommand("SELECT * FROM LasVegas", DB.conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var compilazione = new Moduli()
                        {
                            ID = (int)reader["ID"],
                            DataArrivo = (DateTime)reader["DataArrivo"],
                            DataPartenza = (DateTime)reader["DataPartenza"],
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Cellulare = reader["Cellulare"].ToString(),
                            NumPersone = (int)reader["NumPersone"]
                        };

                        lasVegasData.Add(compilazione);
                    }

                    reader.Close();
                }

                // Codice per ottenere i dati della tabella Modenese
                cmd = new SqlCommand("SELECT * FROM Modenese", DB.conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var compilazione = new Moduli()
                        {
                            ID = (int)reader["ID"],
                            DataArrivo = (DateTime)reader["DataArrivo"],
                            DataPartenza = (DateTime)reader["DataPartenza"],
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Cellulare = reader["Cellulare"].ToString(),
                            NumPersone = (int)reader["NumPersone"]
                        };

                        modeneseData.Add(compilazione);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            finally
            {
                DB.conn.Close();
            }

            ViewData["ModelLasVegas"] = lasVegasData;
            ViewData["ModelModenese"] = modeneseData;

            return View(ardeaData);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }



        [Authorize(Roles = "Administrator")]
        public IActionResult AddEvent()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddEvent([Bind("Titolo, Data, Luogo, Prezzo, Immagine")] Eventi evento)
        {
            if (!ModelState.IsValid)
            {
                // Log di tutti gli errori di validazione nel ModelState
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        foreach (var error in entry.Value.Errors)
                        {
                            // Stampa o logga l'errore specifico
                            Debug.WriteLine($"Error in {entry.Key}: {error.ErrorMessage}");
                        }
                    }
                }

                // Ritorna alla vista con il modello corrente e gli errori di validazione
                return View(evento);
            }

            // La logica di inserimento procede solo se il ModelState è valido
            string queryString = @"INSERT INTO Eventi (Titolo, Data, Luogo, Prezzo, Immagine) 
                           VALUES (@Titolo, @Data, @Luogo, @Prezzo, @Immagine)";
            try
            {
                using (var connection = new SqlConnection(DB.connectionString)) // Assicurati che DB.ConnectionString sia definito
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@Titolo", evento.Titolo);
                    command.Parameters.AddWithValue("@Data", evento.Data);
                    command.Parameters.AddWithValue("@Luogo", evento.Luogo);
                    command.Parameters.AddWithValue("@Prezzo", evento.Prezzo);
                    command.Parameters.AddWithValue("@Immagine", evento.Immagine);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();  // Esegue il comando
                    connection.Close();
                }
                TempData["SuccessMessage"] = "Evento aggiunto con successo!";

                return RedirectToAction("AddEvent","Amministratore");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Errore durante il salvataggio dell'evento: " + ex.Message);
                return View(evento);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult ManageEvents(string discoteca)
        {
            List<Eventi> eventi = new List<Eventi>();

            // Conserviamo il filtro attuale nella ViewData per poterlo riutilizzare nella view
            ViewData["SelectedDiscoteca"] = discoteca;

            try
            {
                using (var connection = new SqlConnection(DB.connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    // Verifica se è stata selezionata una discoteca specifica o se si desidera vedere tutte le discoteche
                    if (!string.IsNullOrEmpty(discoteca) && discoteca != "Tutte le discoteche")
                    {
                        cmd = new SqlCommand("SELECT IdEvento, Titolo, Data, Luogo, Prezzo, Immagine FROM Eventi WHERE Luogo = @Luogo", connection);
                        cmd.Parameters.AddWithValue("@Luogo", discoteca);
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT IdEvento, Titolo, Data, Luogo, Prezzo, Immagine FROM Eventi", connection);
                    }

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        eventi.Add(new Eventi()
                        {
                            IdEvento = (int)reader["IdEvento"],
                            Titolo = reader["Titolo"].ToString(),
                            Data = reader["Data"].ToString(),
                            Luogo = reader["Luogo"].ToString(),
                            Prezzo = (decimal)reader["Prezzo"],
                            Immagine = reader["Immagine"].ToString()
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Errore durante il recupero degli eventi: " + ex.Message);
                return View("Error");
            }
            finally
            {
                DB.conn.Close();
            }

            return View(eventi);
        }






        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateEvent(Eventi updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                return View("ManageEvents", updatedEvent);
            }
            try
            {
                using (var connection = new SqlConnection(DB.connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand("UPDATE Eventi SET Titolo = @Titolo, Data = @Data, Luogo = @Luogo, Prezzo = @Prezzo, Immagine = @Immagine WHERE IdEvento = @IdEvento", connection);
                    cmd.Parameters.AddWithValue("@Titolo", updatedEvent.Titolo);
                    cmd.Parameters.AddWithValue("@Data", updatedEvent.Data);
                    cmd.Parameters.AddWithValue("@Luogo", updatedEvent.Luogo);
                    cmd.Parameters.AddWithValue("@Prezzo", updatedEvent.Prezzo);
                    cmd.Parameters.AddWithValue("@Immagine", updatedEvent.Immagine);
                    cmd.Parameters.AddWithValue("@IdEvento", updatedEvent.IdEvento);
                    await cmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = "Evento modificato con successo!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating event: " + ex.Message);
                return View("ManageEvents", updatedEvent);
            }
            return RedirectToAction("ManageEvents");
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                using (var connection = new SqlConnection(DB.connectionString))
                {
                    connection.Open();
                    var deleteRelatedItemsCmd = new SqlCommand("DELETE FROM CarrelloItems WHERE EventoId = @EventoId", connection);
                    deleteRelatedItemsCmd.Parameters.AddWithValue("@EventoId", id);
                    await deleteRelatedItemsCmd.ExecuteNonQueryAsync();

                    var deleteEventCmd = new SqlCommand("DELETE FROM Eventi WHERE IdEvento = @IdEvento", connection);
                    deleteEventCmd.Parameters.AddWithValue("@IdEvento", id);
                    await deleteEventCmd.ExecuteNonQueryAsync();
                }
                TempData["SuccessMessage"] = "Evento eliminato con successo!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Errore durante l'eliminazione dell'evento: " + ex.Message);
                return View("Error");
            }
            return RedirectToAction("ManageEvents");
        }







    }





}

