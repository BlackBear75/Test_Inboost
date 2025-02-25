using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test_INBOOST.Base;

public abstract class BaseData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreationDate { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletionDate { get; set; }

    protected BaseData()
    {
        Deleted = false;
        var now = DateTime.UtcNow;
        CreationDate = new DateTime(now.Ticks / 100000 * 100000, now.Kind);
    }
}