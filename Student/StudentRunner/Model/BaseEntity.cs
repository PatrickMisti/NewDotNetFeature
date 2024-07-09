using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentRunner.Model;

public class BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public bool Deleted { get; set; } = false;
}