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

namespace C_sharp_examProject
{
    public partial class Login : Page
    {
        public int userId;
        bool isLoggedIn = false;
        

        public Login()
        {
            InitializeComponent();
        }

        private void loginButton_onClick(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=yndlingsfilm.database.windows.net;Initial Catalog=yndlingsfilmDB;User ID=s174879;Password=Markus98;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False");

            User newUser = new User();
            String sqlQuery = "SELECT * FROM Users WHERE Username=@Username AND Password=@Password";
            SqlDataReader reader = null;

            try
            {
                SqlConnection conn = new SqlConnection(@"Data Source=yndlingsfilm.database.windows.net;Initial Catalog=yndlingsfilmDB;User ID=s174879;Password=Markus98;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False");         
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@Username", loginUsername.Text);
                cmd.Parameters.AddWithValue("@Password", loginPassword.Password);
                try
                {
                    conn.Open();
    
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userId = (int)reader["user_id"];
                        isLoggedIn = true;
                        Console.WriteLine(userId); 
                    }
                }

                finally
                {
                    //lukker connection nedenfor, når readeren er done
                    if (reader != null)
                    {
                        reader.Close();
                    }
             
                    if (conn != null)
                    {
                        conn.Close();
                    }

                    if (isLoggedIn)
                    {
                        ChooseGamePage chooseGame = new ChooseGamePage(userId);
                        this.NavigationService.Navigate(chooseGame);
                    }
                    else
                    {
                        MessageBox.Show("Wrong username or password!");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
