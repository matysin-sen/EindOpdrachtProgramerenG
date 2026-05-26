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
    /// Interaction logic for ExtraOnderwerpenToevoegen.xaml
    /// </summary>
    public partial class ExtraOnderwerpenToevoegen : Window
    {
        private Manager _manager;
        private ImportManager _importManager;
        public ExtraOnderwerpenToevoegen(ImportManager importManager, Manager manager)
        {
            InitializeComponent();
            _importManager = importManager;
            _manager = manager;
        }
        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOnderwerpNaam.Text))
            {
                MessageBox.Show("Vul een onderwerpnaam in!");
                return;
            }

            _manager.voegOnderwerpToe(txtOnderwerpNaam.Text.Trim());

            MessageBox.Show($"Onderwerp '{txtOnderwerpNaam.Text.Trim()}' succesvol toegevoegd!");
            txtOnderwerpNaam.Clear();
        }
    }
}
