using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeerKeuzeDL.FileReader
{
    public class FileReaderAntwoordAchterVraag : IFileReader
    {
        public List<Vraag> Read(string pad)
        {
            
            int CurrentLijn = 0;
            List<Vraag> vragen = new List<Vraag>();
            Vraag huidigeVraag = null;

            using (StreamReader sr = new StreamReader(pad)) {
                string vraagText;

                while (!sr.EndOfStream) 
                {
                    string Lijn = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(Lijn)) 
                    { 
                      
                        continue;
					}
                    if(char.IsDigit(Lijn[0]) && (Lijn[1] == '.' || char.IsDigit(Lijn[1])))//leest maar tot 99 vragen niet meer
                    {
                        vraagText = Lijn.Substring(3);
                        huidigeVraag = new Vraag (vraagText, new List<Antwoord>());
                        vragen.Add(huidigeVraag);
						
                        CurrentLijn++;
                    }
                    else if((char.IsLetter(Lijn[0]) && Lijn[1] == '.'))
                    {
                      string antwoordText = Lijn.Substring(2).Trim();
                      string letter = Lijn[0].ToString().ToUpper();
                      huidigeVraag.Antwoorden.Add(new Antwoord (false , antwoordText, letter));
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
