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

            // 1. Lees het bestand in met de gekozen strategie (bijv. FileReaderAntwoordOnder)
            List<Vragen> ingelezenVragen = _fileReader.Read(pad);

            if (ingelezenVragen == null || ingelezenVragen.Count == 0)
            {
                throw new Exception("Geen vragen gevonden of het bestand is leeg.");
            }

            // 2. Loop door alle gevonden vragen en sla ze op in de databank
            foreach (Vragen vraag in ingelezenVragen)
            {
                _repository.VoegVraagToe(vraag);
            }
        }

        public void voegonderwerpToe(string onderwerpNaam)
        {
            _repository.VoegOnderwerpToe(onderwerpNaam);
        }
    }
}

