using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class EpicRacing : Page
    {

        bool goLeft, goRight = false;

        List<Object> itemstoremove = new List<Object>();
        Datatransfer datatransfer = new Datatransfer();


        //timer
        int obstacleOneTimer;
        int stripeTimer;
        int obstacleTwoTimer;
        Boolean levelTwo = false;
        Boolean levelThree = false;
        Boolean gameOver = false;
        //limit
        int obstacleOneTime = 90;
        int obstacleTwoTime = 140;
        int stripeTime = 30;
        Random rnd = new Random();
        DispatcherTimer disTimer = new DispatcherTimer();
        ImageBrush playerImage = new ImageBrush();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;
        int userIdLocal;


        public EpicRacing(int userId)
        {
            InitializeComponent();
            userIdLocal = userId;
           

            disTimer.Tick += gameEngine;
            disTimer.Interval = TimeSpan.FromMilliseconds(10);
            disTimer.Start();

            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/EpicRaceCar.png"));
            player1.Fill = playerImage;

        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            int userId = (int)e.ExtraData;
        }
        //nedenstående bruges til at attache vores keys til window. Der kan være fejl når man bruger pages i hvor god respons der er på keys.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += Canvas_ButtonDown;
            window.KeyUp += Canvas_ButtonUp;
        }


        private void Canvas_ButtonDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left)
            {
                Console.WriteLine(goLeft);
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                Console.WriteLine(goRight);
                goRight = true;
            }
            if (e.Key == Key.Space)
            {
                if (gameOver == true)
                {
                    gameOver = false;
                    ChooseGamePage chooseGame = new ChooseGamePage(userIdLocal);
                    this.NavigationService.Navigate(chooseGame);
                }

            }
        }

        private void Canvas_ButtonUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Console.WriteLine(goLeft);
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                Console.WriteLine(goRight);
                goRight = false;
            }

            
        }

        private void obstacleOneCreator(double x, double y)
        {
            Rectangle obstacleOne = new Rectangle
            {
                Tag = "obstacleOne",
                Height = 10,
                Width = 30,
                Fill = Brushes.DarkGray,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 2

            };

            Canvas.SetTop(obstacleOne, y);
            Canvas.SetLeft(obstacleOne, x);
            Canvas.SetZIndex(obstacleOne, 1);
            myCanvas.Children.Add(obstacleOne);
        }
        private void obstacleTwoCreator(double x, double y)
        {
            Rectangle obstacleTwo = new Rectangle
            {
                Tag = "obstacleTwo",
                Height = 30,
                Width = 30,
                Fill = Brushes.DarkSlateGray,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 2

            };

            Canvas.SetTop(obstacleTwo, y);
            Canvas.SetLeft(obstacleTwo, x);
            Canvas.SetZIndex(obstacleTwo, 1);
            myCanvas.Children.Add(obstacleTwo);
        }
        private void stripeCreator(double x, double y)
        {
            Rectangle newStripe = new Rectangle
            {
                Tag = "stripe",
                Height = 30,
                Width = 10,
                Fill = Brushes.White

            };
            Canvas.SetTop(newStripe, y);
            Canvas.SetLeft(newStripe, x);
            Canvas.SetZIndex(newStripe, 0);
            myCanvas.Children.Add(newStripe);
        }

        private void gameEngine(object sender, EventArgs e)
        {


            Rect car = new Rect(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);

            if (stopWatch.IsRunning == false)
            {
                stopWatch.Start();
                disTimer.Start();
            }
            TimeSpan ts = stopWatch.Elapsed;
            currentTime = String.Format("{0:00}:{1:00}:{2:00}",
            ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Time.Content = currentTime;


            if (goLeft && Canvas.GetLeft(player1) > 0)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) - 5);
            }

            else if (goRight && Canvas.GetLeft(player1) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) + 5);
            }

            obstacleOneTimer -= 3;
            if (obstacleOneTimer < 0)
            {

                double leftOne = rnd.Next(0, 500);
                obstacleOneCreator(leftOne, 0);
                obstacleOneTimer = obstacleOneTime;
            }
            stripeTimer -= 3;
            if (stripeTimer < 0)
            {
                stripeCreator(500 / 2, 0);

                stripeTimer = stripeTime;
            }


            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {

                // Leder efter obstacles og stripes, for at smide dem en omgang ned
                if (x is Rectangle && (string)x.Tag == "obstacleOne" || x is Rectangle && (string)x.Tag == "obstacleTwo")
                {
                    if ((x is Rectangle && (string)x.Tag == "obstacleOne"))
                    {
                        //smider dem en gang ned
                        if (levelThree == true)
                        {
                            Canvas.SetTop(x, Canvas.GetTop(x) + 10);
                        }
                        else
                        {
                            Canvas.SetTop(x, Canvas.GetTop(x) + 2);
                        }
                    }
                    else
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + 2);

                    }

                    //hvis den er ude af billedet smider vi den ud
                    if (Canvas.GetTop(x) > 600)
                    {
                        itemstoremove.Add(x);

                    }

                    //laver en lokal rectangle så vi kan tjekke om den rammer med playeren
                    Rect obstacleOne = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                    if (obstacleOne.IntersectsWith(car))

                    {
                        gameOver = true;
                        disTimer.Stop();
                        stopWatch.Stop();
                        gameOverText.Text = "GAME OVER " + currentTime + "  Press spacebar to go back";
                        datatransfer.addHighScore(userIdLocal, 3, ts.Seconds);

                        
                    }

                }
                if (x is Rectangle && (string)x.Tag == "stripe")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 5);

                    //hvis den er ude af billedet smider vi den ud
                    if (Canvas.GetTop(x) > 600)
                    {
                        itemstoremove.Add(x);

                    }

                }


            }


            ///////////////////////// //LEVEL 2///////////////////////////////////////////
            obstacleTwoTimer -= 3;

            if (ts.Seconds > 10 && obstacleTwoTimer < 0)
            {

                double left = rnd.Next(0, 500);
                obstacleTwoCreator(left, 0);
                obstacleTwoTimer = obstacleTwoTime;

            }

            if (levelTwo == false && ts.Seconds > 10)
            {
                myCanvas.Background = new SolidColorBrush(Colors.Blue);
                levelTwo = true;
                Level.Content = "Level: 2";

            }   

            if (ts.Seconds > 15 && levelThree == false)
            {
                myCanvas.Background = new SolidColorBrush(Colors.Black);
                levelThree = true;
                Level.Content = "Level: 3";
            }

            //Ryder op
            foreach (Rectangle a in itemstoremove)
            {
                myCanvas.Children.Remove(a);
            }

        }
      
    }
}
