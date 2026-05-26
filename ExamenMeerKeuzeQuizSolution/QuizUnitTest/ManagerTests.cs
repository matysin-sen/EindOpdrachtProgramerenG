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
        private List<Vragen> MaakTestVragenLijst()
        {
            var antwoorden1 = new List<Antwoorden>
            {
                new Antwoorden(1, false, "Fout Antwoord 1"),
                new Antwoorden(2, true,  "Juist Antwoord"),
                new Antwoorden(3, false, "Fout Antwoord 2"),
                new Antwoorden(4, false, "Fout Antwoord 3")
            };

            var antwoorden2 = new List<Antwoorden>
            {
                new Antwoorden(5, true,  "Juist Antwoord 2"),
                new Antwoorden(6, false, "Fout Antwoord")
            };

            return new List<Vragen>
            {
                new Vragen(1, "Wat is 1+1?", antwoorden1, new List<Onderwerpen>()),
                new Vragen(2, "Wat is de hoofdstad van België?", antwoorden2, new List<Onderwerpen>()),
                new Vragen(3, "Extra Vraag", antwoorden1, new List<Onderwerpen>())
            };
        }

        [Fact]
        public void GenereerRandomQuiz_MoetJuistAantalVragenBevatten()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var onderwerp = new Onderwerpen(1, "Algemeen");

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
            var mockRepo = new Mock<IVragenRepository>();

            mockRepo.Setup(r => r.GeefRandomVragenVoorOnderwerp(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(new List<Vragen>
                    {
                        new Vragen(1, "Vraag 1", new List<Antwoorden>(), new List<Onderwerpen>())
                    });

            var manager = new Manager(mockRepo.Object);
            var onderwerp = new Onderwerpen(1, "Wiskunde");

            Assert.Throws<Exception>(() => manager.GenereerRandomQuiz(onderwerp, 5, "Test Quiz"));
        }

        [Fact]
        public void BeantwoordVraag_JuistAntwoord_IsCorrectIsTrue()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var manager = new Manager(mockRepo.Object);

            var vraag = MaakTestVragenLijst().First();
            var quiz = new QuizOpstellen(new Onderwerpen(1, "Test"), new List<Vragen> { vraag }, "Test");

            manager.BeantwoordVraag(quiz, vraag, "B", 2); // ID 2 = juist antwoord

            Assert.True(quiz.IngevuldeAntwoorden.ContainsKey(vraag));
            Assert.True(quiz.IngevuldeAntwoorden[vraag].IsCorrect);
            Assert.Equal("B", quiz.IngevuldeAntwoorden[vraag].GekozenLetter);
        }

        [Fact]
        public void BeantwoordVraag_FoutAntwoord_IsCorrectIsFalse()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var manager = new Manager(mockRepo.Object);

            var vraag = MaakTestVragenLijst().First();
            var quiz = new QuizOpstellen(new Onderwerpen(1, "Test"), new List<Vragen> { vraag }, "Test");

            manager.BeantwoordVraag(quiz, vraag, "A", 1); // ID 1 = fout antwoord

            Assert.True(quiz.IngevuldeAntwoorden.ContainsKey(vraag));
            Assert.False(quiz.IngevuldeAntwoorden[vraag].IsCorrect);
            Assert.Equal("A", quiz.IngevuldeAntwoorden[vraag].GekozenLetter);
        }

        [Fact]
        public void GeefAlleOnderwerpen_RoeptRepositoryAan()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var verwachteOnderwerpen = new List<Onderwerpen>
            {
                new Onderwerpen(1, "C#"),
                new Onderwerpen(2, "SQL")
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
            var mockRepo = new Mock<IVragenRepository>();
            var mockReader = new Mock<IFileReader>();
            var onderwerp = new Onderwerpen(1, "SQL");

            var nepVragen = new List<Vragen>
            {
                new Vragen(0, "Vraag 1", new List<Antwoorden>(), new List<Onderwerpen>()),
                new Vragen(0, "Vraag 2", new List<Antwoorden>(), new List<Onderwerpen>())
            };

            mockReader.Setup(r => r.Read(It.IsAny<string>())).Returns(nepVragen);

            var importManager = new ImportManager(mockRepo.Object, mockReader.Object);

            // Act
            importManager.ImporteerBestand("test.txt", mockReader.Object, onderwerp);

            // Assert: VoegVraagToe moet 2x aangeroepen zijn
            mockRepo.Verify(r => r.VoegVraagToe(It.IsAny<Vragen>()), Times.Exactly(2));
        }

        [Fact]
        public void ImporteerBestand_KoppeltOnderwerpAanElkeVraag()
        {
            // Arrange
            var mockRepo = new Mock<IVragenRepository>();
            var mockReader = new Mock<IFileReader>();
            var onderwerp = new Onderwerpen(1, "SQL");

            var nepVragen = new List<Vragen>
            {
                new Vragen(0, "Vraag 1", new List<Antwoorden>(), new List<Onderwerpen>())
            };

            mockReader.Setup(r => r.Read(It.IsAny<string>())).Returns(nepVragen);

            var importManager = new ImportManager(mockRepo.Object, mockReader.Object);

            // Act
            importManager.ImporteerBestand("test.txt", mockReader.Object, onderwerp);

            // Assert: het onderwerp moet gekoppeld zijn aan de vraag
            Assert.Equal(onderwerp, nepVragen[0].Onderwerp[0]);
        }

        [Fact]
        public void BepaalOnderwerpViaBestand_GeoBestand_GeeftAardrijkskundeTerug()
        {
            // Arrange
            var mockRepo = new Mock<IVragenRepository>();
            var importManager = new ImportManager(mockRepo.Object, null);

            var onderwerpen = new List<Onderwerpen>
            {
                new Onderwerpen(1, "Aardrijkskunde"),
                new Onderwerpen(2, "Muziek"),
                new Onderwerpen(3, "SQL")
            };

            // Act
            var resultaat = importManager.BepaalOnderwerpViaBestand("C:\\bestanden\\Geo1.txt", onderwerpen);

            // Assert
            Assert.NotNull(resultaat);
            Assert.Equal("Aardrijkskunde", resultaat.OnderwerpNaam);
        }

        [Fact]
        public void BepaalOnderwerpViaBestand_MuziekBestand_GeeftMuziekTerug()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var importManager = new ImportManager(mockRepo.Object, null);

            var onderwerpen = new List<Onderwerpen>
            {
                new Onderwerpen(1, "Aardrijkskunde"),
                new Onderwerpen(2, "Muziek"),
                new Onderwerpen(3, "SQL")
            };

            var resultaat = importManager.BepaalOnderwerpViaBestand("C:\\bestanden\\Muziek80s1.txt", onderwerpen);

            Assert.NotNull(resultaat);
            Assert.Equal("Muziek", resultaat.OnderwerpNaam);
        }

        [Fact]
        public void BepaalOnderwerpViaBestand_SQLBestand_GeeftSQLTerug()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var importManager = new ImportManager(mockRepo.Object, null);

            var onderwerpen = new List<Onderwerpen>
            {
                new Onderwerpen(1, "Aardrijkskunde"),
                new Onderwerpen(2, "Muziek"),
                new Onderwerpen(3, "SQL")
            };

            var resultaat = importManager.BepaalOnderwerpViaBestand("C:\\bestanden\\SQL_Beg.txt", onderwerpen);

            Assert.NotNull(resultaat);
            Assert.Equal("SQL", resultaat.OnderwerpNaam);
        }

        [Fact]
        public void BepaalOnderwerpViaBestand_OnbekendBestand_GeeftAlgemeenTerug()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var importManager = new ImportManager(mockRepo.Object, null);

            var onderwerpen = new List<Onderwerpen>
            {
                new Onderwerpen(1, "Aardrijkskunde"),
                new Onderwerpen(2, "algemeen")
            };

            var resultaat = importManager.BepaalOnderwerpViaBestand("C:\\bestanden\\onbekend.txt", onderwerpen);

            Assert.NotNull(resultaat);
            Assert.Equal("algemeen", resultaat.OnderwerpNaam);
        }

        [Fact]
        public void BepaalOnderwerpViaBestand_OnderwerpNietInDB_GeeftNullTerug()
        {
            var mockRepo = new Mock<IVragenRepository>();
            var importManager = new ImportManager(mockRepo.Object, null);

            // Lege lijst: onderwerp bestaat niet in DB
            var onderwerpen = new List<Onderwerpen>();

            var resultaat = importManager.BepaalOnderwerpViaBestand("C:\\bestanden\\Geo1.txt", onderwerpen);

            Assert.Null(resultaat);
        }
    }

    // ==================== DOMEINKLASSE TESTS ====================
    public class DomeinTests
    {
        [Fact]
        public void Vragen_MagNietLeegZijn()
        {
            Assert.Throws<ArgumentException>(() => new Vragen(1, "", new List<Antwoorden>(), new List<Onderwerpen>()));
        }

        [Fact]
        public void Vragen_MagNietNullZijn()
        {
            Assert.Throws<ArgumentException>(() => new Vragen(1, null, new List<Antwoorden>(), new List<Onderwerpen>()));
        }

        [Fact]
        public void Antwoorden_IsCorrect_WordtCorrectOpgeslagen()
        {
            var antwoord = new Antwoorden(1, true, "Juist antwoord");
            Assert.True(antwoord.IsCorrect);
        }

        [Fact]
        public void Antwoorden_IsNotCorrect_WordtCorrectOpgeslagen()
        {
            var antwoord = new Antwoorden(1, false, "Fout antwoord");
            Assert.False(antwoord.IsCorrect);
        }

        [Fact]
        public void Onderwerpen_NaamWordtCorrectOpgeslagen()
        {
            var onderwerp = new Onderwerpen(1, "Wiskunde");
            Assert.Equal("Wiskunde", onderwerp.OnderwerpNaam);
            Assert.Equal(1, onderwerp.OnderwerpID);
        }

        [Fact]
        public void QuizOpstellen_BeginScoreIsNul()
        {
            var quiz = new QuizOpstellen(new Onderwerpen(1, "Test"), new List<Vragen>(), "Test Quiz");
            Assert.Equal(0, quiz.Score);
        }

        [Fact]
        public void QuizOpstellen_IngevuldeAntwoordenIsLeegBijAanmaak()
        {
            var quiz = new QuizOpstellen(new Onderwerpen(1, "Test"), new List<Vragen>(), "Test Quiz");
            Assert.NotNull(quiz.IngevuldeAntwoorden);
            Assert.Empty(quiz.IngevuldeAntwoorden);
        }
    }
}