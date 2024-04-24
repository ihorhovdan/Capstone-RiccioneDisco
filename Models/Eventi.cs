using System.ComponentModel.DataAnnotations;

namespace RiccioneDisco.Models
{
    public class Eventi
    {
        public int IdEvento { get; set; }
        public string Titolo { get; set; }
        public string Data { get; set; }
        public string Luogo { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }

        // Assicurati che la collezione sia inizializzata
        public ICollection<CarrelloItemViewModel> CarrelloItems { get; set; } = new List<CarrelloItemViewModel>();
    }

}

