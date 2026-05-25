using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class GegevenAntwoorden
    {
        public Antwoorden AntwoordObject { get; set; }
        public string Letter { get; set; }

        public GegevenAntwoorden(Antwoorden antwoordObject, string letter)
        {
            AntwoordObject = antwoordObject;
            Letter = letter;
        }
    }
}
