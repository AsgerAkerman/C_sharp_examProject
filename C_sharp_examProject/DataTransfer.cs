using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_sharp_examProject
{

    class Datatransfer
    {
        public void addHighScore(int userid, int gameid, int highscore)
        {

            SqlConnection sqlConnection = new SqlConnection(@"Data Source=yndlingsfilm.database.windows.net;Initial Catalog=yndlingsfilmDB;User ID=s174879;Password=Markus98;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False");

            String sqlQuery = "INSERT INTO highscore (user_id,game_id,score) VALUES (@user_id,@game_id,@score)";
            using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
            {
                Console.WriteLine(userid + " " + gameid + " " + highscore);
                command.Parameters.AddWithValue("@user_id", userid);
                command.Parameters.AddWithValue("@game_id", gameid);
                command.Parameters.AddWithValue("@score", highscore);

                sqlConnection.Open();
                int result = command.ExecuteNonQuery();
                if (result < 0)
                    Console.WriteLine("Error inserting data into Database!");
            }
        }

        public int GetHighScore(int userId, int gameId)
        {

            SqlDataReader reader = null;
            int maxScore = 0;
            String sqlQuery = "SELECT MAX(score) as max_score FROM highscore WHERE game_id = @game_id AND user_id = @user_id";
            SqlConnection conn = new SqlConnection(@"Data Source=yndlingsfilm.database.windows.net;Initial Catalog=yndlingsfilmDB;User ID=s174879;Password=Markus98;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False");

            try
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@game_id", gameId);
                cmd.Parameters.AddWithValue("@user_id", userId);
                try
                {
                    conn.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                       maxScore = (int)reader["max_score"];
                       Console.WriteLine(maxScore);
                   
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

                }
            }
            catch (Exception exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
            return maxScore;
        } 
    }
}
