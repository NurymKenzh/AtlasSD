using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class DollarRate
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal? Value { get; set; }
    }

    public class DollarRateIndexPageViewModel
    {
        public IEnumerable<DollarRate> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
