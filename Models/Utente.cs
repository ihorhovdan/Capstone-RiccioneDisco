using RiccioneDisco.Models;
using System.ComponentModel.DataAnnotations;
namespace RiccioneDisco.Models
{

    public class Utente
    {
        public int IdUtente { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        public string Cognome { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Formato email non valido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, ErrorMessage = "La password deve essere lunga almeno 6 caratteri.", MinimumLength = 6)]
        public string Password { get; set; }

        public ICollection<CarrelloItemViewModel> CarrelloItems { get; set; }
        public ICollection<OrdineViewModel> Ordini { get; set; }
    }

}