using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Transactions;

namespace MeerKeuzeDL.Repository
{

        public class VraagRepository : IVraagRepository
    {
            private  string _connectionString;

            // Geef de connection string mee via de constructor
            public VraagRepository(string connectionString)
            {
                _connectionString = connectionString;
            }

        /*
        public void voegvraagtoe(List<Vraag> vragen)
        {
            // OUTPUT INSERTED.VraagID geeft meteen het nieuwe ID terug dat de database heeft verzonnen
            string sqlVraag = "INSERT INTO VRAGEN (Vraagzin) OUTPUT INSERTED.IDVraag VALUES (@Vraagzin)";
            string sqlAntwoord = "INSERT INTO ANTWOORDEN (VraagID, AntwoordZin, IsCorrect) VALUES (@VraagID, @Antwoordzin, @IsCorrect)";
            // --- NIEUW: Query voor de tussentabel ---
            string sqlTussentabel = "INSERT INTO VRAGEN_ONDERWERPEN (VraagID, OnderwerpID) VALUES (@VraagID, @OnderwerpID)";

            // Start een SqlConnection op
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (Vraag vraag in vragen)
                {
                    // Maak de twee commands aan via je connectie
                    using (SqlCommand Vraagcmd = connection.CreateCommand())
                    {
                        using (SqlCommand Antwoordcmd = connection.CreateCommand())
                        {
                            using (SqlCommand Tussentabelcmd = connection.CreateCommand())
                            {



                                // Link de queries aan de juiste commands
                                Vraagcmd.CommandText = sqlVraag;
                                Antwoordcmd.CommandText = sqlAntwoord;
                                Tussentabelcmd.CommandText = sqlTussentabel;

                                // Start een transactie en link ze aan de commands.
                                // We gebruiken een transactie omdat we in twee tabellen tegelijkertijd moeten schrijven
                                SqlTransaction transaction = connection.BeginTransaction();
                                Vraagcmd.Transaction = transaction;
                                Antwoordcmd.Transaction = transaction;
                                Tussentabelcmd.Transaction = transaction;

                                try
                                {
                                    // Maak de parameters aan.
                                    // We hebben telkens maar één loopsessie nodig, dus kunnen we de parameters direct instellen.
                                    Vraagcmd.Parameters.AddWithValue("@Vraagzin", vraag.VraagTekst);


                                    // We voeren deze command uit en slaan telkens de waarde in de eerste kolom (= het id van de sessie) op
                                    int nieuwVraagID = (int)Vraagcmd.ExecuteScalar();


                                    Antwoordcmd.Parameters.AddWithValue("@VraagID", nieuwVraagID);
                                    Antwoordcmd.Parameters.Add("@Antwoordzin", SqlDbType.VarChar, 255);
                                    Antwoordcmd.Parameters.Add("@IsCorrect", SqlDbType.Bit);
                                    foreach (Antwoord antwoord in vraag.Antwoorden)
                                    {
                                        Antwoordcmd.Parameters["@Antwoordzin"].Value = antwoord.AntwoordTekst;
                                        Antwoordcmd.Parameters["@IsCorrect"].Value = antwoord.IsCorrect;

                                        Antwoordcmd.ExecuteNonQuery();
                                    }

                                    foreach (Onderwerp onderwerp in vraag.Onderwerp)
                                    {
                                        Tussentabelcmd.Parameters.AddWithValue("@VraagID", nieuwVraagID);
                                        Tussentabelcmd.Parameters.AddWithValue("@OnderwerpID", onderwerp.OnderwerpID);
                                        Tussentabelcmd.ExecuteNonQuery();
                                    }
                                    transaction.Commit();

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    throw ex;
                                }
                            }
                        }
                    }  
                }
            }
        }
        */
        public int dubbels;
        public void VoegVraagToe(Vraag vraag)
        {
            // OUTPUT INSERTED.VraagID geeft meteen het nieuwe ID terug dat de database heeft verzonnen
            string sqlVraag = "INSERT INTO VRAGEN (Vraagzin) OUTPUT INSERTED.IDVraag VALUES (@Vraagzin)";
            string sqlAntwoord = "INSERT INTO ANTWOORDEN (VraagID, AntwoordZin, IsCorrect) VALUES (@VraagID, @Antwoordzin, @IsCorrect)";
            // --- NIEUW: Query voor de tussentabel ---
            string sqlTussentabel = "INSERT INTO VRAGEN_ONDERWERPEN (VraagID, OnderwerpID) VALUES (@VraagID, @OnderwerpID)";

            string sqlDubbelVraag = "select * from VRAGEN where Vraagzin = @Vraagzin";
            string sqlDubbelVragenOnderwerp = "select * from VRAGEN_ONDERWERPEN where VraagID = @VraagID and OnderwerpID = @OnderwerpID";
            //string sqlDubbelAntwoorden = "select * from VRAGEN where lower(Vraagzin) = lower(@Vraagzin)";
            int idvraag;
            // Onderwerp onderwerp = null;
            
            int onderwerpId;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            using (SqlCommand cmdVragenOnderwerp = conn.CreateCommand())
            {
                cmd.CommandText = sqlDubbelVraag;
                cmd.Parameters.AddWithValue("@Vraagzin", vraag._vraagTekst);


                cmdVragenOnderwerp.CommandText = sqlDubbelVragenOnderwerp;


                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {        
                    if (reader.HasRows)
                    {
                        reader.Read();
                        idvraag = (int)reader["IDVraag"];
                        reader.Close();
                        foreach (var onderwerp in vraag.Onderwerp)
                        {
                            onderwerpId = onderwerp.OnderwerpID;
                            cmdVragenOnderwerp.Parameters.AddWithValue("@OnderwerpID", onderwerpId);
                        }

                            cmdVragenOnderwerp.Parameters.AddWithValue("@VraagID", idvraag);
                        // dit zorgt voor dat er geen dubbele vragen_onderwerpen worden geladen
                        using (SqlDataReader readerVragenOnderwerp = cmdVragenOnderwerp.ExecuteReader())
                        {
                            if (readerVragenOnderwerp.HasRows)
                            {
                                dubbels++;
                                return;

                            }
                            else
                            {
                                readerVragenOnderwerp.Close();
                                using (SqlTransaction transaction = conn.BeginTransaction())
                                {
                                    try
                                    {


                                        foreach (var onderwerp in vraag.Onderwerp)
                                        {
                                            using (SqlCommand cmdTussen = new SqlCommand(sqlTussentabel, conn, transaction))
                                            {
                                                cmdTussen.Parameters.AddWithValue("@VraagID", idvraag);
                                                cmdTussen.Parameters.AddWithValue("@OnderwerpID", onderwerp.OnderwerpID); // Zorg dat dit klopt met de property naam in je klasse Onderwerpen!

                                                cmdTussen.ExecuteNonQuery();
                                            }
                                        }

                                    } catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        throw new Exception("Er is een fout opgetreden bij het toevoegen van de vraag aan het onderwerp: " + ex.Message);
                                    }
                                    // Alles is goed gegaan, sla de wijzigingen definitief op in de databank
                                    transaction.Commit();
                                }
                                
                            }
                        }
                       
                       
                    }
                    else
                    {
                        reader.Close();



                        // Start een transactie
                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                int nieuwVraagID;


                                // 1. Sla de vraag op
                                using (SqlCommand cmdVraag = new SqlCommand(sqlVraag, conn, transaction))
                                {
                                    cmdVraag.Parameters.AddWithValue("@Vraagzin", vraag._vraagTekst);


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
            }
        }
        public Onderwerp VoegOnderwerpToe(string onderwerpNaam)
        {
            int id;
            Onderwerp onderwerp = null;
            const string DubbelCheck = "select * from ONDERWERPEN where  Lower(OnderwerpNaam) = lower(@OnderwerpNaam)";
            // De query is aangepast naar jouw tabelnaam en kolomnamen uit SQLQueryQuizMaken.sql
            const string Insertqry = "INSERT INTO ONDERWERPEN (Onderwerpnaam) OUTPUT INSERTED.IDOnderwerp VALUES (@Onderwerpnaam)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                using (SqlCommand cmdSelect = new SqlCommand(DubbelCheck, conn))
                {
                    cmdSelect.CommandText = DubbelCheck;

                   
                    cmdSelect.Parameters.AddWithValue("@OnderwerpNaam", onderwerpNaam);
                    conn.Open();
                    using (SqlDataReader reader = cmdSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Er is al een onderwerp met deze naam, we maken een Onderwerp object aan en vullen het ID
                            throw new Exception("Er bestaat al een onderwerp met deze naam. Kies een andere naam.");
                            
                        }
                        else
                        {
                            reader.Close();
                            // Er is nog geen onderwerp met deze naam, we voegen het toe
                            using (SqlCommand cmdInsert = new SqlCommand(Insertqry, conn))
                            {
                                cmdInsert.CommandText = Insertqry;
                                cmdInsert.Parameters.AddWithValue("@Onderwerpnaam", onderwerpNaam);
                                try
                                {
                                    // ExecuteScalar voert de INSERT uit en pakt de OUTPUT INSERTED.IDOnderwerp direct vast
                                    id = (int)cmdInsert.ExecuteScalar();

                                    // We maken een nieuw object van jouw Domeinklasse
                                    onderwerp = new Onderwerp(id, onderwerpNaam);
                                }
                                catch (Exception ex)
                                {
                                    // Fouten netjes opvangen en doorgeven naar de UI zodat je weet wát er misging
                                    throw new Exception("Fout bij het toevoegen van het onderwerp: " + ex.Message);
                                }
                                
                            }
                        }
                    }
                }



                //    cmd.Parameters.AddWithValue("@Onderwerpnaam", onderwerpNaam);

                //    conn.Open();
                //    try
                //    {
                //        // ExecuteScalar voert de INSERT uit en pakt de OUTPUT INSERTED.IDOnderwerp direct vast
                //        id = (int)cmd.ExecuteScalar();

                //        // We maken een nieuw object van jouw Domeinklasse
                //        onderwerp = new Onderwerp(id, onderwerpNaam);
                //    }
                //    catch (Exception ex)
                //    {
                //        // Fouten netjes opvangen en doorgeven naar de UI zodat je weet wát er misging
                //        throw new Exception("Fout bij het toevoegen van het onderwerp: " + ex.Message);
                //    }
                //}

                return onderwerp;
            }
        }
        public List<Onderwerp> GeefAlleOnderwerpen()
        {
            List<Onderwerp> onderwerpenLijst = new List<Onderwerp>();

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
                                Onderwerp onderwerp = new Onderwerp(id, naam);
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
        int userID;
        public int voegUserToe(string naam, string achternaam)
        {
            // 1. Schrijf de SQL query (Pas 'GEBRUIKERS', 'Naam' en 'Achternaam' aan naar jouw echte tabel- en kolomnamen)
            string query = "INSERT INTO USERS (Naam, Voornaam) OUTPUT INSERTED.IDUser VALUES (@Naam, @Voornaam)";
            string selectQuery = "SELECT * FROM USERS WHERE LOWER(Naam) = LOWER(@Naam) AND LOWER(Voornaam) = LOWER(@Voornaam)";
            // 2. Maak verbinding met de databank
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // 3. Maak het SQL commando klaar
                using (SqlCommand cmdSelect = new SqlCommand(selectQuery, conn))
                {
                    cmdSelect.Parameters.AddWithValue("@Naam", naam);
                    cmdSelect.Parameters.AddWithValue("@Voornaam", achternaam);
                    // 4. Controleer of de gebruiker al bestaat
                    using (SqlDataReader reader = cmdSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // De gebruiker bestaat al, we kunnen het ID teruggeven of een foutmelding geven
                            return userID = (int)reader["IDUser"];

                        }
                        else
                        {
                            reader.Close();
                            // De gebruiker bestaat nog niet, we voegen hem toe
                            using (SqlCommand cmdInsert = new SqlCommand(query, conn))
                            {
                                cmdInsert.Parameters.AddWithValue("@Naam", naam);
                                cmdInsert.Parameters.AddWithValue("@Voornaam", achternaam);
                                try
                                {
                                    // ExecuteScalar voert de INSERT uit en pakt de OUTPUT INSERTED.IDUser direct vast
                                    userID = (int)cmdInsert.ExecuteScalar();
                                    return userID;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Fout bij het toevoegen van de gebruiker: " + ex.Message);
                                }
                            }
                        }
                    }
                }
                /*
                using (SqlCommand cmdUser = new SqlCommand(selectQuery, conn))
                {
                    cmdUser.CommandText = selectQuery;
                    cmdUser.Parameters.AddWithValue("@Naam", achternaam);
                    cmdUser.Parameters.AddWithValue("@Voornaam", naam);
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            userID = (int)reader["IDUser"];
                            reader.Close();
                            return userID;
                        }
                        else
                        {
                            reader.Close();
                            cmd.CommandText = query;
                            // 4. Koppel de parameters aan de veilige @-variabelen om SQL-injectie te voorkomen
                            cmd.Parameters.AddWithValue("@Naam", achternaam);
                            cmd.Parameters.AddWithValue("@Voornaam", naam);

                            try
                            {


                                // ExecuteNonQuery gebruiken we voor INSERT, UPDATE of DELETE (als we geen ID terug hoeven)
                                return (int)cmd.ExecuteScalar(); // ← ID teruggeven
                            }
                            catch (Exception ex)
                            {
                                // Fouten opvangen en duidelijk doorgeven
                                throw new Exception("Fout bij het toevoegen van de user: " + ex.Message);
                            }
                        }
                    }*/

            }
        
            }
        

        public List<Vraag> GeefRandomVragenVoorOnderwerp(int onderwerpId, int aantalVragen)
        {
            List<Vraag> quizVragen = new List<Vraag>();

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
                            Vraag vraag = new Vraag(vraagId, vraagZin, new List<Antwoord>(), new List<Onderwerp>());
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

                                vraag.Antwoorden.Add(new Antwoord(antwoordId, isCorrect, antwZin)); // ← constructor met ID
                            }
                        }
                    }
                }
            }

            // Geef de complete lijst (vragen mét antwoorden) terug aan je Manager
            return quizVragen;
        }

        public List<Vraag> GeefVragenPerOnderwerp(int onderwerpId)
        {
            List<Vraag> vragen = new List<Vraag>();

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
                    Dictionary<int, Vraag> vraagDict = new Dictionary<int, Vraag>();

                    while (reader.Read())
                    {
                        int vraagId = (int)reader["VraagID"];

                        // 1. Nieuwe vraag aanmaken als we hem nog niet gezien hebben
                        if (!vraagDict.ContainsKey(vraagId))
                        {
                            Vraag nieuweVraag = new Vraag
                            {
                                // Zorg dat deze property namen exact overeenkomen met Vragen.cs
                                VraagID = vraagId,
                                _vraagTekst = reader["VraagZin"].ToString(),
                                Antwoorden = new List<Antwoord>()
                            };
                            vraagDict.Add(vraagId, nieuweVraag);
                        }

                        // 2. Antwoord toevoegen aan de lijst van de vraag (indien aanwezig)
                        if (reader["IDAntwoord"] != DBNull.Value)
                        {
                            Antwoord antwoord = new Antwoord
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

                foreach (Vraag vraag in quiz.VragenLijst) // Let op: Gebruik hier VragenLijst (naam uit jouw klasse)
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
        public void BewaarAntwoorden(int quizId, Dictionary<Vraag, GegevenAntwoord> antwoorden)
        {
            string selectQuery = "SELECT IDTestVraag FROM TESTVRAGEN WHERE TestID = @TestID AND VraagID = @VraagID";
            string insertQuery = "INSERT INTO TESTANTWOORDEN (TestVraagID, AntwoordLetteroptie, AntwoordID) VALUES (@TestVraagID, @AntwoordLetteroptie, @AntwoordID)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (var item in antwoorden)
                {
                    Vraag vraag = item.Key;
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

        public void BewaarUserTestAntwoorden(int gemaakteTestId, Dictionary<Vraag, GegevenAntwoord> antwoorden, int quizId)
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



