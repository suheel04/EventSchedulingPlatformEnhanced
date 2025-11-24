using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventService.Core.DbModels
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // One Category → Many Events
        public ICollection<EventItem> Events { get; set; } = new List<EventItem>();
    }
}
