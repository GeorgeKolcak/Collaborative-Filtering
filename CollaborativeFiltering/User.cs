using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    struct User
    {
        public int NetflixID { get; private set; }

        public int ID { get; set; }

        public User(int netflixID, int id)
            : this()
        {
            NetflixID = netflixID;
            ID = id;
        }

        public override int GetHashCode()
        {
            return (233 + (31 * ID.GetHashCode()));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is User))
            {
                return false;
            }

            User other = (User)obj;

            return (ID == other.ID);
        }
    }
}
