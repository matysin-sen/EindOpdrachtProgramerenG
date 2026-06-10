using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class QuizOpstellen 
    {
        public int Id { get; set; } 
        public string Omschrijving { get; set; }
        public Onderwerp QuizOnderwerp { get;  set; }
        public List<Vraag> VragenLijst { get;  set; }

        // Een Dictionary die bijhoudt welke Vraag is beantwoord met welk Antwoord (+ letter)
        public Dictionary<Vraag, GegevenAntwoord> IngevuldeAntwoorden { get; set; } = new Dictionary<Vraag, GegevenAntwoord>();

        public int Score { get;  set; }

        // Constructor
        public QuizOpstellen(Onderwerp onderwerp, List<Vraag> vragen , string omschrijving)
        {
            QuizOnderwerp = onderwerp;
            VragenLijst = vragen;
            Omschrijving = omschrijving;
            IngevuldeAntwoorden = new Dictionary<Vraag, GegevenAntwoord>();
            Score = 0;
        }

        public QuizOpstellen()
        {
        }

        // Deze methode roep je vanuit je WPF UI aan als de gebruiker op "Volgende" klikt
        public void BeantwoordVraag(Vraag vraag, Antwoord gekozenAntwoord, string gekozenLetter)
        {
            // Sla het antwoord en de letter op in onze lijst
            
            foreach (var antwoord in IngevuldeAntwoorden)
            {
                if (!antwoord.Key.Equals(vraag))
                {
                    if(antwoord.Value.AntwoordObject.Equals(gekozenAntwoord))
                    {

                        throw new InvalidOperationException("Deze vraag is al beantwoord met een ander antwoord. Je kunt niet meerdere dezelfde antwoorden voor dezelfde vraag kiezen.");
                       
                    }
                }

            }
            IngevuldeAntwoorden[vraag] = new GegevenAntwoord(gekozenAntwoord, gekozenLetter);
            

            // Controleer direct of het juist is om de score te verhogen
            if (gekozenAntwoord != null && gekozenAntwoord.IsCorrect)
            {
                Score++;
            }

            for (int i = 0; i < VragenLijst.Count; i++)
            {
                for(int j = 0; j < VragenLijst[i].Antwoorden.Count; j++)
                {
                    if (VragenLijst[i].Antwoorden[j].Equals(VragenLijst[i].Antwoorden[j+1]))
                    {
                        throw new Exception("kan niet 2 keer hetzelfde antwoord hebben");
                    }
                }
            }
            
        }

        
    }

  
}
    

