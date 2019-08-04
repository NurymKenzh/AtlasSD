using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class Region
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Code")]
        public string Code { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Area")]
        public decimal Area { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Population")]
        public int Population { get; set; }
        public string Coordinates { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = "";
                if (language == "en")
                {
                    name = NameEN;
                }
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "ru")
                {
                    name = NameRU;
                }
                return name;
            }
        }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string NameCode
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = "";
                if (language == "en")
                {
                    name = NameEN + $" ({Code})";
                }
                if (language == "kk")
                {
                    name = NameKK + $" ({Code})";
                }
                if (language == "ru")
                {
                    name = NameRU + $" ({Code})";
                }
                return name;
            }
        }
    }

    public class RegionIndexPageViewModel
    {
        public IEnumerable<Region> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
