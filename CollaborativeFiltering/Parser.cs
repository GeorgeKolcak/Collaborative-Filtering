using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    static class Parser
    {
        public static Dictionary<int, int>[] Parse(int movie_count, out Dictionary<int, int> users)
        {
            string path;
            users = new Dictionary<int, int>();
            Dictionary<int, int>[] movies;

            movies = new Dictionary<int, int>[movie_count];

//            path = "training_set/mv_00";
            for (int i = 1; i <= movie_count; i++)
            {
                path = String.Format(@"training_set\mv_{0:0000000}.txt", i);
                movies[i - 1] = ParseSingle(path, users);
            }
            // A path is like this:
            //path = "training_set/mv_0017770.txt";

            return movies;
        }

        private static Dictionary<int, int> ParseSingle(string path, Dictionary<int, int> users)
        {
            string file_text;
            string[] content;
            Dictionary<int, int> new_movie;

            new_movie = new Dictionary<int, int>();


            // Read the file as one string.
            using (System.IO.StreamReader myFile = new System.IO.StreamReader(path))
            {
                // Scrapping the first line as it holds no interest (just the movie id, which we already have)
                myFile.ReadLine();

                while (myFile.EndOfStream == false)
                {
                    file_text = myFile.ReadLine();
                    if (String.IsNullOrWhiteSpace(file_text))
                        continue;

                    content = file_text.Split(',');
                    int netflixID = Convert.ToInt32(content[0]);

                    int user;
                    if (!users.TryGetValue(netflixID, out user))
                    {
                        user = users.Count;
                        users.Add(netflixID, user);
                    }

                    new_movie.Add(user, Convert.ToInt32(content[1]));
                }
            }

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

        private static int AddUser(LinkedList<User> users, User u)
        {
            LinkedListNode<User> current;

            if (users.Count == 0)
            {
                users.AddFirst(u);
                return 0;
            }

            current = users.First;
            while (current.Next != null)
            {
                // User not added if already present
                if (current.Value.NetflixID == u.NetflixID)
                    return current.Value.ID;

                if (current.Value.NetflixID > u.NetflixID)
                {
                    users.AddBefore(current, u);
                    return u.ID;
                }
                current = current.Next;
            }

            users.AddLast(u);
            return u.ID;
        }
    }
}
