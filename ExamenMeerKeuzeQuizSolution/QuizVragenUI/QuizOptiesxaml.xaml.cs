using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using MeerKeuzeDL.Repository;
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
    /// Interaction logic for QuizOptiesxaml.xaml
    /// </summary>
    public partial class QuizOptiesxaml : Window
    {
        public Manager manager;
        private IVraagRepository repository;
        private int userID;
        public QuizOptiesxaml(Manager manager, int userID)
        {
            InitializeComponent();
            this.manager = manager;
            this.userID = userID;
            cmbBoxOnderwerp.ItemsSource = manager.GeefAlleOnderwerpen();
        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Haal de gegevens op uit je scherm
                if (cmbBoxOnderwerp.SelectedItem == null)
                {
                    MessageBox.Show("Kies een onderwerp!");
                    return;
                }
                Onderwerp gekozenOnderwerp = (Onderwerp)cmbBoxOnderwerp.SelectedItem;

                // Controleer of aantal een geldig getal is
                if (!int.TryParse(AantalVragen.Text, out int aantalVragen))
                {
                    MessageBox.Show("Vul een geldig getal in voor het aantal vragen.");
                    return;
                }

                string omschrijving = ""; // Zorg voor een TextBox in je XAML hiervoor
                if (string.IsNullOrWhiteSpace(omschrijving)) omschrijving = $"Quiz {gekozenOnderwerp.OnderwerpNaam} - {DateTime.Now.ToShortDateString()}";

                // 2. Roep de manager aan
                QuizOpstellen aangemaakteQuiz = manager.GenereerRandomQuiz(gekozenOnderwerp, aantalVragen, omschrijving);

                // 3. Open het speelscherm en geef de Vragen uit de quiz mee
                QuizSpelen speelScherm = new QuizSpelen(manager, aangemaakteQuiz, userID);
                speelScherm.Show();

                this.Close(); // Sluit het optie scherm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er ging iets mis: {ex.Message}");
            }
        }
        

      
    }
}
