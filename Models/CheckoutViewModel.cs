using System.Collections.Generic;

namespace RiccioneDisco.Models
{
    public class CheckoutViewModel
    {
        public List<CarrelloItemViewModel> CarrelloItems { get; set; } = new List<CarrelloItemViewModel>(); // Inizializzazione qui
        public decimal TotaleOrdine { get; set; }
        // Aggiungi altre proprietà se necessario, ad esempio:
        public string IndirizzoFatturazione { get; set; }
        public string NumeroCarta { get; set; }
        public string CVV { get; set; }
        public string DataScadenza { get; set; }
        // Aggiungi altre proprietà se necessario, come l'indirizzo di spedizione
    }
}
