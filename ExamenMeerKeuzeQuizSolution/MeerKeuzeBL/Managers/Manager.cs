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
        //public QuizOpstellen StartNieuweQuiz(Onderwerpen onderwerp, int aantalVragen)
        //{
        //    // 1. Haal de vragen op uit de databank via de repository
        //    List<Vragen> geselecteerdeVragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerp.OnderwerpID, aantalVragen);

        //    // 2. Controleer of de databank wel genoeg vragen had
        //    if (geselecteerdeVragen == null || geselecteerdeVragen.Count == 0)
        //    {
        //        throw new System.Exception($"Er zijn geen vragen gevonden in de databank voor het onderwerp '{onderwerp.OnderwerpNaam}'.");
        //    }

        //    // 3. Maak het nieuwe quiz sessie-object aan
        //    return new QuizOpstellen(onderwerp, geselecteerdeVragen);
        //}


        public QuizOpstellen GenereerRandomQuiz(Onderwerpen gekozenOnderwerp, int aantalVragen, string omschrijving)
        {
            // 1. Haal alle vragen op van dit specifieke onderwerp.
            // Zorg dat je een methode in je repository hebt die dit doet!
            List<Vragen> alleVragenVoorOnderwerp = _repository.GeefRandomVragenVoorOnderwerp(gekozenOnderwerp.OnderwerpID, aantalVragen);

            // 2. Controleer of we wel genoeg vragen in de database hebben
            if (alleVragenVoorOnderwerp.Count < aantalVragen)
            {
                throw new Exception($"Je vraagt om {aantalVragen} vragen, maar er zitten er maar {alleVragenVoorOnderwerp.Count} in de database voor dit onderwerp.");
            }

            // 3. De magie: Schud de lijst willekeurig door elkaar en pak de eerste 'aantalVragen'
            Random rnd = new Random();
            List<Vragen> randomSelectie = alleVragenVoorOnderwerp.OrderBy(x => rnd.Next()).Take(aantalVragen).ToList();

            // 4. Maak je Quiz-object aan (ik ga er vanuit dat je klasse 'QuizOpstellen' heet)
            QuizOpstellen nieuweQuiz = new QuizOpstellen()
            {
                // ID wordt later door de database ingevuld
                Omschrijving = omschrijving,
                VragenLijst = randomSelectie
            };

            // 5. Sla deze quiz op in de database via de repository
            // We verwachten het nieuwe gegenereerde QuizID terug te krijgen
            int gegenereerdQuizId = _repository.BewaarQuiz(nieuweQuiz);
            nieuweQuiz.Id = gegenereerdQuizId;

            // 6. Geef de quiz terug zodat de UI hem kan gebruiken
            return nieuweQuiz;
        }

        public void BeantwoordVraag(QuizOpstellen quiz, Vragen vraag, string gekozenLetter, int antwoordId)
        {
            // Zoek het antwoord object op
            var antwoordObj = vraag.Antwoorden.FirstOrDefault(a => a.AntwoordID == antwoordId);

            // Maak het resultaat object
            GegevenAntwoorden resultaat = new GegevenAntwoorden
            {
                AntwoordenID = antwoordId,
                GekozenLetter = gekozenLetter,
                IsCorrect = antwoordObj != null && antwoordObj.IsCorrect // Controleer direct in de data
            };

            // Sla op in de dictionary van de quiz
            quiz.IngevuldeAntwoorden[vraag] = resultaat;
        }
    }
}
