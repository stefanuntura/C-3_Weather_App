using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    internal class NavSearch
    {
        // class to handle the Nav Bar search. Once this term gets updated, it updates classes that are observing it
        public string SearchTerm { get; set; }
    }
}
