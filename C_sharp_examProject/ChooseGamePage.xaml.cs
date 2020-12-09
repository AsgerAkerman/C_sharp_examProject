using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace C_sharp_examProject
{
    
    public partial class ChooseGamePage : Page
    {
        int userIdLocal;
        Datatransfer Datatransfer = new Datatransfer();
        public ChooseGamePage(int userId)
        {
            InitializeComponent();
            userIdLocal = userId;
                
            int highScore1 = Datatransfer.GetHighScore(userIdLocal, 1);
            Highscore1.Text = "Highscore: " + highScore1;


            int highScore2 = Datatransfer.GetHighScore(userIdLocal, 2);
            Highscore2.Text = "Highscore: " + highScore2;


            int highScore3 = Datatransfer.GetHighScore(userIdLocal, 3);
            Highscore3.Text = "Highscore: " + highScore3;


        }

        private void playSpaceInvader_onClick(object sender, RoutedEventArgs e)
        {
            Play playSpaceInvaders = new Play(userIdLocal);
            this.NavigationService.Navigate(playSpaceInvaders, userIdLocal);

        }

        private void playBubbleTrouble_onClick(object sender, RoutedEventArgs e)
        {
            BubbleTrouble bubbleTrouble = new BubbleTrouble(userIdLocal);

            this.NavigationService.Navigate(bubbleTrouble, userIdLocal);
        }

        private void playEpicRacing_onClick(object sender, RoutedEventArgs e)
        {
            EpicRacing epicRacing = new EpicRacing(userIdLocal);
            this.NavigationService.Navigate(epicRacing);
            
        }
        
    }
}
