using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Data.SqlClient;
using RiccioneDisco.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Transactions;

namespace RiccioneDisco.Controllers
{
    public class CarrelloController : Controller
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AggiungiAlCarrello(int eventoId, int quantita)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    using (var connection = new SqlConnection(DB.connectionString))
                    {
                        await connection.OpenAsync();
                        var command = new SqlCommand("INSERT INTO CarrelloItems (EventoId, UserId, Quantita) VALUES (@EventoId, @UserId, @Quantita)", connection);
                        command.Parameters.AddWithValue("@EventoId", eventoId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Quantita", quantita);

                        var result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return RedirectToAction("Index", "AcquistaEventi"); // Redirect all'azione che mostra il carrello
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Errore durante l'aggiunta al carrello.";
                            return View("Error"); // Mostra una pagina di errore generica
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new ErrorViewModel { Message = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "AcquistaEventi");
            }
        }
        [Authorize]
        public async Task<IActionResult> VisualizzaCarrello()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    using (var connection = new SqlConnection(DB.connectionString))
                    {
                        await connection.OpenAsync();
                        var query = "SELECT ci.CarrelloItemId, ci.EventoId, ci.Quantita, e.Titolo, e.Prezzo, e.Immagine FROM CarrelloItems ci JOIN Eventi e ON ci.EventoId = e.IdEvento WHERE ci.UserId = @UserId";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            var carrelloItems = new List<CarrelloItemViewModel>();
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    carrelloItems.Add(new CarrelloItemViewModel
                                    {
                                        CarrelloItemId = (int)reader["CarrelloItemId"],
                                        EventoId = (int)reader["EventoId"],
                                        Quantita = (int)reader["Quantita"],
                                        TitoloEvento = reader["Titolo"].ToString(),
                                        Prezzo = (decimal)reader["Prezzo"],
                                        ImageUrl = reader["Immagine"].ToString()  // Recupera l'URL dell'immagine
                                    });
                                }
                            }

                            return View(carrelloItems); 
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new ErrorViewModel { Message = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "AcquistaEventi");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RimuoviDalCarrello(int itemId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    using (var connection = new SqlConnection(DB.connectionString))
                    {
                        await connection.OpenAsync();
                        var command = new SqlCommand("DELETE FROM CarrelloItems WHERE CarrelloItemId = @ItemId AND UserId = @UserId", connection);
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        command.Parameters.AddWithValue("@UserId", userId);

                        var result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            // Redirect alla pagina del carrello con un messaggio di successo
                            return RedirectToAction("VisualizzaCarrello");
                        }
                        else
                        {
                            // Gestione del caso in cui l'articolo non esiste o non appartiene all'utente
                            ViewBag.ErrorMessage = "Errore durante la rimozione dell'articolo dal carrello.";
                            return View("Error"); // Mostra una pagina di errore generica
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new ErrorViewModel { Message = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "AcquistaEventi");
            }
        }


        private async Task<List<CarrelloItemViewModel>> GetCarrelloItemsAsync(int userId)
        {
            var carrelloItems = new List<CarrelloItemViewModel>();

            try
            {
                using (var connection = new SqlConnection(DB.connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                SELECT ci.CarrelloItemId, ci.EventoId, ci.Quantita, e.Titolo, e.Prezzo, e.Immagine 
                FROM CarrelloItems ci 
                JOIN Eventi e ON ci.EventoId = e.IdEvento 
                WHERE ci.UserId = @UserId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var carrelloItem = new CarrelloItemViewModel
                                {
                                    CarrelloItemId = reader.GetInt32(reader.GetOrdinal("CarrelloItemId")),
                                    EventoId = reader.GetInt32(reader.GetOrdinal("EventoId")),
                                    Quantita = reader.GetInt32(reader.GetOrdinal("Quantita")),
                                    TitoloEvento = reader.GetString(reader.GetOrdinal("Titolo")),
                                    Prezzo = reader.GetDecimal(reader.GetOrdinal("Prezzo")),
                                    ImageUrl = reader.GetString(reader.GetOrdinal("Immagine"))
                                };

                                carrelloItems.Add(carrelloItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante il recupero degli elementi del carrello: " + ex.Message);
                throw;
            }

            return carrelloItems;
        }



        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var carrelloItems = await GetCarrelloItemsAsync(userId);

                if (carrelloItems == null || carrelloItems.Count == 0)
                {
                    TempData["ErrorMessage"] = "Il carrello è vuoto.";
                    return RedirectToAction("VisualizzaCarrello");
                }

                var totaleOrdine = carrelloItems.Sum(item => item.Subtotale); // Utilizza la proprietà Subtotale per calcolare il totale

                var model = new CheckoutViewModel
                {
                    CarrelloItems = carrelloItems,
                    TotaleOrdine = totaleOrdine
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "AcquistaEventi");
            }
        }





        [HttpPost]
        public async Task<IActionResult> ConfermaOrdine(CheckoutViewModel model)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var carrelloItems = await GetCarrelloItemsAsync(userId);
                if (carrelloItems == null || carrelloItems.Count == 0)
                {
                    ViewBag.ErrorMessage = "Non ci sono articoli nel carrello.";
                    return View("Error", new ErrorViewModel { Message = "Non ci sono articoli da confermare." });
                }

                using (var connection = new SqlConnection(DB.connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Inserimento dell'ordine nella tabella Ordini e recupero dell'ID dell'ordine inserito
                            var command = new SqlCommand(@"INSERT INTO Ordini (UserId, TotaleOrdine, IndirizzoFatturazione, NumeroCarta, CVV, DataScadenza) 
                                                    OUTPUT INSERTED.IdOrdine 
                                                    VALUES (@UserId, @TotaleOrdine, @IndirizzoFatturazione, @NumeroCarta, @CVV, @DataScadenza)",
                                                            connection, transaction);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@TotaleOrdine", model.TotaleOrdine);
                            command.Parameters.AddWithValue("@IndirizzoFatturazione", model.IndirizzoFatturazione);
                            command.Parameters.AddWithValue("@NumeroCarta", model.NumeroCarta);
                            command.Parameters.AddWithValue("@CVV", model.CVV);
                            command.Parameters.AddWithValue("@DataScadenza", model.DataScadenza);

                            var ordineId = (int)await command.ExecuteScalarAsync();

                            // Inserimento dei dettagli dell'ordine nella tabella DettaglioOrdine
                            foreach (var item in carrelloItems)
                            {
                                var dettaglioCommand = new SqlCommand(@"INSERT INTO DettaglioOrdine (IdOrdine, EventoId, Quantita, Prezzo, TitoloEvento, ImmagineEvento) 
                                                                VALUES (@IdOrdine, @EventoId, @Quantita, @Prezzo, @TitoloEvento, @ImmagineEvento)",
                                                                        connection, transaction);
                                dettaglioCommand.Parameters.AddWithValue("@IdOrdine", ordineId);
                                dettaglioCommand.Parameters.AddWithValue("@EventoId", item.EventoId);
                                dettaglioCommand.Parameters.AddWithValue("@Quantita", item.Quantita);
                                dettaglioCommand.Parameters.AddWithValue("@Prezzo", item.Prezzo);
                                dettaglioCommand.Parameters.AddWithValue("@TitoloEvento", item.TitoloEvento);
                                dettaglioCommand.Parameters.AddWithValue("@ImmagineEvento", item.ImageUrl);
                                await dettaglioCommand.ExecuteNonQueryAsync();
                            }

                            // Pulizia del carrello dopo la conferma dell'ordine
                            var cleanCartCommand = new SqlCommand("DELETE FROM CarrelloItems WHERE UserId = @UserId", connection, transaction);
                            cleanCartCommand.Parameters.AddWithValue("@UserId", userId);
                            await cleanCartCommand.ExecuteNonQueryAsync();

                            transaction.Commit();

                            
                            return RedirectToAction("DettagliOrdine", "Carrello", new { id = ordineId });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.ErrorMessage = "Si è verificato un errore durante il completamento dell'ordine. Si prega di riprovare più tardi.";
                            return View("Error", new ErrorViewModel { Message = ex.Message });
                        }
                    }
                }
            }
            else
            {
                return View("Checkout", model);
            }
        }





        [Authorize]
        public async Task<IActionResult> DettagliOrdine(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ordine = await GetOrdineAsync(id, userId);

            if (ordine == null)
            {
                ViewBag.ErrorMessage = "Ordine non trovato.";
                return View("Error");
            }

            return View(ordine);
        }

        private async Task<OrdineViewModel> GetOrdineAsync(int ordineId, int userId)
        {
            OrdineViewModel ordine = null;
            try
            {
                using (var connection = new SqlConnection(DB.connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                SELECT o.IdOrdine, o.UserId, o.TotaleOrdine, o.IndirizzoFatturazione, o.NumeroCarta, o.CVV, o.DataScadenza,
                       u.Nome, u.Cognome, u.Email
                FROM Ordini o
                JOIN Utenti u ON o.UserId = u.IdUtente
                WHERE o.IdOrdine = @IdOrdine AND o.UserId = @UserId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdOrdine", ordineId);
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                ordine = new OrdineViewModel
                                {
                                    IdOrdine = reader.GetInt32(reader.GetOrdinal("IdOrdine")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    TotaleOrdine = reader.GetDecimal(reader.GetOrdinal("TotaleOrdine")),
                                    IndirizzoFatturazione = reader.GetString(reader.GetOrdinal("IndirizzoFatturazione")),
                                    NumeroCarta = reader.GetString(reader.GetOrdinal("NumeroCarta")),
                                    CVV = reader.GetString(reader.GetOrdinal("CVV")),
                                    DataScadenza = reader.GetString(reader.GetOrdinal("DataScadenza")),
                                    NomeUtente = reader.GetString(reader.GetOrdinal("Nome")),
                                    CognomeUtente = reader.GetString(reader.GetOrdinal("Cognome")),
                                    EmailUtente = reader.GetString(reader.GetOrdinal("Email")),
                                    Items = await GetDettaglioOrdineItemsAsync(ordineId) // Cambia qui
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nel recupero dell'ordine: " + ex.Message);
                throw;
            }
            return ordine;
        }

        private async Task<List<CarrelloItemViewModel>> GetDettaglioOrdineItemsAsync(int ordineId)
        {
            var items = new List<CarrelloItemViewModel>();

            using (var connection = new SqlConnection(DB.connectionString))
            {
                await connection.OpenAsync();
                var query = @"
            SELECT d.EventoId, d.Quantita, d.Prezzo, d.TitoloEvento, d.ImmagineEvento
            FROM DettaglioOrdine d
            WHERE d.IdOrdine = @IdOrdine";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdOrdine", ordineId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new CarrelloItemViewModel
                            {
                                EventoId = reader.GetInt32(reader.GetOrdinal("EventoId")),
                                Quantita = reader.GetInt32(reader.GetOrdinal("Quantita")),
                                Prezzo = reader.GetDecimal(reader.GetOrdinal("Prezzo")),
                                TitoloEvento = reader.GetString(reader.GetOrdinal("TitoloEvento")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("ImmagineEvento"))
                            });
                        }
                    }
                }
            }

            return items;
        }


        



    }
}
