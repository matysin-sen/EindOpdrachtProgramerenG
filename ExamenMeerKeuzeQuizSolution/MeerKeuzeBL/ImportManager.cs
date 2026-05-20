using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;

namespace MeerKeuzeBL
{
    public class ImportManager
    {
        private readonly IVragenRepoistory _repository;

        // Constructor: we geven de repository interface mee
        public ImportManager(IVragenRepoistory repository)
        {
            _repository = repository;
        }

        // De hoofdmethode die het inlezen en opslaan coördineert
        public void ImporteerBestand(string pad, IFileReader fileReader)
        {
            // 1. Lees het bestand in met de gekozen strategie (bijv. FileReaderAntwoordOnder)
            List<Vragen> ingelezenVragen = fileReader.Read(pad);

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
    }
}
}
