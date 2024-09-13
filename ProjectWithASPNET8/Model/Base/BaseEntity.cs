using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectWithASPNET8.Model.Base
{
    public class BaseEntity
    {
        [Column("id")]
        public long Id { get; set; }
    }
}
