using MeerKeuzeBL.Domein;
using MeerKeuzeDL.FileReader;

namespace ExamenMeerKeuzeQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string pad = @"C:\Users\matys\source\repos\hogent 25-26 programeren basis\programeren gevorderd\Examen\c_1.txt";

            var reader = new FileReaderAntwoordAchterVraag();
            List<Vragen> vragenLijst = reader.Read(pad);

            foreach (var vraag in vragenLijst)
            {
                Console.WriteLine($"Vraag {vraag.VraagID + 1}: {vraag.VraagTekst}");
                char antwoordLetter = 'A';

                foreach (var antwoord in vraag.Antwoorden)
                {
                    string correctMarkering = antwoord.IsCorrect ? "(Correct)" : "";
                    Console.WriteLine($"  {antwoordLetter}. {antwoord.AntwoordTekst} {correctMarkering}");
                    antwoordLetter++;
                }
                Console.WriteLine(); // lege regel voor overzicht
            }
        }
    }
}
