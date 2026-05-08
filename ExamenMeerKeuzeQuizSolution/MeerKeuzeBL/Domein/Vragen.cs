using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Vragen
    {
        public int VraagID { get; init; } // willen de vragen niet meer veranderen, dus init
        public string VraagTekst { get; init; }
        public List<Antwoorden> Antwoorden { get; init; }
       

    }
}
