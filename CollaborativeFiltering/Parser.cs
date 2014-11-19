using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    static class Parser
    {
        public static Dictionary<User, int>[] Parse(int movie_count, out int user_count)
        {
            string path;
            LinkedList<User> users;
            Dictionary<User, int>[] movies;

            users = new LinkedList<User>();
            movies = new Dictionary<User, int>[movie_count];

            path = "training_set/mv_00";
            for (int i = 1; i <= movie_count; i++)
            {
                movies[i - 1] = ParseSingle(path + FillMovieNumber(i), users);
            }
            // A path is like this:
            //path = "training_set/mv_0017770.txt";

            user_count = users.Count;
            return movies;
        }

        private static Dictionary<User, int> ParseSingle(string path, LinkedList<User> users)
        {
            string file_text;
            string[] content;
            User u;
            Dictionary<User, int> new_movie;

            new_movie = new Dictionary<User, int>();


            // Read the file as one string.
            System.IO.StreamReader myFile = new System.IO.StreamReader(path);

            // Scrapping the first line as it holds no interest (just the movie id, which we already have)
            myFile.ReadLine();

            while (myFile.EndOfStream == false)
            {
                file_text = myFile.ReadLine();
                if (String.IsNullOrWhiteSpace(file_text))
                    continue;

                content = file_text.Split(',');

                u = new User(Convert.ToInt32(content[0]), users.Count);
                AddUser(users, u);

                new_movie.Add(u, Convert.ToInt32(content[1]));
            }
            myFile.Close();

            return new_movie;
        }

        // We want a 5-character long string, so we fill it with 0's before the actual number
        private static string FillMovieNumber(int nb)
        {
            string path;

            path = nb.ToString();
            for (int i = 0; i < 5 - path.Length; i++)
            {
                path = "0" + path;
            }
            path = path + ".txt";

            return path;
        }

        private static void AddUser(LinkedList<User> users, User u)
        {
            LinkedListNode<User> current;

            if (users.Count == 0)
            {
                users.AddFirst(u);
                return;
            }

            current = users.First;
            while (current != users.Last)
            {
                // User not added if already present
                if (current.Value.Equals(u))
                    return;

                if (current.Value.NetflixID > u.NetflixID)
                {
                    users.AddBefore(current, u);
                    return;
                }
                current = current.Next;
            }

            if (current.Value.NetflixID > u.NetflixID)
                users.AddBefore(current, u);
            else
                users.AddLast(u);
        }
    }
}
