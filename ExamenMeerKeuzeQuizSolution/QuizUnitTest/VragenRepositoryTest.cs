using MeerKeuzeBL.Domein;
using MeerKeuzeDL.Repository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace QuizIntegratieTest
{
    public class VragenRepositoryTests : IDisposable
    {
        private readonly string _connectionString = "Data Source=DESKTOP-TM1M5LB\\SQLEXPRESS;Initial Catalog=Quiz;Integrated Security=True;Trust Server Certificate=True";
        private readonly VragenRepository _repository;

        private readonly List<int> _aangemaakteVraagIds = new List<int>();
        private readonly List<int> _aangemaakteOnderwerpIds = new List<int>();
        private readonly List<int> _aangemaakteUserIds = new List<int>();
        private readonly List<int> _aangemaakteQuizIds = new List<int>();
        private readonly List<int> _aangemaakteGemaakteTestIds = new List<int>();

        public VragenRepositoryTests()
        {
            _repository = new VragenRepository(_connectionString);
        }

        // ==================== ONDERWERP TESTS ====================

        [Fact]
        public void VoegOnderwerpToe_MoetOnderwerpOpslaanEnIDTeruggeven()
        {
            // Act
            Onderwerpen resultaat = _repository.VoegOnderwerpToe("TestOnderwerp_" + Guid.NewGuid());
            _aangemaakteOnderwerpIds.Add(resultaat.OnderwerpID);

            // Assert
            Assert.NotNull(resultaat);
            Assert.True(resultaat.OnderwerpID > 0);
        }

        [Fact]
        public void VoegOnderwerpToe_MoetNaamCorrectOpslaanInDB()
        {
            // Arrange
            string uniekNaam = "TestOnderwerp_" + Guid.NewGuid();

            // Act
            Onderwerpen resultaat = _repository.VoegOnderwerpToe(uniekNaam);
            _aangemaakteOnderwerpIds.Add(resultaat.OnderwerpID);

            // Assert
            Assert.Equal(uniekNaam, resultaat.OnderwerpNaam);
        }

        [Fact]
        public void GeefAlleOnderwerpen_MoetLijstTeruggeven()
        {
            // Act
            var resultaat = _repository.GeefAlleOnderwerpen();

            // Assert
            Assert.NotNull(resultaat);
            Assert.True(resultaat.Count > 0);
        }

        [Fact]
        public void GeefAlleOnderwerpen_MoetAlfabetischGesorteerdZijn()
        {
            // Act
            var resultaat = _repository.GeefAlleOnderwerpen();

            // Assert
            var gesorteerd = resultaat.OrderBy(o => o.OnderwerpNaam).ToList();
            Assert.Equal(
                gesorteerd.Select(o => o.OnderwerpNaam),
                resultaat.Select(o => o.OnderwerpNaam)
            );
        }

        [Fact]
        public void GeefAlleOnderwerpen_NieuwToegevoegdOnderwerpMoetInLijstStaan()
        {
            // Arrange
            string uniekNaam = "TestOnderwerp_" + Guid.NewGuid();
            Onderwerpen nieuw = _repository.VoegOnderwerpToe(uniekNaam);
            _aangemaakteOnderwerpIds.Add(nieuw.OnderwerpID);

            // Act
            var resultaat = _repository.GeefAlleOnderwerpen();

            // Assert
            Assert.Contains(resultaat, o => o.OnderwerpNaam == uniekNaam);
        }

        // ==================== USER TESTS ====================

        [Fact]
        public void VoegUserToe_MoetUserOpslaanEnIDTeruggeven()
        {
            // Act
            int userId = _repository.voegUserToe("TestVoornaam", "TestAchternaam");
            _aangemaakteUserIds.Add(userId);

            // Assert
            Assert.True(userId > 0);
        }

        [Fact]
        public void VoegUserToe_MeerdereUsers_MoetenVerschillendeIDsHebben()
        {
            // Act
            int userId1 = _repository.voegUserToe("Voornaam1", "Achternaam1");
            int userId2 = _repository.voegUserToe("Voornaam2", "Achternaam2");
            _aangemaakteUserIds.Add(userId1);
            _aangemaakteUserIds.Add(userId2);

            // Assert
            Assert.NotEqual(userId1, userId2);
        }

        // ==================== VRAAG TESTS ====================

        [Fact]
        public void VoegVraagToe_MoetVraagMetAntwoordenOpslaanInDB()
        {
            // Arrange
            Onderwerpen onderwerp = _repository.VoegOnderwerpToe("TestOnderwerp_" + Guid.NewGuid());
            _aangemaakteOnderwerpIds.Add(onderwerp.OnderwerpID);

            var antwoorden = new List<Antwoorden>
            {
                new Antwoorden(false, "Fout 1"),
                new Antwoorden(true,  "Juist"),
                new Antwoorden(false, "Fout 2"),
                new Antwoorden(false, "Fout 3")
            };

            var vraag = new Vragen
            {
                VraagTekst = "TestVraag_" + Guid.NewGuid(),
                Antwoorden = antwoorden,
                Onderwerp = new List<Onderwerpen> { onderwerp }
            };

            // Act — geen exception = geslaagd
            _repository.VoegVraagToe(vraag);
        }

        // ==================== RANDOM VRAGEN TESTS ====================

        [Fact]
        public void GeefRandomVragenVoorOnderwerp_MoetJuistAantalTeruggeven()
        {
            // Arrange
            var onderwerpen = _repository.GeefAlleOnderwerpen();
            Assert.True(onderwerpen.Count > 0, "Geen onderwerpen in DB om te testen.");
            var onderwerp = onderwerpen.First();

            // Act
            var vragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerp.OnderwerpID, 2);

            // Assert
            Assert.NotNull(vragen);
            Assert.True(vragen.Count <= 2);
        }

        [Fact]
        public void GeefRandomVragenVoorOnderwerp_VragenMoetenAntwoordenBevatten()
        {
            // Arrange
            var onderwerpen = _repository.GeefAlleOnderwerpen();
            var onderwerp = onderwerpen.First();

            // Act
            var vragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerp.OnderwerpID, 2);

            // Assert
            foreach (var vraag in vragen)
            {
                Assert.NotNull(vraag.Antwoorden);
                Assert.True(vraag.Antwoorden.Count > 0);
            }
        }

        [Fact]
        public void GeefRandomVragenVoorOnderwerp_AntwoordenMoetenIDsBevatten()
        {
            // Arrange
            var onderwerpen = _repository.GeefAlleOnderwerpen();
            var onderwerp = onderwerpen.First();

            // Act
            var vragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerp.OnderwerpID, 2);

            // Assert: elk antwoord moet een geldig ID hebben
            foreach (var vraag in vragen)
            {
                foreach (var antwoord in vraag.Antwoorden)
                {
                    Assert.True(antwoord.AntwoordID > 0);
                }
            }
        }

        // ==================== QUIZ TESTS ====================

        [Fact]
        public void BewaarQuiz_MoetQuizOpslaanEnIDTeruggeven()
        {
            // Arrange
            var onderwerpen = _repository.GeefAlleOnderwerpen();
            var vragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerpen.First().OnderwerpID, 2);

            var quiz = new QuizOpstellen
            {
                Omschrijving = "TestQuiz_" + Guid.NewGuid(),
                VragenLijst = vragen
            };

            // Act
            int quizId = _repository.BewaarQuiz(quiz);
            _aangemaakteQuizIds.Add(quizId);

            // Assert
            Assert.True(quizId > 0);
        }

        [Fact]
        public void BewaarQuiz_MeerdereQuizzen_MoetenVerschillendeIDsHebben()
        {
            // Arrange
            var onderwerpen = _repository.GeefAlleOnderwerpen();
            var vragen = _repository.GeefRandomVragenVoorOnderwerp(onderwerpen.First().OnderwerpID, 2);

            var quiz1 = new QuizOpstellen { Omschrijving = "Quiz1_" + Guid.NewGuid(), VragenLijst = vragen };
            var quiz2 = new QuizOpstellen { Omschrijving = "Quiz2_" + Guid.NewGuid(), VragenLijst = vragen };

            // Act
            int quizId1 = _repository.BewaarQuiz(quiz1);
            int quizId2 = _repository.BewaarQuiz(quiz2);
            _aangemaakteQuizIds.Add(quizId1);
            _aangemaakteQuizIds.Add(quizId2);

            // Assert
            Assert.NotEqual(quizId1, quizId2);
        }

        // ==================== GEMAAKTETESTEN TESTS ====================

        [Fact]
        public void BewaarGemaaktTest_MoetOpslaanEnIDTeruggeven()
        {
            // Arrange
            int userId = _repository.voegUserToe("TestVoornaam", "TestAchternaam");
            _aangemaakteUserIds.Add(userId);

            // Act
            int gemaakteTestId = _repository.BewaarGemaaktTest(userId, 5);
            _aangemaakteGemaakteTestIds.Add(gemaakteTestId);

            // Assert
            Assert.True(gemaakteTestId > 0);
        }

        [Fact]
        public void GeefScoresVoorUser_MoetScoresTeruggeven()
        {
            // Arrange
            int userId = _repository.voegUserToe("TestVoornaam", "TestAchternaam");
            _aangemaakteUserIds.Add(userId);

            int gemaakteTestId = _repository.BewaarGemaaktTest(userId, 7);
            _aangemaakteGemaakteTestIds.Add(gemaakteTestId);

            // Act
            var scores = _repository.GeefScoresVoorUser(userId);

            // Assert
            Assert.NotNull(scores);
            Assert.True(scores.Count > 0);
            Assert.Equal(7, Convert.ToInt32(scores.First().Score));
        }

        [Fact]
        public void GeefScoresVoorUser_MoetAlleenScoresVanDezeUserTeruggeven()
        {
            // Arrange
            int userId1 = _repository.voegUserToe("User1", "Achternaam1");
            int userId2 = _repository.voegUserToe("User2", "Achternaam2");
            _aangemaakteUserIds.Add(userId1);
            _aangemaakteUserIds.Add(userId2);

            int testId1 = _repository.BewaarGemaaktTest(userId1, 8);
            int testId2 = _repository.BewaarGemaaktTest(userId2, 3);
            _aangemaakteGemaakteTestIds.Add(testId1);
            _aangemaakteGemaakteTestIds.Add(testId2);

            // Act
            var scoresUser1 = _repository.GeefScoresVoorUser(userId1);

            // Assert: alleen scores van user1, niet van user2
            Assert.All(scoresUser1, s => Assert.Equal(userId1, s.UserID));
        }

        // ==================== OPRUIMEN ====================

        public void Dispose()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (int id in _aangemaakteGemaakteTestIds)
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM GEMAAKTETESTEN WHERE IDGemaakteTest = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (int id in _aangemaakteQuizIds)
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM TESTANTWOORDEN WHERE TestVraagID IN (SELECT IDTestVraag FROM TESTVRAGEN WHERE TestID = @ID)", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM TESTVRAGEN WHERE TestID = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM TESTEN WHERE IDTest = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (int id in _aangemaakteUserIds)
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM USERS WHERE IDUser = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (int id in _aangemaakteOnderwerpIds)
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM VRAGEN_ONDERWERPEN WHERE OnderwerpID = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM ONDERWERPEN WHERE IDOnderwerp = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}