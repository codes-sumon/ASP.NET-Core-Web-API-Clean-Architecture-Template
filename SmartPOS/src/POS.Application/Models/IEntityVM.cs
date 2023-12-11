namespace POS.Application.Models;

public interface IEntityVM<T> where T : IEquatable<T>
{
    T Id { get; set; }
}
public interface IEntityVM : IEntityVM<long>
{

}