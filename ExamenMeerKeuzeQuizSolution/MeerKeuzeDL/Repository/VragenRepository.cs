using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using Microsoft.Data.SqlClient;

namespace MeerKeuzeDL.Repository
{

        public class VragenRepository : IVragenRepository
    {
            private readonly string _connectionString;

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
    }
}


