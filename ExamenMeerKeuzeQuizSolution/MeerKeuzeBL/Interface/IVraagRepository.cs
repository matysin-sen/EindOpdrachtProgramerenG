using MeerKeuzeBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeBL.Interface
{
    public interface IVraagRepository
    {
        public void VoegVraagToe(Vraag vraag);

        public List<Onderwerpen> GeefAlleOnderwerpen();
        public Onderwerpen VoegOnderwerpToe(string onderwerpNaam);

       

        public int voegUserToe(string naam, string achternaam);
       
        List<Vraag> GeefRandomVragenVoorOnderwerp(int onderwerpId, int aantalVragen);
        List<Vraag> GeefVragenPerOnderwerp(int onderwerpID);

        public int BewaarQuiz(QuizOpstellen quiz);
        public void BewaarAntwoorden(int quizId, Dictionary<Vraag, GegevenAntwoord> antwoorden);
        public int BewaarGemaaktTest(int userId, int score);
        void BewaarUserTestAntwoorden(int gemaakteTestId, Dictionary<Vraag, GegevenAntwoord> antwoorden, int quizId); 
        public List<GemaakteTest> GeefScoresVoorUser(int userId);

    }
}
