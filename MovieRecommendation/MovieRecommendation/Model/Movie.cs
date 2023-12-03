using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MovieRecommendation.Model
{
    public class Movie
    {
        public int MovieId;
        public string MovieTitle;
        private static string moviedatasetRelative = $"Data/ml-latest-small/movies.csv";
        private static string moviedatasetpath = moviedatasetRelative;
        public Lazy<List<Movie>> movies = new Lazy<List<Movie>>(() => 
        LoadMovieData(moviedatasetpath));

        private static List<Movie> LoadMovieData(string moviedatasetpath)
        {
            var result = new List<Movie>();
            Stream fileReader = File.OpenRead(moviedatasetpath);
            StreamReader reader = new StreamReader(fileReader);
            try
            {
                bool header = true;
                int index = 0;
                var line = "";
                while (!reader.EndOfStream)
                {
                    if (header)
                    {
                        line = reader.ReadLine();
                        header = false;
                    }
                    line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    int movieId = Int32.Parse(fields[0].ToString().TrimStart(new char[] { '0' }));
                    string movieTiltle = fields[1].ToString();
                    result.Add(new Movie() { MovieId = movieId, MovieTitle = movieTiltle });
                    index++;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return result;
        }
        public Movie()
        {

        }
        public Movie Get(int movieId)
        {
           return movies.Value.Single(m=> m.MovieId == movieId);
        }
   
    }
   
}
