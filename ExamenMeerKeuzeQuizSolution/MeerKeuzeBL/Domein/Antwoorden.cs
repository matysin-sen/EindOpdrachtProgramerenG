using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Antwoorden
    {
        public Antwoorden(bool isCorrect, string antwoordTekst, string letter)
        {
            IsCorrect = isCorrect;
            AntwoordTekst = antwoordTekst;
            Letter = letter;
        }
        public Antwoorden(bool isCorrect, string antwoordTekst)
        {
            IsCorrect = isCorrect;
            AntwoordTekst = antwoordTekst;
          
        }
        public bool IsCorrect { get; set; }
        public string AntwoordTekst { get; init; }
        public string Letter { get; set; }


    }
}
