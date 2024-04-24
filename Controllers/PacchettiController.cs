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
        public IActionResult Ardea()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Ardea(Moduli moduli)
        {
            var error = true;
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
                TempData["MessageSuccess"] = $"Preventivo inviato, sarai contattato a breve!";
            }
            else
            {
                TempData["MessageError"] = $"Errore durante l'invio del preventivo.";

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
