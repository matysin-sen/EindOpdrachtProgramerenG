using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using System.Linq;

namespace MeerKeuzeBL.Managers
{
    public class ImportManager
    {
        private readonly IVragenRepository _repository;
        private  IFileReader _fileReader;
       
        // Constructor: we geven de repository interface mee
        public ImportManager(IVragenRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

        // De hoofdmethode die het inlezen en opslaan coördineert
        /*
        public void ImporteerBestand(string pad)
        {
            List<Onderwerpen> alleOnderwerpen = _repository.GeefAlleOnderwerpen();
            // 1.BEPAAL HET ONDERWERP VIA JE NIEUWE METHODE
            Onderwerpen match = BepaalOnderwerpViaBestand(pad, alleOnderwerpen);

            if (match == null) 
            {
                throw new Exception("Geen overeenkomend onderwerp gevonden voor dit bestand.");
            }
            // 2. Lees het bestand in met de gekozen strategie (bijv. FileReaderAntwoordOnder)
            List<Vragen> ingelezenVragen = _fileReader.Read(pad);

            if (ingelezenVragen == null || ingelezenVragen.Count == 0)
            {
                throw new Exception("Geen vragen gevonden of het bestand is leeg.");
            }

            // 3. Loop door alle gevonden vragen en sla ze op in de databank
            foreach (Vragen vraag in ingelezenVragen)
            {
                vraag.Onderwerp = new List<Onderwerpen> { match };// Koppel het berekende onderwerp aan de vraag
                _repository.VoegVraagToe(vraag);
            }
        }*/
        // In ImportManager.cs
        public void ImporteerBestand(string pad,IFileReader reader, Onderwerpen gekozenOnderwerp)
        {
            // 1. Maak de juiste reader aan via de factory
            // We sturen de benodigde parameters mee die je in FileReaderFactory.cs hebt gedefinieerd
            
            List<Vragen> ingelezenVragen = reader.Read(pad);

            foreach (Vragen vraag in ingelezenVragen)
            {
                // Koppel direct het object dat de gebruiker in de UI heeft gekozen
                vraag.Onderwerp = new List<Onderwerpen> { gekozenOnderwerp };
                _repository.VoegVraagToe(vraag);
            }
        }

        public void voegonderwerpToe(string onderwerpNaam)
        {
            _repository.VoegOnderwerpToe(onderwerpNaam);
        }

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

            // 2. Zoek in de lijst uit de database naar het object met de juiste naam
            // .FirstOrDefault() geeft null terug als de naam niet in de DB staat
            return alleOnderwerpen.FirstOrDefault(o => o.OnderwerpNaam.Equals(gezochteNaam, StringComparison.OrdinalIgnoreCase));
        }
    }
}

