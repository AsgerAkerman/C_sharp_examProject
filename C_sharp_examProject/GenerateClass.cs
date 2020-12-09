using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace C_sharp_examProject
{

    //Denne klasse står for at generate de forskellige skud og invaders, som skal op på skærmen.

    public partial class Play
    {
        //Først laves et nyt objekt af Play klassen for at kunne få adgang til dets xaml-elemnter.
        //Play playPage = new Play();

        int bulletsLeft = 40;
        int superBulletCounter = 0;
        int invaderTotal = 0;
        int invaderImages = 0;


        //Denne metode laver spillerens bullets ud fra en Rectangle, som derefter placeres lige over midten af spillermodellen.
        public void PlayerBullets()
        {
            if (bulletsLeft > 0)
            {
                Rectangle playerBullet = new Rectangle()
                {
                    Tag = "bullet",
                    Height = 15,
                    Width = 5,
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Yellow
                };
                Canvas.SetTop(playerBullet, Canvas.GetTop(playerModel) - playerBullet.Height);
                Canvas.SetLeft(playerBullet, Canvas.GetLeft(playerModel) + playerModel.Width / 2);

                playArea.Children.Add(playerBullet);
            }
        }

        //Ligesom ovenstående metode, så bliver der her genereret et skud fra spilleren, men her er det en anden for skud.
        public void SuperBullet()
        {
            Rectangle superBullet = new Rectangle()
            {
                Tag = "superBullet",
                Height = 15,
                Width = 100,
                Fill = Brushes.Red,
                Stroke = Brushes.Red
            };
            Canvas.SetTop(superBullet, Canvas.GetTop(playerModel) - superBullet.Height);
            Canvas.SetLeft(superBullet, Canvas.GetLeft(playerModel) + playerModel.Width / 2);

            playArea.Children.Add(superBullet);

            superBulletCounter = 0;
        }

        //Denne metode er næsten den samme som player bullets, men med forskellen at metoden tager to positioner for,
        //at skudene ikke spawner de samme steder hele tiden, men derimod sættes de til at følge efter spilleren.
        public void InvaderBullets(double x_Pos, double y_Pos)
        {
            Rectangle invaderBullet = new Rectangle()
            {
                Tag = "invaderBullet",
                Height = 30,
                Width = 10,
                Fill = Brushes.Green,
                Stroke = Brushes.Green
            };
            Canvas.SetTop(invaderBullet, y_Pos);
            Canvas.SetLeft(invaderBullet, x_Pos);

            playArea.Children.Add(invaderBullet);
        }

        //Denne metode står for at lave invaders og derefter sætte billederne fra images-mappen som 'skin'.
        public void GenerateInvaders(int startNumberInvaders)
        {
            int spaceForNewInvader = 0;
            invaderTotal = startNumberInvaders;

            for (int i = 0; i < startNumberInvaders; i++)
            {
                ImageBrush invaderPictures = new ImageBrush();

                Rectangle invaderModel = new Rectangle()
                {
                    Tag = "invader",
                    Height = 100,
                    Width = 75,
                    Fill = invaderPictures
                };

                Canvas.SetTop(invaderModel, 30);
                Canvas.SetLeft(invaderModel, spaceForNewInvader);

                playArea.Children.Add(invaderModel);

                spaceForNewInvader -= 100;
                invaderImages++;

                if (invaderImages > 2)
                {
                    invaderImages = 1;
                }

                if (invaderImages == 1)
                {
                    invaderPictures.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spacerinvader2.png"));
                }
                if (invaderImages == 2)
                {
                    invaderPictures.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/spaceinvader1.png"));
                }
            }

        }
    }
}
