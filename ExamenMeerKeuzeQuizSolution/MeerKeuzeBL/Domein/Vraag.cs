using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Vraag
    {
        public Vraag(int vraagID, string vraagzin, List<Antwoord> antwoorden, List<Onderwerpen> onderwerp)
        {
            VraagID = vraagID;
            VraagTekst = vraagzin;
            Antwoorden = antwoorden;
            Onderwerp = onderwerp;
        }
        public Vraag(string vraagzin, List<Antwoord> antwoorden , List<Onderwerpen> onderwerp)
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
        public List<Antwoord> Antwoorden { get; set; }
        public List<Onderwerpen> Onderwerp { get; set; }//voor als we meerdere onderwerpen per vraag willen


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
    }
}
