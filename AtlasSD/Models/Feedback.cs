using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasSD.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Email")]
        public string Email { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Message")]
        public string Message { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DateTime")]
        public DateTime DateTime { get; set; }
    }

    public class FeedbackIndexPageViewModel
    {
        public IEnumerable<Feedback> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
