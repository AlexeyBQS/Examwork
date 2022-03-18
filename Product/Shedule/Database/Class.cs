using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class Class
    {
        public int ClassId { get; set; } = default;
        public string Name { get; set; } = null!;

        public ICollection<Group> Groups { get; set; } = default!;
        public ICollection<Lesson> Lessons { get; set; } = default!;
    }
}
