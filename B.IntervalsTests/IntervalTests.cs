using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace B.Intervals.Tests;

[TestClass()]
public class IntervalTests
{
    [TestMethod()]
    public void Ctor_HappyCase()
    {
        Interval<int> interval = new Interval<int>(1, 2);

        interval.Start.Should().Be(1);
        interval.End.Should().Be(2);
    }

    [TestMethod()]
    public void Ctor_ShouldThrow_IfOrderIncorrect()
    {
        var act = () => new Interval<int>(2, 1);

        act.Should().Throw<ArgumentException>();
    }


    [TestMethod()]
    public void SafeCreate_HappyCase()
    {
        Interval<int> intervalA = Interval<int>.SafeCreate(1, 2);
        intervalA.Start.Should().Be(1);
        intervalA.End.Should().Be(2);

        Interval<int> intervalB = Interval<int>.SafeCreate(2, 1);
        intervalB.Start.Should().Be(1);
        intervalB.End.Should().Be(2);
    }

    [DataTestMethod()]
    [DataRow(1, 3, 1, true)]
    [DataRow(1, 3, 3, true)]
    [DataRow(1, 3, 2, true)]
    [DataRow(1, 3, 0, false)]
    [DataRow(1, 3, 4, false)]
    public void Contains_ShouldReturnExpectedValue_ForInclusiveInterval(
        int start, int end, int testPoint, bool expectedResult)
    {
        Interval<int> sut = new Interval<int>(
            start,
            end,
            includesStart: true,
            includesEnd: true);

        sut.Contains(testPoint).Should().Be(expectedResult);
    }

    [DataTestMethod()]
    [DataRow(1, 3, 1, false)]
    [DataRow(1, 3, 3, false)]
    [DataRow(1, 3, 2, true)]
    [DataRow(1, 3, 0, false)]
    [DataRow(1, 3, 4, false)]
    public void Contains_ShouldReturnExpectedValue_ForExclusiveInterval(
        int start, int end, int testPoint, bool expectedResult)
    {
        Interval<int> sut = new Interval<int>(
            start,
            end,
            includesStart: false,
            includesEnd: false);

        sut.Contains(testPoint).Should().Be(expectedResult);
    }

    [DataTestMethod()]
    [DataRow(1, 2, 11, 12, false)]
    [DataRow(11, 12, 1, 2, false)]
    [DataRow(1, 12, 11, 22, true)]
    [DataRow(1, 10, 5, 6, true)]
    [DataRow(5, 6, 1, 10, true)]
    [DataRow(1, 2, 2, 3, true)]
    [DataRow(2, 3, 1, 2, true)]
    public void Overlaps_ReturnsExpectedResult_ForInclusiveIntervals(
        int i1Start, int i1End, int i2Start, int i2End, bool expectedResult)
    {
        var i1 = new Interval<int>(i1Start, i1End, includesStart: true, includesEnd: true);
        var i2 = new Interval<int>(i2Start, i2End, includesStart: true, includesEnd: true);

        i1.Intersects(i2).Should().Be(expectedResult);
    }

    [DataTestMethod()]
    [DataRow(1, 2, 11, 12, false)]
    [DataRow(11, 12, 1, 2, false)]
    [DataRow(1, 12, 11, 22, true)]
    [DataRow(1, 10, 5, 6, true)]
    [DataRow(5, 6, 1, 10, true)]
    [DataRow(1, 2, 2, 3, false)]
    [DataRow(2, 3, 1, 2, false)]
    public void Overlaps_ReturnsExpectedResult_ForNonInclusiveIntervals(
        int i1Start, int i1End, int i2Start, int i2End, bool expectedResult)
    {
        var i1 = new Interval<int>(i1Start, i1End, includesStart: false, includesEnd: false);
        var i2 = new Interval<int>(i2Start, i2End, includesStart: false, includesEnd: false);

        i1.Intersects(i2).Should().Be(expectedResult);
    }

    [TestMethod()]
    public void IntersectionWith_ShouldReturnSame_IfIntervalsAreTheSame()
    {
        new Interval<int>(0, 10, true, true)
            .IntersectionWith(new Interval<int>(0, 10, true, true))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, true, true));
    }

    [TestMethod()]
    public void IntersectionWith_ShouldReturnNull_OtherIsNull()
    {
        new Interval<int>(0, 10, true, true)
            .IntersectionWith(null)
            .Should().BeNull();
    }

    [TestMethod()]
    public void IntersectionWith_ShouldReturnNull_IfIntervalsDontIntersect()
    {
        new Interval<int>(0, 10, true, true)
            .IntersectionWith(new Interval<int>(20, 30, true, true))
            .Should().BeNull();
    }

    [DataTestMethod()]
    [DataRow(true, true, true)]
    [DataRow(true, false, false)]
    [DataRow(false, true, false)]
    public void IntersectionWith_ShouldDetermineExpectedStartExclusivity_WhenStartPointsMatch(
        bool i1IncludesStart, bool i2InlcudesStart, bool expectedResult)
    {
        new Interval<int>(0, 10, i1IncludesStart, true)
            .IntersectionWith(new Interval<int>(0, 10, i2InlcudesStart, true))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, expectedResult, true));
    }

    [DataTestMethod()]
    [DataRow(true, true, true)]
    [DataRow(true, false, false)]
    [DataRow(false, true, false)]
    public void IntersectionWith_ShouldDetermineExpectedEndExclusivity_WhenEndPointsMatch(
        bool i1IncludesEnd, bool i2InlcudesEnd, bool expectedResult)
    {
        new Interval<int>(0, 10, true, i1IncludesEnd)
            .IntersectionWith(new Interval<int>(0, 10, true, i2InlcudesEnd))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, true, expectedResult));
    }

    [DataTestMethod()]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void IntersectionWith_ShouldDetermineExpectedStartExclusivity_WhenThisStartIsIncludedInOther(
        bool i1IncludesStart, bool expectedResult)
    {
        new Interval<int>(0, 10, i1IncludesStart, true)
            .IntersectionWith(new Interval<int>(-10, 10, true, true))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, expectedResult, true));
    }

    [DataTestMethod()]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void IntersectionWith_ShouldDetermineExpectedStartExclusivity_WhenOtherStartIsIncludedInOther(
        bool i2IncludesStart, bool expectedResult)
    {
        new Interval<int>(-10, 10, true, true)
            .IntersectionWith(new Interval<int>(0, 10, i2IncludesStart, true))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, expectedResult, true));
    }

    [DataTestMethod()]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void IntersectionWith_ShouldDetermineExpectedEndExclusivity_WhenThisEndIsIncludedInOther(
        bool i1IncludesEnd, bool expectedResult)
    {
        new Interval<int>(0, 10, true, i1IncludesEnd)
            .IntersectionWith(new Interval<int>(0, 20, true, true))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, true, expectedResult));
    }

    [DataTestMethod()]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void IntersectionWith_ShouldDetermineExpectedEndExclusivity_WhenOtherEndIsIncludedInOther(
        bool i2IncludesEnd, bool expectedResult)
    {
        new Interval<int>(0, 20, true, true)
            .IntersectionWith(new Interval<int>(0, 10, true, i2IncludesEnd))
            .Should().BeEquivalentTo(new Interval<int>(0, 10, true, expectedResult));
    }
}
