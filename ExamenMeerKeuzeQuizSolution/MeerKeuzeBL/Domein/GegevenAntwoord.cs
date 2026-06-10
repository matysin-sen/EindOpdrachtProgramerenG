using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class GegevenAntwoord
    {
        public Antwoord AntwoordObject { get; set; }
        public int AntwoordenID { get; set; } 
        public string Letter { get; set; }
        public string GekozenLetter { get; set; } // Bijv. "A"
        public bool IsCorrect { get; set; }
        

        public GegevenAntwoord(Antwoord antwoordObject, string letter)
        {
            AntwoordObject = antwoordObject;
            Letter = letter;
            AntwoordenID = antwoordObject.AntwoordID; 
            IsCorrect = antwoordObject.IsCorrect;
        }

        public GegevenAntwoord()
        {
        }
    }
}
