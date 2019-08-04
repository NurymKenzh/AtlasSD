using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class Map
    {
        public int Id { get; set; }
        [NotMapped]
        public int? BlocId { get; set; }
        [NotMapped]
        public int? GroupId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public int IndicatorId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public Indicator Indicator { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DescriptionEN")]
        [DataType(DataType.MultilineText)]
        public string DescriptionEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DescriptionKK")]
        [DataType(DataType.MultilineText)]
        public string DescriptionKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DescriptionRU")]
        [DataType(DataType.MultilineText)]
        public string DescriptionRU { get; set; }
        [NotMapped]
        public List<MapInterval> MapIntervals { get; set; }
        [Display(Name = "MinValue")]
        public decimal[] MinValues { get; set; }
        [Display(Name = "Color")]
        public string[] Colors { get; set; }
        public int[] IndicatorIds { get; set; }
        [NotMapped]
        public Indicator[] Indicators { get; set; }
        [NotMapped]
        public List<MapIndicator> MapIndicators { get; set; }
        public string[] IndicatorColors { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KeywordsEN")]
        [DataType(DataType.MultilineText)]
        public string KeywordsEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KeywordsKK")]
        [DataType(DataType.MultilineText)]
        public string KeywordsKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KeywordsRU")]
        [DataType(DataType.MultilineText)]
        public string KeywordsRU { get; set; }
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
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    Description = "";
                if (language == "en")
                {
                    Description = DescriptionEN;
                }
                if (language == "kk")
                {
                    Description = DescriptionKK;
                }
                if (language == "ru")
                {
                    Description = DescriptionRU;
                }
                return Description;
            }
        }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Keywords")]
        public string Keywords
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    Keywords = "";
                if (language == "en")
                {
                    Keywords = KeywordsEN;
                }
                if (language == "kk")
                {
                    Keywords = KeywordsKK;
                }
                if (language == "ru")
                {
                    Keywords = KeywordsRU;
                }
                return Keywords;
            }
        }
    }

    public class MapInterval
    {
        public int Id { get; set; }
        public decimal MinValue { get; set; }
        public string Color { get; set; }
    }

    public class MapIndicator
    {
        public int Id { get; set; }
        public int BlocId { get; set; }
        public Bloc Bloc { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int IndicatorId { get; set; }
        public Indicator Indicator { get; set; }
        public string Color { get; set; }
    }

    public class MapIndexPageViewModel
    {
        public IEnumerable<Map> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
