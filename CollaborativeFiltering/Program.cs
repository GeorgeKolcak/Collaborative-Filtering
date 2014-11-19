using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    class Program
    {
        static void Main(string[] args)
        {
            int movieCount = Int32.Parse(Console.ReadLine());
            int userCount;

            Dictionary<User, int>[] ratings = Parser.Parse(movieCount, out userCount);

            int[] movieRatingCount = new int[movieCount];
            double[] movieRatingSums = new double[movieCount];

            int[] userRatingCount = new int[userCount];
            double[] userRatingSums = new double[userCount];

            int allRatingCount = 0;
            double allRatingSum = 0;

            Dictionary<User, double>[] normalisedRatings = new Dictionary<User,double>[movieCount];

            for (int i = 0; i < movieCount; i++)
            {
                foreach (User user in ratings[i].Keys)
                {
                    movieRatingCount[i]++;
                    movieRatingSums[i] += ratings[i][user];

                    userRatingCount[user.ID]++;
                    userRatingSums[user.ID] += ratings[i][user];

                    allRatingCount++;
                    allRatingSum += ratings[i][user];
                }
            }

            for (int i = 0; i < movieCount; i++)
            {
                normalisedRatings[i] = new Dictionary<User, double>();

                foreach(User user in ratings[i].Keys)
                {
                    normalisedRatings[i].Add(user, (ratings[i][user] - (userRatingSums[user.ID] / userRatingCount[user.ID]) - (movieRatingSums[i] / movieRatingCount[i]) + (allRatingSum / allRatingCount)));
                }
            }

            Recommender recommender = new Recommender(10, 0.001);
            recommender.Learn(normalisedRatings, userCount);

            int testDataCount = 0;
            Dictionary<int, List<int>> testData = new Dictionary<int, List<int>>();

            using (StreamReader sr = new StreamReader("probe.txt"))
            {
                string line;

                int movieID = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line.EndsWith(":"))
                    {
                        movieID = Int32.Parse(line.Substring(0, (line.Length - 1)));

                        testData.Add(movieID, new List<int>());
                    }
                    else
                    {
                        testDataCount++;
                        testData[movieID].Add(Int32.Parse(line));
                    }
                }
            }

            double score = 0;

            foreach (int movieID in testData.Keys)
            {
                foreach (int userID in testData[movieID])
                {
                    foreach (User user in ratings[movieID].Keys)
                    {
                        if (user.NetflixID == userID)
                        {
                            score += Math.Pow((ratings[movieID][user] - (recommender.PredictRating(user.ID, movieID) + (movieRatingSums[movieID] / movieRatingCount[movieID]) +
                                (userRatingSums[user.ID] / userRatingCount[user.ID]) - (allRatingSum / allRatingCount))) ,2);

                            break;
                        }
                    }
                }
            }

            score = Math.Sqrt(score / testDataCount);

            Console.WriteLine(score);
            Console.ReadKey();
        }
    }
}
