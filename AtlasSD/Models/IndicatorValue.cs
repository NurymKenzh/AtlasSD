using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class IndicatorValue
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public int IndicatorId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public Indicator Indicator { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Region")]
        public int RegionId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Region")]
        public Region Region { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Source")]
        public int? SourceId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Source")]
        public Source Source { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal? Value { get; set; }
    }

    public class IndicatorValueViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        public int BlocId { get; set; }
        public Bloc Bloc { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public int? IndicatorId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public Indicator Indicator { get; set; }
        public int Year { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Source")]
        public int SourceId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Source")]
        public Source Source { get; set; }
        public List<IndicatorValue> IndicatorValues { get; set; }
        public int?[] IndicatorIds { get; set; }
        public string[] RegionIds { get; set; }
        public int?[] Years { get; set; }
    }

    public class IndicatorValueIndexPageViewModel
    {
        public IEnumerable<IndicatorValue> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
