namespace RiccioneDisco.Models
{
    public class OrdineViewModel
    {
        public int IdOrdine { get; set; }
        public int UserId { get; set; }
        public decimal TotaleOrdine { get; set; }
        public string IndirizzoFatturazione { get; set; }
        public string NumeroCarta { get; set; }
        public string CVV { get; set; }
        public string DataScadenza { get; set; }

        public string NomeUtente { get; set; }
        public string CognomeUtente { get; set; }
        public string EmailUtente { get; set; }
        // Relazioni
        public virtual Utente Utente { get; set; }
        public List<CarrelloItemViewModel> Items { get; set; }


        public List<CarrelloItemViewModel> EventiAcquistati { get; set; }
    }
}
