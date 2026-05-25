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
        private IVragenRepository repository;
        
        public QuizOptiesxaml(Manager manager)
        {
            InitializeComponent();
            this.manager = manager;
            cmbBoxOnderwerp.ItemsSource = manager.GeefAlleOnderwerpen();
        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBoxOnderwerp.SelectedItem == null)
            {
                MessageBox.Show("Selecteer eerst een onderwerp.");
                return;
            }

            // 2. Nu kunnen we het veilig omzetten (casten) naar een écht Onderwerpen object
            Onderwerpen gekozenOnderwerp = (Onderwerpen)cmbBoxOnderwerp.SelectedItem;

            // 3. Haal het aantal vragen op
            int aantalvragen = AantalVragen.Value.HasValue ? (int)AantalVragen.Value.Value : 0;

            if (aantalvragen <= 0)
            {
                MessageBox.Show("Voer een geldig aantal vragen in (groter dan 0).");
                return;
            }

            // 4. Open het nieuwe scherm en geef het échte object door in plaats van een string
            // Let op: pas de constructor in QuizSpelen.xaml.cs aan zodat hij een 'Onderwerpen' object verwacht!
            QuizSpelen quizSpelen = new QuizSpelen(manager, gekozenOnderwerp, aantalvragen);
            quizSpelen.Show();

            // Optioneel: sluit dit huidige keuzescherm af
            this.Close();
        }

        private void cmbBoxOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
