using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    class Recommender
    {
        private int featureCount;

        private double[,] aMatrix;
        private double[,] bMatrix;

        public Recommender(int featureCount)
        {
            this.featureCount = featureCount;
        }

        public void Learn()
        {

        }

        public int PredictRating(int userID, int movieID)
        {
            double rating = 0;

            for (int k = 0; k < featureCount; k++)
            {
                rating += (aMatrix[userID, k] * bMatrix[k, movieID]);
            }

            return (int)Math.Round(rating);
        }
    }
}
