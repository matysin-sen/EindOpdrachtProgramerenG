using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System.Linq;

namespace MeerKeuzeBL.Managers
{
    public class ImportManager
    {
        private readonly IVraagRepository _repository;
        private  IFileReader _fileReader;
       
        // Constructor: we geven de repository interface mee
        public ImportManager(IVraagRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

     
        public void ImporteerBestand(string pad,IFileReader reader, Onderwerp gekozenOnderwerp)
        {
            // 1. Maak de juiste reader aan via de factory
            // We sturen de benodigde parameters mee die je in FileReaderFactory.cs hebt gedefinieerd
            
            List<Vraag> ingelezenVragen = reader.Read(pad);

            foreach (Vraag vraag in ingelezenVragen)
            {
                // Koppel direct het object dat de gebruiker in de UI heeft gekozen
                vraag.Onderwerp = new List<Onderwerp> { gekozenOnderwerp };
                _repository.VoegVraagToe(vraag);
            }
        }

        public void voegonderwerpToe(string onderwerpNaam)
        {
            _repository.VoegOnderwerpToe(onderwerpNaam);
        }

        /*
        public Onderwerpen BepaalOnderwerpViaBestand(string bestandsPad, List<Onderwerpen> alleOnderwerpen)
        {
            // Dit haalt "Geo1.txt" of "Muziek80s1.txt" uit de lange C:\... mapstructuur
            string bestandsNaam = System.IO.Path.GetFileName(bestandsPad).ToUpper();
            string gezochteNaam = "";
            if (bestandsNaam.Contains("GEO"))
            {
                gezochteNaam = "Aardrijkskunde";
               
            }
            else if (bestandsNaam.Contains("MUZIEK"))
            {
                gezochteNaam = "Muziek";
               
            }
            else if (bestandsNaam.Contains("SQL"))
            {
                gezochteNaam = "SQL";
               
            }
            else
            {
                // Fallback als er geen match is
                gezochteNaam = "algemeen";
                
            }
               // ik vond dit nog een leuke manier om het te doen maar het was niet de meest efficiënte, dus ik heb het uiteindelijk niet gebruikt.
            // 2. Zoek in de lijst uit de database naar het object met de juiste naam
            // .FirstOrDefault() geeft null terug als de naam niet in de DB staat
            return alleOnderwerpen.FirstOrDefault(o => o.OnderwerpNaam.Equals(gezochteNaam, StringComparison.OrdinalIgnoreCase));
        }*/
    }
}

