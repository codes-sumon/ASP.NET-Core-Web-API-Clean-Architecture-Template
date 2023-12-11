namespace POS.Application.Common.Collection;

public interface IDropdown<T>
{
    public IList<T> Data { get; set; }
}