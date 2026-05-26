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
    /// Interaction logic for ScoreBekijken.xaml
    /// </summary>
    public partial class ScoreBekijken : Window
    {
        private Manager Manager;
        private int userID;

        public ScoreBekijken(Manager manager, int userID)
        {
            InitializeComponent();
            this.Manager = manager;
            this.userID = userID;

            LaadScores();
        }

        private void LaadScores()
        {
            dgScores.ItemsSource = Manager.GeefScoresVoorUser(userID);
        }
    }
}
