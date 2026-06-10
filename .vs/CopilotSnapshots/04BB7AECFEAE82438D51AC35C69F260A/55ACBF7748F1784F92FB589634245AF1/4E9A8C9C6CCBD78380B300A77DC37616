using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace MeerKeuzeBL.Tests
{
    // ==================== MANAGER TESTS ====================
    public class ManagerTests
    {
        private List<Vraag> MaakTestVragenLijst()
        {
            var antwoorden1 = new List<Antwoord>
            {
                new Antwoord(1, false, "Fout Antwoord 1"),
                new Antwoord(2, true,  "Juist Antwoord"),
                new Antwoord(3, false, "Fout Antwoord 2"),
                new Antwoord(4, false, "Fout Antwoord 3")
            };

            var antwoorden2 = new List<Antwoord>
            {
                new Antwoord(5, true,  "Juist Antwoord 2"),
                new Antwoord(6, false, "Fout Antwoord")
            };

            return new List<Vraag>
            {
                new Vraag(1, "Wat is 1+1?", antwoorden1, new List<Onderwerp>()),
                new Vraag(2, "Wat is de hoofdstad van België?", antwoorden2, new List<Onderwerp>()),
                new Vraag(3, "Extra Vraag", antwoorden1, new List<Onderwerp>())
            };
        }

        [Fact]
        public void GenereerRandomQuiz_MoetJuistAantalVragenBevatten()
        {
            var mockRepo = new Mock<IVraagRepository>();
            var onderwerp = new Onderwerp(1, "Algemeen");

            mockRepo.Setup(r => r.GeefRandomVragenVoorOnderwerp(1, 2))
                    .Returns(MaakTestVragenLijst().Take(2).ToList());
            mockRepo.Setup(r => r.BewaarQuiz(It.IsAny<QuizOpstellen>()))
                    .Returns(1);

            var manager = new Manager(mockRepo.Object);
            var quiz = manager.GenereerRandomQuiz(onderwerp, 2, "Test Quiz");

            Assert.NotNull(quiz);
            Assert.Equal("Test Quiz", quiz.Omschrijving);
            Assert.Equal(2, quiz.VragenLijst.Count);
            Assert.Equal(1, quiz.QuizOnderwerp.OnderwerpID);
        }

        [Fact]
        public void GenereerRandomQuiz_MoetExceptionGooienAlsNietGenoegVragen()
        {
            var mockRepo = new Mock<IVraagRepository>();

            mockRepo.Setup(r => r.GeefRandomVragenVoorOnderwerp(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(new List<Vraag>
                    {
                        new Vraag(1, "Vraag 1", new List<Antwoord>(), new List<Onderwerp>())
                    });

            var manager = new Manager(mockRepo.Object);
            var onderwerp = new Onderwerp(1, "Wiskunde");

            Assert.Throws<Exception>(() => manager.GenereerRandomQuiz(onderwerp, 5, "Test Quiz"));
        }

        [Fact]
        public void BeantwoordVraag_JuistAntwoord_IsCorrectIsTrue()
        {
            var mockRepo = new Mock<IVraagRepository>();
            var manager = new Manager(mockRepo.Object);

            var vraag = MaakTestVragenLijst().First();
            var quiz = new QuizOpstellen(new Onderwerp(1, "Test"), new List<Vraag> { vraag }, "Test");

            manager.BeantwoordVraag(quiz, vraag, "B", 2); // ID 2 = juist antwoord

            Assert.True(quiz.IngevuldeAntwoorden.ContainsKey(vraag));
            Assert.True(quiz.IngevuldeAntwoorden[vraag].IsCorrect);
            Assert.Equal("B", quiz.IngevuldeAntwoorden[vraag].GekozenLetter);
        }

        [Fact]
        public void BeantwoordVraag_FoutAntwoord_IsCorrectIsFalse()
        {
            var mockRepo = new Mock<IVraagRepository>();
            var manager = new Manager(mockRepo.Object);

            var vraag = MaakTestVragenLijst().First();
            var quiz = new QuizOpstellen(new Onderwerp(1, "Test"), new List<Vraag> { vraag }, "Test");

            manager.BeantwoordVraag(quiz, vraag, "A", 1); // ID 1 = fout antwoord

            Assert.True(quiz.IngevuldeAntwoorden.ContainsKey(vraag));
            Assert.False(quiz.IngevuldeAntwoorden[vraag].IsCorrect);
            Assert.Equal("A", quiz.IngevuldeAntwoorden[vraag].GekozenLetter);
        }

        [Fact]
        public void GeefAlleOnderwerpen_RoeptRepositoryAan()
        {
            var mockRepo = new Mock<IVraagRepository>();
            var verwachteOnderwerpen = new List<Onderwerp>
            {
                new Onderwerp(1, "C#"),
                new Onderwerp(2, "SQL")
            };

            mockRepo.Setup(r => r.GeefAlleOnderwerpen()).Returns(verwachteOnderwerpen);
            var manager = new Manager(mockRepo.Object);

            var resultaat = manager.GeefAlleOnderwerpen();

            Assert.Equal(2, resultaat.Count);
            Assert.Equal("C#", resultaat[0].OnderwerpNaam);
            mockRepo.Verify(r => r.GeefAlleOnderwerpen(), Times.Once);
        }
    }

    // ==================== IMPORTMANAGER TESTS ====================
    public class ImportManagerTests
    {
        [Fact]
        public void ImporteerBestand_RoeptVoegVraagToeAanVoorElkeVraag()
        {
            // Arrange
            var mockRepo = new Mock<IVraagRepository>();
            var mockReader = new Mock<IFileReader>();
            var onderwerp = new Onderwerp(1, "SQL");

            var nepVragen = new List<Vraag>
            {
                new Vraag(0, "Vraag 1", new List<Antwoord>(), new List<Onderwerp>()),
                new Vraag(0, "Vraag 2", new List<Antwoord>(), new List<Onderwerp>())
            };

            mockReader.Setup(r => r.Read(It.IsAny<string>())).Returns(nepVragen);

            var importManager = new ImportManager(mockRepo.Object, mockReader.Object);

            // Act
            importManager.ImporteerBestand("test.txt", mockReader.Object, onderwerp);

            // Assert: VoegVraagToe moet 2x aangeroepen zijn
            mockRepo.Verify(r => r.VoegVraagToe(It.IsAny<Vraag>()), Times.Exactly(2));
        }

        [Fact]
        public void ImporteerBestand_KoppeltOnderwerpAanElkeVraag()
        {
            // Arrange
            var mockRepo = new Mock<IVraagRepository>();
            var mockReader = new Mock<IFileReader>();
            var onderwerp = new Onderwerp(1, "SQL");

            var nepVragen = new List<Vraag>
            {
                new Vraag(0, "Vraag 1", new List<Antwoord>(), new List<Onderwerp>())
            };

            mockReader.Setup(r => r.Read(It.IsAny<string>())).Returns(nepVragen);

            var importManager = new ImportManager(mockRepo.Object, mockReader.Object);

            // Act
            importManager.ImporteerBestand("test.txt", mockReader.Object, onderwerp);

            // Assert: het onderwerp moet gekoppeld zijn aan de vraag
            Assert.Equal(onderwerp, nepVragen[0].Onderwerp[0]);
        }



        // ==================== DOMEINKLASSE TESTS ====================
        public class DomeinTests
        {
            [Fact]
            public void Vragen_MagNietLeegZijn()
            {
                Assert.Throws<ArgumentException>(() => new Vraag(1, "", new List<Antwoord>(), new List<Onderwerp>()));
            }

            [Fact]
            public void Vragen_MagNietNullZijn()
            {
                Assert.Throws<ArgumentException>(() => new Vraag(1, null, new List<Antwoord>(), new List<Onderwerp>()));
            }

            [Fact]
            public void Antwoorden_IsCorrect_WordtCorrectOpgeslagen()
            {
                var antwoord = new Antwoord(1, true, "Juist antwoord");
                Assert.True(antwoord.IsCorrect);
            }

            [Fact]
            public void Antwoorden_IsNotCorrect_WordtCorrectOpgeslagen()
            {
                var antwoord = new Antwoord(1, false, "Fout antwoord");
                Assert.False(antwoord.IsCorrect);
            }

            [Fact]
            public void Onderwerpen_NaamWordtCorrectOpgeslagen()
            {
                var onderwerp = new Onderwerp(1, "Wiskunde");
                Assert.Equal("Wiskunde", onderwerp.OnderwerpNaam);
                Assert.Equal(1, onderwerp.OnderwerpID);
            }

            [Fact]
            public void QuizOpstellen_BeginScoreIsNul()
            {
                var quiz = new QuizOpstellen(new Onderwerp(1, "Test"), new List<Vraag>(), "Test Quiz");
                Assert.Equal(0, quiz.Score);
            }

            [Fact]
            public void QuizOpstellen_IngevuldeAntwoordenIsLeegBijAanmaak()
            {
                var quiz = new QuizOpstellen(new Onderwerp(1, "Test"), new List<Vraag>(), "Test Quiz");
                Assert.NotNull(quiz.IngevuldeAntwoorden);
                Assert.Empty(quiz.IngevuldeAntwoorden);
            }
        }
    }
}