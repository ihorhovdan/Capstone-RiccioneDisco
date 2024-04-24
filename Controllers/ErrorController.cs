using Microsoft.AspNetCore.Mvc;
using RiccioneDisco.Models;
using System.Diagnostics;

public class ErrorController : Controller
{
    [HttpGet("/Error")]
    public IActionResult Error()
    {
        // Ottenere l'ID della richiesta (se disponibile)
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        var errorViewModel = new ErrorViewModel
        {
            RequestId = requestId,
            Message = "An error occurred while processing your request." // Aggiungi un messaggio di errore se necessario
        };

        return View(errorViewModel);
    }
}