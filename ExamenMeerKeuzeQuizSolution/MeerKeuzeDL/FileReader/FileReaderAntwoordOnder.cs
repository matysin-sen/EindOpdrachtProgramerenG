using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeDL.FileReader
{
    public class FileReaderAntwoordOnder : IFileReader
    {
        public List<Vragen> Read(string pad)
        {
           
            bool aanAntwoord = false;
            int CurrentLijn = 0;
            int lettersCount = 0;
            List<Vragen> vragen = new List<Vragen>();
            Vragen huidigeVraag = null;

            using (StreamReader sr = new StreamReader(pad))
            {
                string vraagText;

                while (!sr.EndOfStream)
                {
                    string Lijn = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(Lijn))
                    {
                        
                        continue;
                    }
                    // 1. Controleer of het een vraag is (begint met 1 of meer cijfers, gevolgd door een punt)
                    // We checken nu eerst of de lengte wel minstens 2 tekens is om crashes te voorkomen.
                    if (Lijn.Length >= 2 && char.IsDigit(Lijn[0]) && (Lijn[1] == '.' || char.IsDigit(Lijn[1])))
                    {
                        // Zoek op welke positie het puntje precies staat
                        int puntIndex = Lijn.IndexOf('.');

                        // Knip alles weg NA het puntje
                        if (puntIndex != -1 && Lijn.Length > puntIndex + 1)
                        {
                            vraagText = Lijn.Substring(puntIndex + 1).Trim();
                        }
                        else
                        {
                            // Fallback voor als er geen tekst achter het puntje stond
                            vraagText = "";
                        }

                        huidigeVraag = new Vragen(vraagText, new List<Antwoorden>());
                        vragen.Add(huidigeVraag);
                        CurrentLijn++;
                    }
                    // 2. Controleren of we bij het sectie "Antwoorden" onderaan het document zijn beland
                    else if (Lijn.StartsWith("Antwoorden", StringComparison.OrdinalIgnoreCase))
                    {
                        aanAntwoord = true;
                        continue;
                    }
                    // 3. De juiste letters onderaan het document uitlezen en koppelen
                    else if (aanAntwoord == true)
                    {
                        string correctLetter = Lijn.Trim().ToUpper();

                        // Zorg dat we niet crashen als er per ongeluk meer letters dan vragen staan
                        if (lettersCount < vragen.Count)
                        {
                            vragen[lettersCount].Antwoorden.ForEach(a =>
                            {
                                if (a.Letter == correctLetter)
                                {
                                    a.IsCorrect = true;
                                }
                            });
                            lettersCount++;
                        }
                    }
                    // 4. De mogelijke antwoord-opties uitlezen (bijv. "A. Parijs")
                    else if (Lijn.Length >= 2 && char.IsLetter(Lijn[0]) && Lijn[1] == '.' && aanAntwoord == false)
                    {
                        string antwoordText = "";

                        // Veilig knippen na "A."
                        if (Lijn.Length > 2)
                        {
                            antwoordText = Lijn.Substring(2).Trim();
                        }

                        string letter = Lijn[0].ToString().ToUpper();

                        // Zorg dat we een vraag hebben om het antwoord aan toe te voegen
                        if (huidigeVraag != null)
                        {
                            huidigeVraag.Antwoorden.Add(new Antwoorden(false, antwoordText, letter));
                        }
                    }
                    // 5. ZWEVENDE TEKST OPVANGEN (De vraag zelf of titels)
                    else if (huidigeVraag != null && aanAntwoord == false)
                    {
                        // Negeer de hoofdtitel bovenaan het document
                        if (!Lijn.ToLower().Contains("raad de artiest") && !Lijn.ToLower().Contains("quiz"))
                        {
                            // Plak deze tekst vast aan de vraag die we momenteel aan het inlezen zijn
                            huidigeVraag.VraagTekst += " " + Lijn.Trim();
                            huidigeVraag.VraagTekst = huidigeVraag.VraagTekst.Trim(); // Zorg dat er geen spatie teveel vooraan staat
                        }
                    }
                }

                return vragen;
            }
        }
    }
}

