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
using System.Windows.Threading;

namespace C_sharp_examProject
{
    /// <summary>
    /// Interaction logic for BubbleTrouble.xaml
    /// </summary>
    public partial class BubbleTrouble : Page
    {

        bool gameOver = false;
        bool goLeft;
        bool goRight;
        int ballSpeed = 5;
        int ballsHit = 0;
        bool ballWasHit;
        double ballCoordinateX;
        double ballCoordinateY;
        double ballSize = 160;
        bool shotExists = false;
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        Datatransfer datatransfer = new Datatransfer();
        DispatcherTimer timer = new DispatcherTimer();
        ImageBrush playerImage = new ImageBrush();
        int userIdLocal;


        public BubbleTrouble(int userId)
        {
            InitializeComponent();
            userIdLocal = userId;

            timer.Tick += GameLoop;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
    
            InitBall(Canvas.GetLeft(player) + 20, 10, ballSize, "isMovingRight");

            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceinvader1.png"));
            player.Fill = playerImage;

            myCanvas.Focus();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 5, player.Height);
            playerHitBox.Height = player.Height;
            playerHitBox.Width = player.Width;

            score.Content = "Score: " + ballsHit;
            ballWasHit = false;
            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                Rect screenBorder = new Rect(Canvas.GetLeft(myCanvas), Canvas.GetTop(myCanvas), myCanvas.Width, myCanvas.Height);
                if (x is Rectangle && (string)x.Tag == "shot")
                {
                    // bool her der ikke tallader nye shots før denne er væk
                    shotExists = true;
                    x.Height += 20;
                    Canvas.SetTop(x, Canvas.GetTop(x) - 10);

                    if (Canvas.GetTop(x) < 1)
                    {
                        itemsToRemove.Add(x);
                        shotExists = false;
                    }

                    Rect shot = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && ((string)y.Tag == "ballIsMovingUp" || (string)y.Tag == "ballIsMovingDown"))
                        {
                            Rect ball = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);


                            if (shot.IntersectsWith(ball))
                            {
                                ballWasHit = true;

                                // kontrol om den bliver mindre herinde. 
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                shotExists = false;
                                ballSize = y.Width;
                                ballCoordinateX = Canvas.GetLeft(y);
                                ballCoordinateY = Canvas.GetTop(y);

                                ballsHit += 1;
                                if (ballsHit >= 31)
                                {
                                    ShowGameOver("You Won!");
                                }
                            }
                        }
                    }
                }
                // her styres boldens bevægelser
                if (x is Rectangle && (string)x.Tag == "ballIsMovingUp" || x is Rectangle && (string)x.Tag == "ballIsMovingDown")
                {
                    Rect ball = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (x.Name == "isMovingRight")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ballSpeed);
                        if (Canvas.GetLeft(x) + x.Width >= 1000)
                        {

                            x.Name = "isMovingLeft";
                        }
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ballSpeed);

                        if (Canvas.GetLeft(x) <= 0)
                        {
                            x.Name = "isMovingRight";
                        }
                    }

                    if ((string)x.Tag == "ballIsMovingUp")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - ballSpeed);
                        if (Canvas.GetTop(x) - (x.Height / 2) - 50 < x.Height / 2)
                        {
                            x.Tag = "ballIsMovingDown";
                        }
                    }
                    else
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + ballSpeed);
                        if (Canvas.GetTop(x) + x.Height + 30 > Application.Current.MainWindow.Height)
                        {


                            x.Tag = "ballIsMovingUp";
                        }
                    }
                    Rect ballHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (playerHitBox.IntersectsWith(ballHitBox))
                    {
                        ShowGameOver("GameOver");
                    }
                }
            }
            if (ballWasHit && ballSize > 10)
            {
                InitBall(ballCoordinateX, ballCoordinateY, ballSize / 2, "isMovingRight");
                InitBall(ballCoordinateX, ballCoordinateY, ballSize / 2, "isMovingLeft");
            }
            foreach (Rectangle shot in itemsToRemove)
            {
                myCanvas.Children.Remove(shot);
            }

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += KeyIsDown;
            window.KeyUp += KeyIsUp;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }
            if (e.Key == Key.Space && !shotExists)
            {
                Rectangle shot = new Rectangle
                {
                    Tag = "shot",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };
                Canvas.SetTop(shot, Canvas.GetTop(player) - shot.Height);
                Canvas.SetLeft(shot, Canvas.GetLeft(player) + player.Width / 2);
                myCanvas.Children.Add(shot);
            }
            if (e.Key == Key.Enter && gameOver == true)
            {
                gameOver = false;
                ChooseGamePage chooseGame = new ChooseGamePage(userIdLocal);
                this.NavigationService.Navigate(chooseGame);
            }
        }

        private void InitBall(double x, double y, double size, String name)
        {
            ImageBrush ballSkin = new ImageBrush();
            ballSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/circle.png"));
            Rectangle ball = new Rectangle
            {
                Tag = "ballIsMovingUp",
                Height = size,
                Width = size,
                Fill = ballSkin,
                Name = name
            };

            Canvas.SetTop(ball, y);
            Canvas.SetLeft(ball, x);
            myCanvas.Children.Add(ball);
        }

        private void ShowGameOver(String message)
        {
            gameOver = true;
            timer.Stop();
            score.Content += " " + message + " Press enter to play again";
            datatransfer.addHighScore(userIdLocal,1,ballsHit);
            
        }
    }
}

