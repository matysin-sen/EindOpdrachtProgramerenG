using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class QuizOpstellen
    {
        public Onderwerpen QuizOnderwerp { get; private set; }
        public List<Vragen> VragenLijst { get; private set; }

        // Een Dictionary die bijhoudt welke Vraag is beantwoord met welk Antwoord (+ letter)
        public Dictionary<Vragen, GegevenAntwoorden> IngevuldeAntwoorden { get; private set; }

        public int Score { get; private set; }

        // Constructor
        public QuizOpstellen(Onderwerpen onderwerp, List<Vragen> vragen)
        {
            QuizOnderwerp = onderwerp;
            VragenLijst = vragen;
            IngevuldeAntwoorden = new Dictionary<Vragen, GegevenAntwoorden>();
            Score = 0;
        }

        // Deze methode roep je vanuit je WPF UI aan als de gebruiker op "Volgende" klikt
        public void BeantwoordVraag(Vragen vraag, Antwoorden gekozenAntwoord, string gekozenLetter)
        {
            // Sla het antwoord en de letter op in onze lijst
            IngevuldeAntwoorden[vraag] = new GegevenAntwoorden(gekozenAntwoord, gekozenLetter);

            // Controleer direct of het juist is om de score te verhogen
            if (gekozenAntwoord != null && gekozenAntwoord.IsCorrect)
            {
                Score++;
            }
        }
    }

  
}
    

