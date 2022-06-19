using Baker_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baker_Project.ViewModels
{
    public class HomeVM
    {
        public List<Design> Designs { get; set; }
        public List<HomePageProducts> HomePageProducts { get; set; }
        public List<Category> Categories { get; set; }
    }
}
