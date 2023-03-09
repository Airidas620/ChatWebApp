using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class UserGroup 
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public HermeAppWebUser User { get; set; } = new HermeAppWebUser();

        public int GroupId { get; set; }
        public Group Group { get; set; } = new Group();
    }

}
