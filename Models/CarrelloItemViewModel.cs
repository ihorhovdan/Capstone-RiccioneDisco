namespace RiccioneDisco.Models
{
    public class CarrelloItemViewModel
    {
        public int CarrelloItemId { get; set; }
        public int EventoId { get; set; }
        public int Quantita { get; set; }
        public decimal Subtotale => Prezzo * Quantita;

        // Relazioni
        public virtual Eventi Evento { get; set; }
        public virtual Utente Utente { get; set; }

        // Proprietà non inclusa nel modello originale, ma necessaria per il calcolo
        public decimal Prezzo { get; set; }
        public string TitoloEvento { get; set; }  // Aggiunta per facilitare l'accesso al titolo dell'evento
        public string ImageUrl { get; set; }  // Aggiunta per facilitare l'accesso all'immagine dell'evento
    }
}
