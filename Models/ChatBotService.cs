namespace RiccioneDisco.Models
{
    public class ChatBotService
    {
        private Dictionary<string, string> responses = new Dictionary<string, string>
        {
            {"ciao", "Ciao! Come posso aiutarti oggi?"},
            {"come stai", "Sono un bot, quindi non ho emozioni, ma grazie per aver chiesto!"},
            {"navette", "Le navette per le discoteche si prendono di fronte al Samsara, c'è un grande parcheggio, fermata 37 del pullman 11" }
            // Aggiungi qui altre domande e risposte
        };

        public string GetResponse(string userMessage)
        {
            string response = "Mi dispiace, non so come rispondere a questo. Puoi provare a formulare la domanda in un altro modo?";
            foreach (var entry in responses)
            {
                if (userMessage.ToLower().Contains(entry.Key))
                {
                    response = entry.Value;
                    break;
                }
            }
            return response;
        }





    }
}
