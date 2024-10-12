# B.Intervals
Implements generic intervals for orderable types (that implement IComparable<T>), for example
- Interval<int>
- Interval<double>
- Interval<DateTime>
- Interval<DateOnly>
- Interval<TimeOnly>
- Interval<MyType>

Examples using intervals of integers


**Intervals by default are inclusive of their endpoints.**

    var interval = new Interval<int>(0, 10);
    Console.WriteLine(interval.Contains(0)); // Will print true
    Console.WriteLine(interval.Contains(5)); // Will print true

You can change them to exclude either end

    interval.IncludesStart = false;
    Console.WriteLine(interval.Contains(0)); // Will print false


**To declare interval start- or end-inclusivity at construction time**

    var endExclusiveInterval = new Interval(0, 10, includesStart: true, includesEnd: false);


**To test if an interval contains a "point"**

    bool result = new Interval<int>(0, 10).Contains(8);


**To test if two intervals intersect**

    bool result = new Interval<int>(0, 10).Intersects(new Interval<int>(5, 15));


**To get the intersection of two intervals**

    Interval<int>? intersection = new Interval<int>(0, 10).IntersectionWith(new Interval<int>(5, 15));

**Version History**
v1.0.2 Added support multiple .Net flavors
