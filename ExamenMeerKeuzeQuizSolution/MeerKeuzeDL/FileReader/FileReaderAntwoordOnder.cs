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
                    if (char.IsDigit(Lijn[0]) && (Lijn[1] == '.' || char.IsDigit(Lijn[1])))//leest maar tot 99 vragen
                    {
                        vraagText = Lijn.Substring(3);
                        huidigeVraag = new Vragen(vraagText, new List<Antwoorden>());
                        vragen.Add(huidigeVraag);

                        CurrentLijn++;
                    }
                    else if (aanAntwoord == true)
                    {
                        string correctLetter = Lijn.Trim().ToUpper();
                        for (int i = 0; i < vragen.Count; i++)
                        {
                            if ((i == lettersCount))
                            {
                                vragen[i].Antwoorden.ForEach(a =>
                                {
                                    if (a.Letter == correctLetter)
                                    {
                                        a.IsCorrect = true;
                                    }
                                });

                            }
                           
                        }
                        lettersCount++;
                    }
                    else if ((char.IsLetter(Lijn[0]) && Lijn[1] == '.') && aanAntwoord == false)
                    {
                        string antwoordText = Lijn.Substring(2).Trim();
                        string letter = Lijn[0].ToString().ToUpper();
                        huidigeVraag.Antwoorden.Add(new Antwoorden(false, antwoordText, letter));
                    }
                    else if (Lijn.StartsWith("Antwoorden"))
                    {
                        aanAntwoord = true;
                        continue;


                    }
                    


                   
                }

                return vragen;
            }

        }
    }
}

