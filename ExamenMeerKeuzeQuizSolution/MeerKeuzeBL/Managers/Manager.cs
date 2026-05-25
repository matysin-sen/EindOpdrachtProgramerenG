using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Managers
{
    public class Manager
    {
        IVragenRepository _repository;

        public Manager(IVragenRepository repository)
        {
            _repository = repository;   
        }

        public Manager()
        {
        }

        public void VoegUserToe(string naam, string achternaam)
        {
            _repository.voegUserToe(naam, achternaam);
        }

      
        public List<Onderwerpen> GeefAlleOnderwerpen()
        {
            return _repository.GeefAlleOnderwerpen();
        }
        public QuizOpstellen StartNieuweQuiz(Onderwerpen onderwerp, int aantalVragen)
        {
            // 1. Haal de vragen op uit de databank via de repository
            List<Vragen> geselecteerdeVragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerp.OnderwerpID, aantalVragen);

            // 2. Controleer of de databank wel genoeg vragen had
            if (geselecteerdeVragen == null || geselecteerdeVragen.Count == 0)
            {
                throw new System.Exception($"Er zijn geen vragen gevonden in de databank voor het onderwerp '{onderwerp.OnderwerpNaam}'.");
            }

            // 3. Maak het nieuwe quiz sessie-object aan
            return new QuizOpstellen(onderwerp, geselecteerdeVragen);
        }
    }
}
