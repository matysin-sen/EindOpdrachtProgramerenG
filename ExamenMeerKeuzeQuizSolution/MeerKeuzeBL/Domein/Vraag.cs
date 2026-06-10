using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Vraag
    {
        public Vraag(int vraagID, string vraagzin, List<Antwoord> antwoorden, List<Onderwerp> onderwerp)
        {
            VraagID = vraagID;
            VraagTekst = vraagzin;
            Antwoorden = antwoorden;
            Onderwerp = onderwerp;
        }
        public Vraag(string vraagzin, List<Antwoord> antwoorden , List<Onderwerp> onderwerp)
        {
           
            VraagTekst = vraagzin;
            Antwoorden = antwoorden;
            Onderwerp = onderwerp;
        }

        public Vraag(string vraagTekst, List<Antwoord> antwoorden)
        {
            VraagTekst = vraagTekst;
            Antwoorden = antwoorden;
        }

        public Vraag()
        {
        }

        public int VraagID { get; init; } // willen de vragen niet meer veranderen, dus init
        public string VraagTekst { get; set; }
        public List<Antwoord> Antwoorden { get; set; } = new List<Antwoord>();
        public List<Onderwerp> Onderwerp { get; set; }//voor als we meerdere onderwerpen per vraag willen


        public override bool Equals(object? obj)
        {
            return base.Equals(obj as Vraag);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        public bool Equals(Vraag? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return VraagID == other.VraagID;
        }

        public void VoegAntwoordToe(Antwoord nieuwAntwoord)
        {
            // Controleer of de tekst van het antwoord al bestaat in de huidige lijst
            if (Antwoorden.Any(a => a.AntwoordTekst.Equals(nieuwAntwoord.AntwoordTekst, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Dit antwoord bestaat al bij deze vraag.");
            }

            Antwoorden.Add(nieuwAntwoord);
        }
    }
}
