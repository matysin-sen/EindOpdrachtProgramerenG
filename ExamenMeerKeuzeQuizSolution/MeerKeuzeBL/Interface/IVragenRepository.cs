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

       

        public int voegUserToe(string naam, string achternaam);
       
        List<Vragen> GeefRandomVragenVoorOnderwerp(int onderwerpId, int aantalVragen);
        List<Vragen> GeefVragenPerOnderwerp(int onderwerpID);

        public int BewaarQuiz(QuizOpstellen quiz);
        public void BewaarAntwoorden(int quizId, Dictionary<Vragen, GegevenAntwoorden> antwoorden);
        public int BewaarGemaaktTest(int userId, int score);
        void BewaarUserTestAntwoorden(int gemaakteTestId, Dictionary<Vragen, GegevenAntwoorden> antwoorden, int quizId); 
        public List<GemaakteTest> GeefScoresVoorUser(int userId);

    }
}
