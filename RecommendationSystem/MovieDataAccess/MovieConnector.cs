using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MovieModel;

namespace MovieDataAccess
{
    public class MovieConnector: Connector
    {
        public List<Movie> GetAllMovies()
        {
            List<Movie> list = new List<Movie>();
            string sql = "select * from Movie";
            open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                Movie movie = new Movie();
                movie.MovieId = reader.GetInt32(0);
                movie.Title = reader.GetString(1);
                movie.Genres = reader.GetString(2);
                list.Add(movie);

            }
            reader.Close();
            close();
            return list;
        }
        public Movie GetMovie(int id)
        {
            string sql = "select * from Movie where MovieId = @movieId";
            SqlParameter parMovieId = new SqlParameter("@movieId", SqlDbType.Int);
            parMovieId.Value = id;
            Movie movie = null;
            open();
            SqlDataReader reader = Query(sql, new[] {parMovieId});
            if (reader.Read())
            {
                movie = new Movie();
                movie.MovieId = reader.GetInt32(0);
                movie.Title = reader.GetString(1);
                movie.Genres = reader.GetString (2);
            }
            reader.Close();
            close();
            return movie;

        }
    }
}
