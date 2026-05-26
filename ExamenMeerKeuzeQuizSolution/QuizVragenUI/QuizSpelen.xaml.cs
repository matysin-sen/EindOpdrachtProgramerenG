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
        private int userId;

        // We slaan de hele quiz op in plaats van alleen de vragen
        private QuizOpstellen _actieveQuiz;

        private int _huidigeVraagIndex = 0;
        private Vragen huidigeVraag;

        // Constructor krijgt nu het QuizOpstellen object binnen
        public QuizSpelen(Manager manager, QuizOpstellen actieveQuiz , int userID)
        {
            InitializeComponent();
            _manager = manager;
            _actieveQuiz = actieveQuiz;
            this.userId = userID;

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
            huidigeVraag = _actieveQuiz.VragenLijst[_huidigeVraagIndex];

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

            // 1. Check selectie
            RadioButton gekozen = null;
            int letterIndex = -1; // 0=A, 1=B, 2=C, 3=D

            if (rbOptieA.IsChecked == true) { gekozen = rbOptieA; letterIndex = 0; }
            else if (rbOptieB.IsChecked == true) { gekozen = rbOptieB; letterIndex = 1; }
            else if (rbOptieC.IsChecked == true) { gekozen = rbOptieC; letterIndex = 2; }
            else if (rbOptieD.IsChecked == true) { gekozen = rbOptieD; letterIndex = 3; }

            if (gekozen == null)
            {
                MessageBox.Show("Selecteer eerst een antwoord!");
                return;
            }

            // 2. Gebruik letterIndex voor de lijst, Tag voor het AntwoordID
            int antwoordId = (int)gekozen.Tag; // AntwoordID uit de DB
            var gekozenAntwoordObj = huidigeVraag.Antwoorden[letterIndex]; // Juiste lijstindex
            string gekozenLetter = ((char)('A' + letterIndex)).ToString();

            // 3. Opslaan in Manager
            _manager.BeantwoordVraag(_actieveQuiz, huidigeVraag, gekozenLetter, antwoordId);

            // 4. Volgende vraag of afronden
            _huidigeVraagIndex++;

            if (_huidigeVraagIndex < _actieveQuiz.VragenLijst.Count)
            {
                LaadVraag();
                if (_huidigeVraagIndex == _actieveQuiz.VragenLijst.Count - 1)
                    btnVolgende.Content = "Quiz Afronden";
            }
            else
            {
                // --- HIER GAAT HET MIS: QUIZ OPSLAAN ---
                // 1. Eerst de quiz opslaan in DB om het ID te krijgen
                //int opgeslagenQuizId = _manager.BewaarQuiz(_actieveQuiz);

                // 2. Nu pas de antwoorden opslaan met het ID
                _manager.SlaQuizEnAntwoordenOp(_actieveQuiz, userId);

                MessageBox.Show($"Einde quiz! Score: {_actieveQuiz.Score}");
                this.Close();
            }
        }
    }
}
