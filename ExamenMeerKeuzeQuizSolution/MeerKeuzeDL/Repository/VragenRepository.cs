using MeerKeuzeBL.Domein;
using Microsoft.Data.SqlClient;

namespace MeerKeuzeDL.Repository
{

        public class VragenRepository
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
            string sqlVraag = "INSERT INTO Vragen (Vraagzin) OUTPUT INSERTED.VraagID VALUES (@VraagTekst)";
            string sqlAntwoord = "INSERT INTO Antwoorden (VraagID, AntwoordZin, IsCorrect) VALUES (@VraagID, @AntwoordTekst, @IsCorrect)";

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
        }
}


