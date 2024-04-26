using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RiccioneDisco.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace RiccioneDisco.Controllers
{
    public class PacchettiController : Controller
    {
        private readonly EmailService _emailService;

        public PacchettiController(EmailService emailService)
        {
            _emailService = emailService;
        }
        public IActionResult Ardea()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Ardea(Moduli moduli)
        {
            if (!ModelState.IsValid)
            {
                return View(moduli);
            }

            var error = false;
            try
            {
                DB.conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Ardea
                                       (Nome, Cognome, Email, NumPersone, DataArrivo, DataPartenza, Cellulare)
                                       VALUES (@nome, @cognome, @email, @numPersone, @dataArrivo, @dataPartenza, @cellulare)", DB.conn);
                cmd.Parameters.AddWithValue("@nome", moduli.Nome);
                cmd.Parameters.AddWithValue("@cognome", moduli.Cognome);
                cmd.Parameters.AddWithValue("@email", moduli.Email);
                cmd.Parameters.AddWithValue("@numPersone", moduli.NumPersone);
                cmd.Parameters.AddWithValue("@dataArrivo", moduli.DataArrivo);
                cmd.Parameters.AddWithValue("@dataPartenza", moduli.DataPartenza);
                cmd.Parameters.AddWithValue("@cellulare", moduli.Cellulare);

                var nRows = cmd.ExecuteNonQuery();
                if (nRows > 0)
                {
                    error = false;
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }

            if (!error)
            {
                string logoUrl = "https://riccionedisco.it/images/logo1.png";

                string messageBody = $"<img src='{logoUrl}' alt='Logo' style='max-width: 200px;'><br><br><br>" +
                                     $"Gentile, {moduli.Nome} {moduli.Cognome}, grazie per averci contattato per la tua prossima vacanza. Siamo lieti di fornirti un preventivo per il soggiorno desiderato.<br><br>" +
                                     $"Di seguito troverai i dettagli relativi al tuo preventivo:<br>" +
                                     $"Data di arrivo: {moduli.DataArrivo.Date.ToString("dd/MM/yyyy")}<br>" +
                                     $"Data di partenza: {moduli.DataPartenza.Date.ToString("dd/MM/yyyy")}<br>" +
                                     $"Numero di adulti: {moduli.NumPersone}<br><br>" +
                                     $"Ti invitiamo a controllare attentamente le informazioni fornite. Se hai bisogno di ulteriori informazioni o desideri apportare delle modifiche al tuo preventivo, non esitare a contattarci. Siamo qui per assisterti al meglio e garantire che il tuo soggiorno presso l'Hotel Ardea sia indimenticabile.<br><br>" +
                                     $"Ti ringraziamo ancora per aver scelto di prenotare con noi e non vediamo l'ora di darti il benvenuto.<br><br>" +
                                     $"Cordiali saluti,<br>" +
                                     $"Il team di Riccione Disco";

                _emailService.SendEmail(moduli.Email, "Riccione Disco - Conferma Prenotazione", messageBody);
                TempData["MessageSuccess"] = "Preventivo inviato, sarai contattato a breve!";
            }
            else
            {
                TempData["MessageError"] = "Errore durante l'invio del preventivo.";
            }

            return RedirectToAction("Index", "Home");
        }








        public IActionResult LasVegas()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Lasvegas(Moduli moduli)
        {
            var error = true;
            try
            {
                DB.conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO LasVegas
                                           (Nome, Cognome, Email, NumPersone, DataArrivo, DataPartenza, Cellulare)
                                           VALUES (@nome, @cognome, @email, @numPersone, @dataArrivo, @dataPartenza, @cellulare)", DB.conn);
                cmd.Parameters.AddWithValue("@nome", moduli.Nome);
                cmd.Parameters.AddWithValue("@cognome", moduli.Cognome);
                cmd.Parameters.AddWithValue("@email", moduli.Email);
                cmd.Parameters.AddWithValue("@numPersone", moduli.NumPersone);
                cmd.Parameters.AddWithValue("@dataArrivo", moduli.DataArrivo);
                cmd.Parameters.AddWithValue("@dataPartenza", moduli.DataPartenza);
                cmd.Parameters.AddWithValue("@cellulare", moduli.Cellulare);

                var nRows = cmd.ExecuteNonQuery();
                if (nRows > 0)
                {
                    error = false;
                }

            }
            catch (Exception ex)
            {

                return View("Error", ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }


            if (!error)
            {
                TempData["MessageSuccess"] = $"Preventivo inviato, sarai contattato a breve!";
            }
            else
            {
                TempData["MessageError"] = $"Errore durante l'invio del preventivo.";

            }

            return RedirectToAction("Index", "Home");
        }








        public IActionResult Modenese()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Modenese(Moduli moduli)
        {
            var error = true;
            try
            {
                DB.conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Modenese
                                           (Nome, Cognome, Email, NumPersone, DataArrivo, DataPartenza, Cellulare)
                                           VALUES (@nome, @cognome, @email, @numPersone, @dataArrivo, @dataPartenza, @cellulare)", DB.conn);
                cmd.Parameters.AddWithValue("@nome", moduli.Nome);
                cmd.Parameters.AddWithValue("@cognome", moduli.Cognome);
                cmd.Parameters.AddWithValue("@email", moduli.Email);
                cmd.Parameters.AddWithValue("@numPersone", moduli.NumPersone);
                cmd.Parameters.AddWithValue("@dataArrivo", moduli.DataArrivo);
                cmd.Parameters.AddWithValue("@dataPartenza", moduli.DataPartenza);
                cmd.Parameters.AddWithValue("@cellulare", moduli.Cellulare);

                var nRows = cmd.ExecuteNonQuery();
                if (nRows > 0)
                {
                    error = false;
                }

            }
            catch (Exception ex)
            {

                return View("Error", ex.Message);
            }
            finally
            {
                DB.conn.Close();
            }


            if (!error)
            {
                TempData["MessageSuccess"] = $"Preventivo inviato, sarai contattato a breve!";
            }
            else
            {
                TempData["MessageError"] = $"Errore durante l'invio del preventivo.";

            }

            return RedirectToAction("Index", "Home");
        }

        
            

            
        

    }
}
