using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Domein
{
    public class Onderwerpen
    {
        public Onderwerpen(int onderwerpID, string onderwerpNaam)
        {
            OnderwerpID = onderwerpID;
            OnderwerpNaam = onderwerpNaam;
        }

        public int OnderwerpID { get; init; }
        public string OnderwerpNaam { get; set; }

        public override string? ToString()
        {
            return OnderwerpNaam;
        }
    }
}
