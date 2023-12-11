namespace POS.Domain.Common;

public class MasterEntity<T> where T : IEquatable<T>
{
    public T Id { get; set; }
}
public class MasterEntity : MasterEntity<long>
{

}