using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT.Containers
{
    public class Item
    {
        public int id;
        public string name;

        public Item(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
