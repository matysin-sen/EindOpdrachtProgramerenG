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
            List<Antwoorden> antwoorden = new List<Antwoorden>
            {
                new Antwoorden(rbCorrectA.IsChecked == true, txtAntwoordA.Text),
                new Antwoorden(rbCorrectB.IsChecked == true, txtAntwoordB.Text),
                new Antwoorden(rbCorrectC.IsChecked == true, txtAntwoordC.Text),
                new Antwoorden(rbCorrectD.IsChecked == true, txtAntwoordD.Text)
            };

            // 3. Onderwerp ophalen
            Onderwerpen gekozenOnderwerp = (Onderwerpen)cmbOnderwerp.SelectedItem;

            // 4. Vraag aanmaken
            Vragen nieuweVraag = new Vragen
            {
                VraagTekst = txtVraag.Text,
                Antwoorden = antwoorden,
                Onderwerp = new List<Onderwerpen> { gekozenOnderwerp }
            };

            // 5. Opslaan via manager
            _manager.VoegVraagToe(nieuweVraag);

            MessageBox.Show("Vraag succesvol opgeslagen!");
            this.Close();
        }
    }
}