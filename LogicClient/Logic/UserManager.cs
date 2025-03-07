using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{

    public class User
    {
        public int _id;
        public string _name;
        public bool _isAI;

        public void InitUser(int id, string nickname)
        {
            _id = id;
            _name = nickname;
            _isAI = false;
        }
    }

    public class UserManager
    {
        public List<User> _users = new List<User>();

        public void AddUser(User newUser)
        {
            _users.Add(newUser);
        }
    }
}
