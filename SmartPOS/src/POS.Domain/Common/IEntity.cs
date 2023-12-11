using POS.Domain.Enums;
namespace POS.Domain.Common;

public interface IEntity<T> where T : IEquatable<T>
{
    T Id { get; set; }
    long CreatedBy { get; set; }
    DateTimeOffset CreatedDate { get; set; }
    long? UpdatedBy { get; set; }
    DateTimeOffset? UpdatedDate { get; set; }
    bool IsDelete { get; set; }
    public EntityStatus Status { get; set; }
}
public interface IEntity : IEntity<long>
{

}