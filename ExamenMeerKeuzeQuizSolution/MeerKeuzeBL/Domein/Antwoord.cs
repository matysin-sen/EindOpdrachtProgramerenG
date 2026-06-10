using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Antwoord
    {
        public Antwoord(bool isCorrect, string antwoordTekst, string letter)
        {
            IsCorrect = isCorrect;
            AntwoordTekst = antwoordTekst;
            Letter = letter;
        }
        public Antwoord(bool isCorrect, string antwoordTekst)
        {
            IsCorrect = isCorrect;
            AntwoordTekst = antwoordTekst;

        }
        public Antwoord(int id, bool isCorrect, string antwoordTekst)
        {
            AntwoordID = id;
            IsCorrect = isCorrect;
            AntwoordTekst = antwoordTekst;

        }

        public Antwoord()
        {
        }

        public int AntwoordID { get; init; }
        public bool IsCorrect { get; set; }
        public string AntwoordTekst { get; set; }
        public string Letter { get; set; }

       

        public override bool Equals(object? obj)
        {
            if (obj is Antwoord ander)
            {
                return this.AntwoordID == ander.AntwoordID;
            }
            return false;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AntwoordID, AntwoordTekst);
        }
    }
   
    
}
