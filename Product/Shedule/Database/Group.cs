using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class Group
    {
        public int GroupId { get; set; } = default;
        public int? TeacherId { get; set; } = null!;
        public int? ClassId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int CountPupils { get; set; } = default;

        public Teacher Teacher { get; set; } = null!;
        public Class Class { get; set; } = null!;
        
        public ICollection<Lesson> Lessons { get; set; } = default!;
    }
}
