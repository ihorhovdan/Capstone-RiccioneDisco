
    document.addEventListener("DOMContentLoaded", function () {
                // Trova il form tramite l'ID o la classe
                const form = document.querySelector("form");

    // Aggiungi un listener per l'evento di submit
    form.addEventListener("submit", function (event) {
                    // Ottieni i valori delle date di check-in e check-out
                    const dataArrivo = document.querySelector("input[name='DataArrivo']").value;
    const dataPartenza = document.querySelector("input[name='DataPartenza']").value;

    // Calcola la differenza in giorni
    const diffGiorni = (new Date(dataPartenza) - new Date(dataArrivo)) / (1000 * 60 * 60 * 24);

    // Seleziona il paragrafo per il messaggio di errore
    const errorMsg = document.querySelector("#error-msg");

    // Verifica se la differenza è di 7 giorni
    if (diffGiorni != 7) {
        // Se non è 7, mostra un messaggio di errore nel paragrafo e impedi l'invio del form
        errorMsg.textContent = "La prenotazione deve essere di esattamente 8 giorni e 7 notti. Per favore, seleziona nuovamente le date.";
    errorMsg.style.color = "red";
    event.preventDefault(); // Impedisce l'invio del form
                    } else {
        // Se la differenza è corretta, nasconde il messaggio di errore
        errorMsg.textContent = "";
                    }
                });
            });
