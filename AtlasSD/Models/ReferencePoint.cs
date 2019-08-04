using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class ReferencePoint
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public int? IndicatorId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public Indicator Indicator { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Minimum")]
        public decimal Min { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Maximum")]
        public decimal Max { get; set; }
    }

    public class ReferencePointViewModel : ReferencePoint
    {
        [NotMapped]
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        public int BlocId { get; set; }
        public Bloc Bloc { get; set; }
        [NotMapped]
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "FieldIsRequired")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }

    public class ReferencePointIndexPageViewModel
    {
        public IEnumerable<ReferencePoint> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
