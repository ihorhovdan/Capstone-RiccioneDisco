namespace RiccioneDisco.Models
{
    public class Moduli
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public int NumPersone { get; set; }

        public DateTime DataArrivo { get; set; }
        public DateTime DataPartenza { get; set; }
        public string Cellulare { get; set; }
    }
}
