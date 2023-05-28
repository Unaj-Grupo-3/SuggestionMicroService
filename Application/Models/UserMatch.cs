using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UserMatch
    {
        public int UserMatchId { get; set; }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 3 estados del Like. 0 => Todavia no lo vio.
        //                     1 => Le dio Like (checkear el otro)
        //                    -1 => Le dio Dislike

        public int LikeUser2 { get; set; }
        public int LikeUser1 { get; set; }
    }
}
