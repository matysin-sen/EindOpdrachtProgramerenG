using MeerKeuzeBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Interface
{
    public interface IVragenRepository
    {
        public void VoegVraagToe(Vragen vraag);

        public List<Onderwerpen> GeefAlleOnderwerpen();
        public Onderwerpen VoegOnderwerpToe(string onderwerpNaam);

        public List<Vragen> GeefVragenVoorOnderwerp(int onderwerpID);

        public void voegUserToe(string naam, string achternaam);

    }
}
