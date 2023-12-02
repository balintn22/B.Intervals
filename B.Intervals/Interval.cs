using System;

namespace B.Intervals;

/// <summary>
/// Implements a generic interval, that can test if a point is within it.
/// Supports both inclusive and exclusive endpoints.
/// Requires that T : IComparable<typeparamref name="T"/>.
/// </summary>
public class Interval<T> : IInterval<T> where T : IComparable<T>
{
    public T Start { get; set; }
    public T End { get; set; }
    public bool IncludesStart { get; set; }
    public bool IncludesEnd { get; set; }


    /// <summary>
    /// Creates an interval, requiring start and end vales to be correctly ordered.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="includesStart">Specifies whether the interval includes its start point, for containment tests.</param>
    /// <param name="includesEnd">Specifies whether the interval includes its end point, for containment tests.</param>
    /// <exception cref="ArgumentException">Thrown if start > end.</exception>
    public Interval(
        T start,
        T end,
        bool includesStart = true,
        bool includesEnd = true)
    {
        if (IsGreaterThan(start, end))
            throw new ArgumentException($"Invalid order of interval boundaries. " +
                $"Start ({start}) should be not be greater then end ({end}).");

        Start = start;
        End = end;
        IncludesStart = includesStart;
        IncludesEnd = includesEnd;
    }

    /// <summary>
    /// Safely creates an interval, swapping the values if necessary to make them correctly ordered.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="includesStart">Specifies whether the interval includes its start point, for containment tests.</param>
    /// <param name="includesEnd">Specifies whether the interval includes its end point, for containment tests.</param>
    public static Interval<T> SafeCreate(
        T start,
        T end,
        bool includesStart = true,
        bool includesEnd = true)
    {
        T s, e;

        if (IsGreaterThan(start, end))
        {
            s = end;
            e = start;
        }
        else
        {
            s = start;
            e = end;
        }

        return new Interval<T>(
            s,
            e,
            includesStart: includesStart,
            includesEnd: includesEnd);
    }

    private bool IsIncludedByTheStart(T point)
    {
        return IncludesStart
            ? IsGreaterThanOrEqual(point, Start)
            : IsGreaterThan(point, Start);
    }

    private bool IsIncludedByTheEnd(T point)
    {
        return IncludesStart
            ? IsGreaterThanOrEqual(End, point)
            : IsGreaterThan(End, point);
    }

    internal static bool IsGreaterThan(T first, T second) =>
        first.CompareTo(second) > 0;

    internal static bool IsGreaterThanOrEqual(T first, T second) =>
        first.CompareTo(second) >= 0;

    /// <summary>
    /// Determines if the interval contains the test point using IComparer<T>.
    /// Uses the start and endpoint inclusion values (specified at interval
    /// construction time) to treat start and endpoint inclusion.
    /// </summary>
    public bool Contains(T point)
    {
        return IsIncludedByTheStart(point) && IsIncludedByTheEnd(point);
    }

    public bool Intersects(IInterval<T> other)
    {
        if (this == null)
            return false;

        if (other == null)
            return false;

        return Contains(other.Start)
            || Contains(other.End)
            || other.Contains(Start)
            || other.Contains(End);
    }

    /// <summary>
    /// Determines the intersection of two intervals.
    /// Not very mathematical in the sense that if there's no intersection,
    /// return a null value, rather than an empty interval.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// An interval that is the intersection of this interval and the other.
    /// Null if either of the arguments are null, or the two don't intersect.
    /// </returns>
    public Interval<T>? IntersectionWith(Interval<T>? other)
    {
        if (this == null || other == null)
            return null;

        T start;
        bool includesStart;
        int startComparision = this.Start.CompareTo(other.Start);
        if (startComparision < 0)
        {
            start = other.Start;
            includesStart = other.IncludesStart;
        }
        else if (startComparision == 0)
        {
            start = this.Start;
            includesStart = this.IncludesStart && other.IncludesStart;
        }
        else // if (startComparision > 0)
        {
            start = this.Start;
            includesStart = this.IncludesStart;
        }

        T end;
        bool includesEnd;
        int endComparision = this.End.CompareTo(other.End);
        if (endComparision < 0)
        {
            end = this.End;
            includesEnd = this.IncludesEnd;
        }
        else if (endComparision == 0)
        {
            end = this.End;
            includesEnd = this.IncludesEnd && other.IncludesEnd;
        }
        else // if (endComparision > 0)
        {
            end = other.End;
            includesEnd = other.IncludesEnd;
        }

        int newStartEndComparison = start.CompareTo(end);
        if (newStartEndComparison > 0)
            return null;

        return new Interval<T>(start, end, includesStart, includesEnd);
    }
}
