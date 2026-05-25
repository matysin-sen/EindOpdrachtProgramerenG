using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;

namespace MeerKeuzeBL.Managers
{
    public class ImportManager
    {
        private readonly IVragenRepository _repository;
        private readonly IFileReader _fileReader;

        // Constructor: we geven de repository interface mee
        public ImportManager(IVragenRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

        // De hoofdmethode die het inlezen en opslaan coördineert
        public void ImporteerBestand(string pad)
        {
            // 1.BEPAAL HET ONDERWERP VIA JE NIEUWE METHODE
            List < Onderwerpen > berekendeOnderwerpen = BepaalOnderwerpViaBestand(pad);
            // 2. Lees het bestand in met de gekozen strategie (bijv. FileReaderAntwoordOnder)
            List<Vragen> ingelezenVragen = _fileReader.Read(pad);

            if (ingelezenVragen == null || ingelezenVragen.Count == 0)
            {
                throw new Exception("Geen vragen gevonden of het bestand is leeg.");
            }

            // 3. Loop door alle gevonden vragen en sla ze op in de databank
            foreach (Vragen vraag in ingelezenVragen)
            {
                vraag.Onderwerp = berekendeOnderwerpen; // Koppel het berekende onderwerp aan de vraag
                _repository.VoegVraagToe(vraag);
            }
        }

        public void voegonderwerpToe(string onderwerpNaam)
        {
            _repository.VoegOnderwerpToe(onderwerpNaam);
        }

        public List<Onderwerpen> BepaalOnderwerpViaBestand(string bestandsPad)
        {
            // Dit haalt "Geo1.txt" of "Muziek80s1.txt" uit de lange C:\... mapstructuur
            string bestandsNaam = System.IO.Path.GetFileName(bestandsPad);

            Onderwerpen gevondenOnderwerp = null;

            if (bestandsNaam.Contains("Geo"))
            {
                gevondenOnderwerp = new Onderwerpen(1, "Aardrijkskunde");
            }
            else if (bestandsNaam.Contains("Muziek"))
            {
                gevondenOnderwerp = new Onderwerpen(2, "Muziek");
            }
            else if (bestandsNaam.Contains("SQL"))
            {
                gevondenOnderwerp = new Onderwerpen(3, "Informatica");

            }
            else
            {
                gevondenOnderwerp = new Onderwerpen(4, "Alles");
            }

            return new List<Onderwerpen> { gevondenOnderwerp };
        }
    }
}

