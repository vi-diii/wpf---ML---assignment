using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieModel;
using System.Data.SqlClient;

namespace MovieDataAccess
{
    public class MovieRatingConnector  : Connector
    {
        public List<MovieRating> GetAllMovieRating()
        {
            List<MovieRating> list = new List<MovieRating>();
            string sql = "select * from MovieRating";
            open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                MovieRating rating = new MovieRating();
                rating.UserId = reader.GetInt32(1);
                rating.MovieID = reader.GetInt32(2);
                object obj = reader.GetValue(3);
                rating.Label = float.Parse(obj.ToString());
                list.Add(rating);
            }
            reader.Close();
            close();
            return list;
        }
    }
}
