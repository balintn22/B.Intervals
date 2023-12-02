namespace B.Intervals;

public interface IInterval<T>
{
    T Start { get; set; }
    T End { get; set; }
    bool IncludesStart { get; set; }
    bool IncludesEnd { get; set; }
    bool Contains(T point);
    bool Intersects(IInterval<T> other);
}
