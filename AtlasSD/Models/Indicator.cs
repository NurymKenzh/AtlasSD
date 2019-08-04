using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class Indicator
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Type")]
        public string Type { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Group")]
        public int? GroupId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Group")]
        public Group Group { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Code")]
        public string Code { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Formula")]
        public string Formula { get; set; }
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
        public string NameCode
        {
            get
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return Name;
                }
                else
                {
                    return Name + $" ({Code})";
                }
            }
        }
        public string CodeName
        {
            get
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return Name;
                }
                else
                {
                    return $"{Code}. " + Name;
                }
            }
        }
    }

    public class IndicatorIndexPageViewModel
    {
        public IEnumerable<Indicator> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
