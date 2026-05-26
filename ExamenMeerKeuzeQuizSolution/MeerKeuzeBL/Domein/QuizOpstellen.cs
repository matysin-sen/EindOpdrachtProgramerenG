using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class QuizOpstellen
    {
        public int Id { get; set; } 
        public string Omschrijving { get; set; }
        public Onderwerpen QuizOnderwerp { get;  set; }
        public List<Vragen> VragenLijst { get;  set; }

        // Een Dictionary die bijhoudt welke Vraag is beantwoord met welk Antwoord (+ letter)
        public Dictionary<Vragen, GegevenAntwoorden> IngevuldeAntwoorden { get; set; } = new Dictionary<Vragen, GegevenAntwoorden>();

        public int Score { get;  set; }

        // Constructor
        public QuizOpstellen(Onderwerpen onderwerp, List<Vragen> vragen , string omschrijving)
        {
            QuizOnderwerp = onderwerp;
            VragenLijst = vragen;
            Omschrijving = omschrijving;
            IngevuldeAntwoorden = new Dictionary<Vragen, GegevenAntwoorden>();
            Score = 0;
        }

        public QuizOpstellen()
        {
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
    

