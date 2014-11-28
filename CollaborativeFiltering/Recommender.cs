using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    class Recommender
    {
        private Random random = new Random();

        private int featureCount;

        private int movieCount;
        private int userCount;

        private double[,] aMatrix;
        private double[,] bMatrix;

        private double ni;
        private double lambda;

        public Recommender(int featureCount, double ni, double lambda = 0.0)
        {
            this.featureCount = featureCount;

            this.ni = ni;
            this.lambda = lambda;
        }

        public void Learn(Dictionary<int, double>[] trainingData, int userCount)
        {
            movieCount = trainingData.Length;
            this.userCount = userCount;

            aMatrix = new double[userCount, featureCount];

            for (int i = 0; i < userCount; i++)
            {
                for (int j = 0; j < featureCount; j++)
                {
                    aMatrix[i, j] = random.NextDouble();
                }
            }

            bMatrix = new double[featureCount, movieCount];

            for (int i = 0; i < featureCount; i++)
            {
                for (int j = 0; j < movieCount; j++)
                {
                    bMatrix[i, j] = random.NextDouble();
                }
            }

            double oldError = Double.MaxValue;
            double error = 0.0;

            for (int i = 0; i < movieCount; i++)
            {
                foreach (int user in trainingData[i].Keys)
                {
                    error += Math.Pow((trainingData[i][user] - KSum(user, i)), 2);
                }
            }

            while (error < oldError)
            {
                oldError = error;
                error = 0.0;

                for (int i = 0; i < movieCount; i++)
                {
                    foreach (int u in trainingData[i].Keys)
                    {
                        error += Math.Pow((trainingData[i][u] - KSum(u, i)), 2);
                    }
                }

                int movieID = random.Next(movieCount);
                int user = trainingData[movieID].Keys.ElementAt(random.Next(trainingData[movieID].Count));

                double[] tempAData = new double[featureCount];
                double[] tempBData = new double[featureCount];

                for (int k = 0; k < featureCount; k++)
                {
                    tempAData[k] = (((1 - lambda) * aMatrix[user, k]) + (ni * bMatrix[k, movieID] * (trainingData[movieID][user] - KSum(user, movieID))));
                    tempBData[k] = (((1 - lambda) * bMatrix[k, movieID]) + (ni * aMatrix[user, k] * (trainingData[movieID][user] - KSum(user, movieID))));
                }

                for (int k = 0; k < featureCount; k++)
                {
                    aMatrix[user, k] = tempAData[k];
                    bMatrix[k, movieID] = tempBData[k];
                }
            }
        }

        public double PredictRating(int userID, int movieID)
        {
            return KSum(userID, movieID);
        }

        private double KSum(int userID, int movieID)
        {
            double sum = 0;

            for (int k = 0; k < featureCount; k++)
            {
                sum += (aMatrix[userID, k] * bMatrix[k, movieID]);
            }

            return sum;
        }
    }
}
