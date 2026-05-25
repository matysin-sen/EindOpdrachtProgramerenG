using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Vragen
    {
        public Vragen(int vraagID, string vraagzin, List<Antwoorden> antwoorden, List<Onderwerpen> onderwerp)
        {
            VraagID = vraagID;
            VraagTekst = vraagzin;
            Antwoorden = antwoorden;
            Onderwerp = onderwerp;
        }
        public Vragen(string vraagzin, List<Antwoorden> antwoorden , List<Onderwerpen> onderwerp)
        {
           
            VraagTekst = vraagzin;
            Antwoorden = antwoorden;
            Onderwerp = onderwerp;
        }

        public Vragen(string vraagTekst, List<Antwoorden> antwoorden)
        {
            VraagTekst = vraagTekst;
            Antwoorden = antwoorden;
        }

        public int VraagID { get; init; } // willen de vragen niet meer veranderen, dus init
        public string VraagTekst { get; set; }
        public List<Antwoorden> Antwoorden { get; set; }
        public List<Onderwerpen> Onderwerp { get; set; }//voor als we meerdere onderwerpen per vraag willen


        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
