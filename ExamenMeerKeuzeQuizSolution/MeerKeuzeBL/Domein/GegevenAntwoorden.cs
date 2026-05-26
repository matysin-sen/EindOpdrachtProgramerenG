using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class GegevenAntwoorden
    {
        public Antwoorden AntwoordObject { get; set; }
        public int AntwoordenID { get; set; } 
        public string Letter { get; set; }
        public string GekozenLetter { get; set; } // Bijv. "A"
        public bool IsCorrect { get; set; }
        public GegevenAntwoorden(Antwoorden antwoordObject, string letter)
        {
            AntwoordObject = antwoordObject;
            Letter = letter;
            AntwoordenID = antwoordObject.AntwoordID; // ← dit toevoegen
            IsCorrect = antwoordObject.IsCorrect;
        }

        public GegevenAntwoorden()
        {
        }
    }
}
