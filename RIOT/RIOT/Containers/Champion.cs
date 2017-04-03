using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT.Containers
{
    public class Champion
    {
        public string name;
        public int id;

        public Champion(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
