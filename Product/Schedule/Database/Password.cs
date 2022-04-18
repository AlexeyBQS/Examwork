using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Database
{
    public class Password
    {
        public int PasswordId { get; set; } = default!;
        public string Hash { get; set; } = null!;

        public override string ToString() => $"{PasswordId}: {Hash}";
    }
}
