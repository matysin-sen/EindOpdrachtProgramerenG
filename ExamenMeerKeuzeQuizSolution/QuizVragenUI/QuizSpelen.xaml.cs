using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuizVragenUI
{
    /// <summary>
    /// Interaction logic for QuizSpelen.xaml
    /// </summary>
    public partial class QuizSpelen : Window
    {
        private Manager _manager;

        // We slaan de hele quiz op in plaats van alleen de vragen
        private QuizOpstellen _actieveQuiz;

        private int _huidigeVraagIndex = 0;

        // Constructor krijgt nu het QuizOpstellen object binnen
        public QuizSpelen(Manager manager, QuizOpstellen actieveQuiz)
        {
            InitializeComponent();
            _manager = manager;
            _actieveQuiz = actieveQuiz;

            if (_actieveQuiz.VragenLijst != null && _actieveQuiz.VragenLijst.Count > 0)
            {
                LaadVraag();
            }
            else
            {
                MessageBox.Show("Er zijn geen vragen gevonden voor deze quiz!");
                this.Close();
            }
        }

        private void LaadVraag()
        {
            // Reset de bolletjes
            rbOptieA.IsChecked = false; rbOptieB.IsChecked = false;
            rbOptieC.IsChecked = false; rbOptieD.IsChecked = false;

            // Haal de vraag op via jouw VragenLijst property
            Vragen huidigeVraag = _actieveQuiz.VragenLijst[_huidigeVraagIndex];

            // Toon de vraag (pas aan naar jouw property naam, bv. VraagStelling)
            txtVraag.Text = $"Vraag {_huidigeVraagIndex + 1}: {huidigeVraag.VraagTekst}";

            // Toon antwoorden in de RadioButtons (pas 'Tekst' aan indien nodig)
            if (huidigeVraag.Antwoorden.Count >= 4)
            {
                rbOptieA.Content = "A. " + huidigeVraag.Antwoorden[0].AntwoordTekst;
                rbOptieA.Tag = huidigeVraag.Antwoorden[0].AntwoordID;

                rbOptieB.Content = "B. " + huidigeVraag.Antwoorden[1].AntwoordTekst;
                rbOptieB.Tag = huidigeVraag.Antwoorden[1].AntwoordID;

                rbOptieC.Content = "C. " + huidigeVraag.Antwoorden[2].AntwoordTekst;
                rbOptieC.Tag = huidigeVraag.Antwoorden[2].AntwoordID;

                rbOptieD.Content = "D. " + huidigeVraag.Antwoorden[3].AntwoordTekst;
                rbOptieD.Tag = huidigeVraag.Antwoorden[3].AntwoordID;
            }
        }

        private void btnVolgende_Click(object sender, RoutedEventArgs e)
        {
            if (rbOptieA.IsChecked == false && rbOptieB.IsChecked == false &&
                rbOptieC.IsChecked == false && rbOptieD.IsChecked == false)
            {
                MessageBox.Show("Je moet een antwoord selecteren!");
                return;
            }

            // 1. Vind de geselecteerde radiobutton
            RadioButton geselecteerde = null;
            if (rbOptieA.IsChecked == true) geselecteerde = rbOptieA;
            else if (rbOptieB.IsChecked == true) geselecteerde = rbOptieB;
            else if (rbOptieC.IsChecked == true) geselecteerde = rbOptieC;
            else if (rbOptieD.IsChecked == true) geselecteerde = rbOptieD;

            if (geselecteerde == null)
            {
                MessageBox.Show("Selecteer een antwoord!");
                return;
            }
            Vragen huidigeVraag = _actieveQuiz.VragenLijst[_huidigeVraagIndex];
            // 2. Haal het AntwoordId uit de Tag (de index van je lijst)
            int index = int.Parse(geselecteerde.Tag.ToString());
            var gekozenAntwoordObj = huidigeVraag.Antwoorden[index];

            // 3. Bepaal de letter (bijv. 'A' als index 0 is)
            string gekozenLetter = ((char)('A' + index)).ToString();

            // 4. Stuur naar de manager
            _manager.BeantwoordVraag(_actieveQuiz, huidigeVraag, gekozenLetter, gekozenAntwoordObj.AntwoordID);

     
            // 4. Ga naar de volgende vraag
            _huidigeVraagIndex++;

            if (_huidigeVraagIndex < _actieveQuiz.VragenLijst.Count)
            {
                if (_huidigeVraagIndex == _actieveQuiz.VragenLijst.Count - 1)
                {
                    btnVolgende.Content = "Quiz Afronden";
                }
                LaadVraag();
            }
            else
            {
                // Einde quiz! Nu lees je de score direct uit jouw klasse
                MessageBox.Show($"Einde van de quiz! Jouw score is: {_actieveQuiz.Score} van de {_actieveQuiz.VragenLijst.Count}");

                // Omdat _actieveQuiz nu vol zit met de IngevuldeAntwoorden dictionary,
                // kun je deze nu makkelijk opslaan in de database (GegevenAntwoorden).
                // _manager.SlaGegevenAntwoordenOp(_actieveQuiz); 

                this.Close();
            }
        }
    }
}
