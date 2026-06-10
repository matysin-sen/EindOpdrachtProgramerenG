using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Managers;
using System.Collections.Generic;
using System.Windows;

namespace QuizVragenUI
{
    public partial class VraagToevoegen : Window
    {
        private Manager _manager;

        public VraagToevoegen(Manager manager)
        {
            InitializeComponent();
            _manager = manager;

            // Laad de onderwerpen in de combobox
            cmbOnderwerp.ItemsSource = _manager.GeefAlleOnderwerpen();
            cmbOnderwerp.DisplayMemberPath = "OnderwerpNaam";
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validatie
            if (cmbOnderwerp.SelectedItem == null)
            {
                MessageBox.Show("Kies een onderwerp!"); return;
            }
            if (string.IsNullOrWhiteSpace(txtVraag.Text))
            {
                MessageBox.Show("Vul een vraagzin in!"); return;
            }
            if (string.IsNullOrWhiteSpace(txtAntwoordA.Text) ||
                string.IsNullOrWhiteSpace(txtAntwoordB.Text) ||
                string.IsNullOrWhiteSpace(txtAntwoordC.Text) ||
                string.IsNullOrWhiteSpace(txtAntwoordD.Text))
            {
                MessageBox.Show("Vul alle 4 de antwoorden in!"); return;
            }
            if (rbCorrectA.IsChecked == false && rbCorrectB.IsChecked == false &&
                rbCorrectC.IsChecked == false && rbCorrectD.IsChecked == false)
            {
                MessageBox.Show("Duid het juiste antwoord aan!"); return;
            }

            // 2. Antwoorden aanmaken
            List<Antwoord> antwoorden = new List<Antwoord>
            {
                new Antwoord(rbCorrectA.IsChecked == true, txtAntwoordA.Text),
                new Antwoord(rbCorrectB.IsChecked == true, txtAntwoordB.Text),
                new Antwoord(rbCorrectC.IsChecked == true, txtAntwoordC.Text),
                new Antwoord(rbCorrectD.IsChecked == true, txtAntwoordD.Text)
            };

            // 3. Onderwerp ophalen
            Onderwerp gekozenOnderwerp = (Onderwerp)cmbOnderwerp.SelectedItem;

            // 4. Vraag aanmaken
            Vraag nieuweVraag = new Vraag
            {
                VraagTekst = txtVraag.Text,
                Antwoorden = antwoorden,
                Onderwerp = new List<Onderwerp> { gekozenOnderwerp }
            };

            // 5. Opslaan via manager
            _manager.VoegVraagToe(nieuweVraag);

            MessageBox.Show("Vraag succesvol opgeslagen!");
            this.Close();
        }
    }
}