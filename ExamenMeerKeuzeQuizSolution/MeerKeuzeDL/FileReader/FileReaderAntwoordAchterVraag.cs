using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeDL.FileReader
{
    public class FileReaderAntwoordAchterVraag : IFileReader
    {
        public List<Vragen> Read(string pad)
        {
            bool Lezer = false;
            int CurrentLijn = 0;
            List<Vragen> vragen = new List<Vragen>();
            Vragen huidigeVraag = null;

            using (StreamReader sr = new StreamReader(pad)) {
                string vraagText;

                while (!sr.EndOfStream) 
                {
                    string Lijn = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(Lijn)) 
                    { 
                        Lezer = false;
                        continue;
					}
                    if(char.IsDigit(Lijn[0]) && Lijn[1] == '.')
                    {
                        vraagText = Lijn.Substring(3);
                        huidigeVraag = new Vragen { VraagTekst = vraagText, Antwoorden = new List<Antwoorden>() };
                        vragen.Add(huidigeVraag);
						Lezer = true;
                        CurrentLijn++;
                    }
                    else if(Lezer && (char.IsLetter(Lijn[0]) && Lijn[1] == '.'))
                    {
                      string antwoordText = Lijn.Substring(2).Trim();
                      string letter = Lijn[0].ToString().ToUpper();
                      huidigeVraag.Antwoorden.Add(new Antwoorden (false , antwoordText, letter));
					}
					else if (Lijn.StartsWith("Correct:"))
                    {
                        string correctLetter = Lijn.Substring("Correct:".Length).Trim().ToUpper();

                       
                        for(int i = 0; i < huidigeVraag.Antwoorden.Count; i++) {
                            if(huidigeVraag.Antwoorden[i].Letter == correctLetter) {
                                huidigeVraag.Antwoorden[i].IsCorrect = true;
                                break;
                            }
                        }
                    }
                }
            }

            return vragen;
        }

    }
}
