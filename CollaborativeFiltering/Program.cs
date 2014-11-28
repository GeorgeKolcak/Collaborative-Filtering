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

            Dictionary<int, int> users;

            Dictionary<int, int>[] ratings = Parser.Parse(movieCount, out users);

            int[] movieRatingCount = new int[movieCount];
            double[] movieRatingSums = new double[movieCount];

            int[] userRatingCount = new int[users.Count];
            double[] userRatingSums = new double[users.Count];

            int allRatingCount = 0;
            double allRatingSum = 0;

            Dictionary<int, double>[] normalisedRatings = new Dictionary<int,double>[movieCount];

            for (int i = 0; i < movieCount; i++)
            {
                foreach (int user in ratings[i].Keys)
                {
                    movieRatingCount[i]++;
                    movieRatingSums[i] += ratings[i][user];

                    userRatingCount[user]++;
                    userRatingSums[user] += ratings[i][user];

                    allRatingCount++;
                    allRatingSum += ratings[i][user];
                }
            }

            for (int i = 0; i < movieCount; i++)
            {
                normalisedRatings[i] = new Dictionary<int, double>();

                foreach(int user in ratings[i].Keys)
                {
                    normalisedRatings[i].Add(user, (ratings[i][user] - (userRatingSums[user] / userRatingCount[user]) - (movieRatingSums[i] / movieRatingCount[i]) + (allRatingSum / allRatingCount)));
                }
            }

            Recommender recommender = new Recommender(10, 0.001);
            recommender.Learn(normalisedRatings, users.Count);

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
                        movieID = (Int32.Parse(line.Substring(0, (line.Length - 1))) - 1);

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
                if (movieID >= movieCount)
                {
                    continue;
                }

                foreach (int userID in testData[movieID])
                {
                    score += Math.Pow((ratings[movieID][users[userID]] - (recommender.PredictRating(users[userID], movieID) + (movieRatingSums[movieID] / movieRatingCount[movieID]) +
                        (userRatingSums[users[userID]] / userRatingCount[users[userID]]) - (allRatingSum / allRatingCount))), 2);

                    //foreach (int user in ratings[movieID].Keys)
                    //{
                    //    if (users[userID] == user)
                    //    {
                            

                    //        break;
                    //    }
                    //}
                }
            }

            score = Math.Sqrt(score / testDataCount);

            Console.WriteLine(score);
            Console.ReadKey();
        }
    }
}
