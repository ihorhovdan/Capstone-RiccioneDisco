using System.ComponentModel.DataAnnotations;

namespace RiccioneDisco.Models
{
    public class ChangeProfileViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio.")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Il campo Email è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Il campo Email non è un indirizzo email valido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Vecchia Password è obbligatorio.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Il campo Nuova Password è obbligatorio.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "La conferma della password non corrisponde.")]
        public string ConfirmPassword { get; set; }
    }



}
