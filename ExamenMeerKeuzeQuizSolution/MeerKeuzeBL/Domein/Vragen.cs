using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Vragen
    {
        public Vragen(int vraagID, string vraagTekst, List<Antwoorden> antwoorden)
        {
            VraagID = vraagID;
            VraagTekst = vraagTekst;
            Antwoorden = antwoorden;
        }
        public Vragen(string vraagTekst, List<Antwoorden> antwoorden)
        {
           
            VraagTekst = vraagTekst;
            Antwoorden = antwoorden;
        }
        public int VraagID { get; init; } // willen de vragen niet meer veranderen, dus init
        public string VraagTekst { get; init; }
        public List<Antwoorden> Antwoorden { get; init; }


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
