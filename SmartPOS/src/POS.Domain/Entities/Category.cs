using POS.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Category : BaseEntity, IEntity
    {
        [Key]
        public long Categoryid { get; set; }
        public string CategoryName { get; set; } = String.Empty;
        public string CategoryDescription { get; set; } = String.Empty;
    }
}
