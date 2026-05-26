using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace MeerKeuzeDL.Repository
{

        public class VragenRepository : IVragenRepository
    {
            private  string _connectionString;

            // Geef de connection string mee via de constructor
            public VragenRepository(string connectionString)
            {
                _connectionString = connectionString;
            }
         

        public void VoegVraagToe(Vragen vraag)
            {
            // OUTPUT INSERTED.VraagID geeft meteen het nieuwe ID terug dat de database heeft verzonnen
            string sqlVraag = "INSERT INTO VRAGEN (Vraagzin) OUTPUT INSERTED.IDVraag VALUES (@Vraagzin)";
            string sqlAntwoord = "INSERT INTO ANTWOORDEN (VraagID, AntwoordZin, IsCorrect) VALUES (@VraagID, @Antwoordzin, @IsCorrect)";
            // --- NIEUW: Query voor de tussentabel ---
            string sqlTussentabel = "INSERT INTO VRAGEN_ONDERWERPEN (VraagID, OnderwerpID) VALUES (@VraagID, @OnderwerpID)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Start een transactie
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int nieuwVraagID;

                            // 1. Sla de vraag op
                            using (SqlCommand cmdVraag = new SqlCommand(sqlVraag, conn, transaction))
                            {
                                cmdVraag.Parameters.AddWithValue("@Vraagzin", vraag.VraagTekst);

                                // ExecuteScalar voert de query uit en geeft het resultaat van OUTPUT INSERTED.VraagID terug
                                nieuwVraagID = (int)cmdVraag.ExecuteScalar();
                            }

                            // 2. Sla de antwoorden op
                            if (vraag.Antwoorden != null)
                            {
                                foreach (var antwoord in vraag.Antwoorden)
                                {
                                    using (SqlCommand cmdAntwoord = new SqlCommand(sqlAntwoord, conn, transaction))
                                    {
                                        // Koppel het zojuist gekregen VraagID aan het antwoord
                                        cmdAntwoord.Parameters.AddWithValue("@VraagID", nieuwVraagID);
                                        cmdAntwoord.Parameters.AddWithValue("@AntwoordZin", antwoord.AntwoordTekst);
                                        cmdAntwoord.Parameters.AddWithValue("@IsCorrect", antwoord.IsCorrect);

                                        cmdAntwoord.ExecuteNonQuery();
                                    }
                                }
                            }
                        // --- 3. NIEUW: Sla de koppeling met onderwerpen op in de tussentabel ---
                        if (vraag.Onderwerp != null)
                        {
                            foreach (var onderwerp in vraag.Onderwerp)
                            {
                                using (SqlCommand cmdTussen = new SqlCommand(sqlTussentabel, conn, transaction))
                                {
                                    cmdTussen.Parameters.AddWithValue("@VraagID", nieuwVraagID);
                                    cmdTussen.Parameters.AddWithValue("@OnderwerpID", onderwerp.OnderwerpID); // Zorg dat dit klopt met de property naam in je klasse Onderwerpen!

                                    cmdTussen.ExecuteNonQuery();
                                }
                            }
                        }
                        // Alles is goed gegaan, sla de wijzigingen definitief op in de databank
                        transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Er is iets misgegaan, draai alle aanpassingen van deze methode terug
                            transaction.Rollback();
                            throw new Exception("Fout bij het wegschrijven naar de databank: " + ex.Message);
                        }
                    }
                }
            }
        public Onderwerpen VoegOnderwerpToe(string onderwerpNaam)
        {
            int id;
            Onderwerpen onderwerp = null;

            // De query is aangepast naar jouw tabelnaam en kolomnamen uit SQLQueryQuizMaken.sql
            const string query = "INSERT INTO ONDERWERPEN (Onderwerpnaam) OUTPUT INSERTED.IDOnderwerp VALUES (@Onderwerpnaam)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Onderwerpnaam", onderwerpNaam);

                conn.Open();
                try
                {
                    // ExecuteScalar voert de INSERT uit en pakt de OUTPUT INSERTED.IDOnderwerp direct vast
                    id = (int)cmd.ExecuteScalar();

                    // We maken een nieuw object van jouw Domeinklasse
                    onderwerp = new Onderwerpen(id, onderwerpNaam);
                }
                catch (Exception ex)
                {
                    // Fouten netjes opvangen en doorgeven naar de UI zodat je weet wát er misging
                    throw new Exception("Fout bij het toevoegen van het onderwerp: " + ex.Message);
                }
            }

            return onderwerp;
        }
        public List<Onderwerpen> GeefAlleOnderwerpen()
        {
            List<Onderwerpen> onderwerpenLijst = new List<Onderwerpen>();

            // We selecteren beide kolommen uit de tabel en sorteren ze alfabetisch
            string query = "SELECT IDOnderwerp, Onderwerpnaam FROM ONDERWERPEN ORDER BY Onderwerpnaam ASC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();

                        // Gebruik een SqlDataReader om meerdere rijen uit te lezen
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Haal de data uit de huidige rij
                                int id = (int)reader["IDOnderwerp"];
                                string naam = reader["Onderwerpnaam"].ToString();

                                // Maak een nieuw C# object aan en voeg toe aan de lijst
                                Onderwerpen onderwerp = new Onderwerpen(id, naam);
                                onderwerpenLijst.Add(onderwerp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Fout bij het ophalen van de onderwerpen: " + ex.Message);
                    }
                }
            }

            return onderwerpenLijst;
        }

        public int voegUserToe(string naam,string achternaam)
        {
            // 1. Schrijf de SQL query (Pas 'GEBRUIKERS', 'Naam' en 'Achternaam' aan naar jouw echte tabel- en kolomnamen)
            string query = "INSERT INTO USERS (Naam, Voornaam) OUTPUT INSERTED.IDUser VALUES (@Naam, @Voornaam)";

            // 2. Maak verbinding met de databank
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // 3. Maak het SQL commando klaar
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 4. Koppel de parameters aan de veilige @-variabelen om SQL-injectie te voorkomen
                    cmd.Parameters.AddWithValue("@Naam", achternaam);
                    cmd.Parameters.AddWithValue("@Voornaam", naam);

                    try
                    {
                        // 5. Open de verbinding en voer de query uit
                        conn.Open();

                        // ExecuteNonQuery gebruiken we voor INSERT, UPDATE of DELETE (als we geen ID terug hoeven)
                        return (int)cmd.ExecuteScalar(); // ← ID teruggeven
                    }
                    catch (Exception ex)
                    {
                        // Fouten opvangen en duidelijk doorgeven
                        throw new Exception("Fout bij het toevoegen van de user: " + ex.Message);
                    }
                }
            }
        }

        public List<Vragen> GeefRandomVragenVoorOnderwerp(int onderwerpId, int aantalVragen)
        {
            List<Vragen> quizVragen = new List<Vragen>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // 1. Haal random vragen op via de tussentabel (ORDER BY NEWID() zorgt voor randomisatie)
                string sqlVragen = @"
            SELECT TOP (@Aantal) v.IDVraag, v.Vraagzin 
            FROM VRAGEN v
            INNER JOIN VRAGEN_ONDERWERPEN vo ON v.IDVraag = vo.VraagID
            WHERE vo.OnderwerpID = @OnderwerpID
            ORDER BY NEWID()";

                using (SqlCommand cmd = new SqlCommand(sqlVragen, conn))
                {
                    cmd.Parameters.AddWithValue("@Aantal", aantalVragen);
                    cmd.Parameters.AddWithValue("@OnderwerpID", onderwerpId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int vraagId = Convert.ToInt32(reader["IDVraag"]);
                            string vraagZin = reader["Vraagzin"].ToString();

                            // Maak de vraag alvast aan
                            Vragen vraag = new Vragen(vraagId, vraagZin, new List<Antwoorden>(), new List<Onderwerpen>());
                            quizVragen.Add(vraag);
                        }
                    }
                }

                // 2. We hebben nu de vragen, maar we moeten ook hun antwoorden ophalen!
                string sqlAntwoorden = "SELECT IDAntwoord, AntwoordZin, IsCorrect FROM ANTWOORDEN WHERE VraagID = @VraagID";

                foreach (var vraag in quizVragen)
                {
                    using (SqlCommand cmdAntw = new SqlCommand(sqlAntwoorden, conn))
                    {
                        cmdAntw.Parameters.AddWithValue("@VraagID", vraag.VraagID);
                        using (SqlDataReader readerAntw = cmdAntw.ExecuteReader())
                        {
                            while (readerAntw.Read())
                            {
                                int antwoordId = Convert.ToInt32(readerAntw["IDAntwoord"]); // ← toevoegen
                                string antwZin = readerAntw["AntwoordZin"].ToString();
                                bool isCorrect = Convert.ToBoolean(readerAntw["IsCorrect"]);

                                vraag.Antwoorden.Add(new Antwoorden(antwoordId, isCorrect, antwZin)); // ← constructor met ID
                            }
                        }
                    }
                }
            }

            // Geef de complete lijst (vragen mét antwoorden) terug aan je Manager
            return quizVragen;
        }

        public List<Vragen> GeefVragenPerOnderwerp(int onderwerpId)
        {
            List<Vragen> vragen = new List<Vragen>();

            // De SQL query die gebruik maakt van jouw tabelnamen
            string query = @"
        SELECT v.VraagID, v.VraagZin, 
               a.IDAntwoord, a.AntwoordZin, a.IsCorrect
        FROM VRAGEN v
        INNER JOIN VRAGEN_ONDERWERPEN vo ON v.VraagID = vo.VraagID
        LEFT JOIN ANTWOORDEN a ON v.VraagID = a.VraagID
        WHERE vo.OnderwerpID = @OnderwerpID";

            // Zorg dat 'connectionString' de naam is van de variabele die jouw connectiestring bevat
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OnderwerpID", onderwerpId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Dictionary om vragen te groeperen op hun VraagID
                    Dictionary<int, Vragen> vraagDict = new Dictionary<int, Vragen>();

                    while (reader.Read())
                    {
                        int vraagId = (int)reader["VraagID"];

                        // 1. Nieuwe vraag aanmaken als we hem nog niet gezien hebben
                        if (!vraagDict.ContainsKey(vraagId))
                        {
                            Vragen nieuweVraag = new Vragen
                            {
                                // Zorg dat deze property namen exact overeenkomen met Vragen.cs
                                VraagID = vraagId,
                                VraagTekst = reader["VraagZin"].ToString(),
                                Antwoorden = new List<Antwoorden>()
                            };
                            vraagDict.Add(vraagId, nieuweVraag);
                        }

                        // 2. Antwoord toevoegen aan de lijst van de vraag (indien aanwezig)
                        if (reader["IDAntwoord"] != DBNull.Value)
                        {
                            Antwoorden antwoord = new Antwoorden
                            {
                                // Zorg dat deze property namen exact overeenkomen met Antwoorden.cs
                                AntwoordID = (int)reader["IDAntwoord"],
                                AntwoordTekst = reader["AntwoordZin"].ToString(),
                                IsCorrect = (bool)reader["IsCorrect"]
                            };
                            vraagDict[vraagId].Antwoorden.Add(antwoord);
                        }
                    }
                    // Zet de dictionary waarden terug naar een lijst
                    vragen = vraagDict.Values.ToList();
                }
            }
            return vragen;
        }

        public int BewaarQuiz(QuizOpstellen quiz)
        {
            int nieuwQuizId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // 1. Sla de hoofd-quiz op
                // SQL-tabel is QUIZ, kolommen Omschrijving
                string queryQuiz = "INSERT INTO TESTEN (TestOmschrijving) OUTPUT INSERTED.IDTest VALUES (@Omschrijving);";

                using (SqlCommand cmd = new SqlCommand(queryQuiz, conn))
                {
                    cmd.Parameters.AddWithValue("@Omschrijving", quiz.Omschrijving);
                    // ExecuteScalar voert uit en geeft de waarde van de eerste kolom van de eerste rij terug (het nieuwe ID)
                    nieuwQuizId = (int)cmd.ExecuteScalar();
                }

                // 2. Sla de koppeling op in de tussentabel QUIZ_VRAGEN
                string queryTussenTabel = "INSERT INTO TESTVRAGEN (TestID, VraagID,Volgnummer) VALUES (@QuizId, @VraagId, @Volgnummer)";

                foreach (Vragen vraag in quiz.VragenLijst) // Let op: Gebruik hier VragenLijst (naam uit jouw klasse)
                {
                    using (SqlCommand cmdTussen = new SqlCommand(queryTussenTabel, conn))
                    {
                        cmdTussen.Parameters.AddWithValue("@QuizId", nieuwQuizId);
                        // Zorg dat 'Id' overeenkomt met de property in Vragen.cs
                        cmdTussen.Parameters.AddWithValue("@VraagId", vraag.VraagID);
                        cmdTussen.Parameters.AddWithValue("@Volgnummer", quiz.VragenLijst.IndexOf(vraag) + 1);
                        cmdTussen.ExecuteNonQuery();
                    }
                }
            }

            return nieuwQuizId;
        }
        public void BewaarAntwoorden(int quizId, Dictionary<Vragen, GegevenAntwoorden> antwoorden)
        {
            string selectQuery = "SELECT IDTestVraag FROM TESTVRAGEN WHERE TestID = @TestID AND VraagID = @VraagID";
            string insertQuery = "INSERT INTO TESTANTWOORDEN (TestVraagID, AntwoordLetteroptie, AntwoordID) VALUES (@TestVraagID, @AntwoordLetteroptie, @AntwoordID)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var item in antwoorden)
                {
                    Vragen vraag = item.Key;
                    int testVraagId = 0;

                    // 1. Haal het TestVraagID op
                    using (SqlCommand cmdSelect = new SqlCommand(selectQuery, conn))
                    {
                        cmdSelect.Parameters.AddWithValue("@TestID", quizId);
                        cmdSelect.Parameters.AddWithValue("@VraagID", vraag.VraagID);

                        object result = cmdSelect.ExecuteScalar();
                        if (result != null)
                            testVraagId = (int)result;
                    }

                    // 2. Sla ALLE antwoorden van deze vraag op met hun letter (A, B, C, D)
                    if (testVraagId > 0)
                    {
                        char letter = 'A';
                        foreach (var antwoord in vraag.Antwoorden) // alle 4 antwoorden
                        {
                            using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                            {
                                cmdInsert.Parameters.AddWithValue("@TestVraagID", testVraagId);
                                cmdInsert.Parameters.AddWithValue("@AntwoordLetteroptie", letter.ToString());
                                cmdInsert.Parameters.AddWithValue("@AntwoordID", antwoord.AntwoordID);
                                cmdInsert.ExecuteNonQuery();
                            }
                            letter++; // A → B → C → D
                        }
                    }
                }
            }
        }
        // Nieuwe methode toevoegen
        public int BewaarGemaaktTest(int userId, int score)
        {
            string query = "INSERT INTO GEMAAKTETESTEN (UserID, DatumGemaakt, Score) OUTPUT INSERTED.IDGemaakteTest VALUES (@UserID, @DatumGemaakt, @Score)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@DatumGemaakt", DateTime.Now);
                cmd.Parameters.AddWithValue("@Score", score);
                try
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar(); // ← IDGemaakteTest teruggeven
                }
                catch (Exception ex)
                {
                    throw new Exception("Fout bij het opslaan van gemaakte test: " + ex.Message);
                }
            }
        }

        public void BewaarUserTestAntwoorden(int gemaakteTestId, Dictionary<Vragen, GegevenAntwoorden> antwoorden, int quizId)
        {
            // Haal eerst alle TestVraagIDs op
            string selectQuery = "SELECT IDTestVraag, VraagID FROM TESTVRAGEN WHERE TestID = @TestID";
            string insertQuery = "INSERT INTO USERTESTANTWOORDEN (GemaakteTestID, TestVraagID, GekozenLetter) VALUES (@GemaakteTestID, @TestVraagID, @GekozenLetter)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Bouw een dictionary op: VraagID → IDTestVraag
                Dictionary<int, int> vraagNaarTestVraag = new Dictionary<int, int>();

                using (SqlCommand cmdSelect = new SqlCommand(selectQuery, conn))
                {
                    cmdSelect.Parameters.AddWithValue("@TestID", quizId);
                    using (SqlDataReader reader = cmdSelect.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int testVraagId = (int)reader["IDTestVraag"];
                            int vraagId = (int)reader["VraagID"];
                            vraagNaarTestVraag[vraagId] = testVraagId;
                        }
                    }
                }

                // Sla nu elk antwoord op
                foreach (var item in antwoorden)
                {
                    int vraagId = item.Key.VraagID;

                    if (vraagNaarTestVraag.ContainsKey(vraagId))
                    {
                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@GemaakteTestID", gemaakteTestId);
                            cmdInsert.Parameters.AddWithValue("@TestVraagID", vraagNaarTestVraag[vraagId]);
                            cmdInsert.Parameters.AddWithValue("@GekozenLetter", item.Value.GekozenLetter);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        public List<GemaakteTest> GeefScoresVoorUser(int userId)
        {
            List<GemaakteTest> scores = new List<GemaakteTest>();

            string query = @"SELECT IDGemaakteTest, UserID, DatumGemaakt, Score 
                     FROM GEMAAKTETESTEN 
                     WHERE UserID = @UserID 
                     ORDER BY DatumGemaakt DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            scores.Add(new GemaakteTest
                            {
                                IDGemaakteTest = (int)reader["IDGemaakteTest"],
                                UserID = (int)reader["UserID"],
                                DatumGemaakt = (DateTime)reader["DatumGemaakt"],
                                Score = Convert.ToInt32(reader["Score"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Fout bij ophalen scores: " + ex.Message);
                }
            }

            return scores;
        }
    }
    
}



