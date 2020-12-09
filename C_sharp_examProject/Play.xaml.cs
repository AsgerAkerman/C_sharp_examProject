using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Threading;

namespace C_sharp_examProject
{
   >
    public partial class Play : Page
    {

        /*
         * Nedenfor sættes alle variablerne for antal invaders, deres funktioner som hastigheden, hvor ofte de skyder 
         * efters spilleren osv.
         * Desuden sættes her styring for om spilleren bevæger sig og om spillet er slut.
         */

        int invaderSpeed = 5;
        int invadersKilled = 0;
        int shotReset = 100;
        int shotFrequency = 0;
        int highscore = 0;
        int userIdLocal;

        bool moveLeft, moveRight = false;
        bool gameOver = false;
        Datatransfer dataTransfer = new Datatransfer();


        //Spillets timer
        DispatcherTimer gameTime = new DispatcherTimer();

        //Dette bruges til at sætte billedet for spilleren
        ImageBrush playerImage = new ImageBrush();

        //Indeholder en liste over items(invaders, bullets), der ikke skal bruges på spillepladen og derfor skal fjernes
        List<Rectangle> removeItems = new List<Rectangle>();

        //Her sættes spillet op og startet. Dette indebærer, at der her sættes hvor ofte spillet opfatte ændringer vha. gameTime.
        public Play(int userId)
        {
            gameOver = false;
            InitializeComponent();
            userIdLocal = userId;

            //Sætter dispatchTimer til game metoden, da den skal styre spillets timer
            gameTime.Tick += Game;

            //Sætter timeren til at køre hvert 25 milisekunder og derefter startes timeren med .Start().
            gameTime.Interval = TimeSpan.FromMilliseconds(20);
            gameTime.Start();

            //Her sættes spilleres billede/model til at være billedet, som er importeret ind i images mappen.
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spacerinvader3.png"));
            playerModel.Fill = playerImage;

            playArea.Focus();

            GenerateInvaders(25);
        }


        //Denne metode står for at styre spillet, som bliver opdateret (kørt) hver gang gameTime runder 20 milisek.
        private void Game(object sender, EventArgs e)
        {
            //Sætter regel for at spilleren ikke kan bevæge sig ud over spille-arealet til højre og venstre.
            if (moveLeft == true && Canvas.GetLeft(playerModel) > 0)
            {
                Canvas.SetLeft(playerModel, Canvas.GetLeft(playerModel) - 10);
            }
            else if (moveRight == true && Canvas.GetLeft(playerModel) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(playerModel, Canvas.GetLeft(playerModel) + 10);
            }

            //Styrer hvor ofte der skal skydes efter spilleren.
            shotFrequency -= 2;
            if (shotFrequency <= 0)
            {
                InvaderBullets(Canvas.GetLeft(playerModel) + 20, 10);
                shotFrequency = shotReset;
            }

            //Sætter hitbox for spilleren, som skal bruges til at tjekke om spilleren rammes af et skud.
            //Det fungerer ligesom en 'usynlig' box der har samme størrelse om spilleren.
            Rect playerHitbox = new Rect(Canvas.GetLeft(playerModel), Canvas.GetTop(playerModel), playerModel.Width, playerModel.Height);

            invadersHit.Content = "Invaders hit: " + invadersKilled;

            //De næste to loops er med til at gøre at både invaders og spilleres skud bevæger sig og deres position ikke er faste.
            //Desuden gøres således at skud og invaders fjernes hvis de rammer hinanden eller ryger ud fra playArea.
            foreach (var x in playArea.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet" || (string)x.Tag == "superBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 15);
                    if (Canvas.GetTop(x) < 5)
                    {
                        removeItems.Add(x);
                    }
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var a in playArea.Children.OfType<Rectangle>())
                    {
                        if (a is Rectangle && (string)a.Tag == "invader")
                        {
                            Rect invaderKill = new Rect(Canvas.GetLeft(a), Canvas.GetTop(a), a.Width, a.Height);

                            if (bullet.IntersectsWith(invaderKill))
                            {
                                removeItems.Add(a);
                                removeItems.Add(x);
                                invaderTotal -= 1;
                                invadersKilled += 1;

                                superBulletCounter += 1;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "invader")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + invaderSpeed);
                    if (Canvas.GetLeft(x) > 1000)
                    {
                        Canvas.SetLeft(x, -85);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 50));
                    }
                    Rect invaderHitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitbox.IntersectsWith(invaderHitbox))
                    {
                        IsGameOver("Du har sgu tabt. Tryk på space for at komme tilbage");
                    }
                }

                if (x is Rectangle && (string)x.Tag == "invaderBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 7);
                    if (Canvas.GetTop(x) > 600)
                    {
                        removeItems.Add(x);
                    }
                    Rect invaderBulletHitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitbox.IntersectsWith(invaderBulletHitbox))
                    {
                        IsGameOver("Du har sgu tabt. Tryk på space for at komme tilbage");
                    }
                }

            }

            foreach (Rectangle i in removeItems)
            {
                playArea.Children.Remove(i);
            }

            if (invaderTotal < 1)
            {
                IsGameOver("Du har sgu tabt. Tryk på space for at komme tilbage");

            }

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += KeyPressed;
            window.KeyUp += KeyReleased;
        }


        //Denne metode styrer spillerens bevægelse til venstre og højre når piletasterne trykkes ned.
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }


        //Denne metode stopper spillerens bevægelse til venstre og højre når piletasterne slippes.
        //Samt er det metoden for at tjekke om spilleren skyder vha. spacebaren.
        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if(e.Key == Key.Space)
            {
                if (gameOver == true)
                {
                    gameOver = false;
                    ChooseGamePage chooseGame = new ChooseGamePage(userIdLocal);
                    this.NavigationService.Navigate(chooseGame);
                }
                if (superBulletCounter == 10)
                {
                    SuperBullet();
                }
                else
                {
                    PlayerBullets();
                }

                bulletsLeft -= 1;
                if (bulletsLeft < 0)
                {
                    shotsLeft.Content = "No more shots!!!";
                }
                else
                {
                    shotsLeft.Content = "Shots left: " + bulletsLeft;
                }
            }
        }

        

        private void IsGameOver(String msg)
        {
            gameOver = true;
            highscore = (bulletsLeft * 2) + invadersKilled;
            Highscore.Content = "Highscore: " + highscore;
            gameOverText.Text = msg;
      
            dataTransfer.addHighScore(userIdLocal,2,highscore);
            gameTime.Stop();
        }
    }
}
