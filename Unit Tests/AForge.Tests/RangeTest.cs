﻿using NUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class RangeTest
    {
        [TestCase( 0, 1, 1, 2, true )]
        [TestCase( 0, 1, 2, 3, false )]
        [TestCase( 0, 10, 2, 4, true )]
        [TestCase( 0, 10, 5, 15, true )]
        [TestCase( 0, 10, -5, 5, true )]
        [TestCase( 2, 4, 0, 10, true )]
        [TestCase( 5, 15, 0, 10, true )]
        [TestCase( -5, 5, 0, 10, true )]
        public void IsOverlappingTest( float min1, float max1, float min2, float max2, bool expectedResult )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.That( range1.IsOverlapping( range2 ), Is.EqualTo(expectedResult));
        }

        [TestCase( 0.4f, 7.3f, 1, 7, true )]
        [TestCase( -6.6f, -0.1f, -6, -1, true )]
        [TestCase( 0.4f, 7.3f, 0, 8, false )]
        [TestCase( -6.6f, -0.1f, -7, 0, false )]
        public void ToRangeTest( float fMin, float fMax, int iMin, int iMax, bool innerRange )
        {
            Range range = new Range( fMin, fMax );
            IntRange iRange = range.ToIntRange( innerRange );

            Assert.That( iRange.Min, Is.EqualTo(iMin));
            Assert.That( iRange.Max, Is.EqualTo(iMax));
        }

        [TestCase( 1.1f, 2.2f, 1.1f, 2.2f, true )]
        [TestCase( -2.2f, -1.1f, -2.2f, -1.1f, true )]
        [TestCase( 1.1f, 2.2f, 2.2f, 3.3f, false )]
        [TestCase( 1.1f, 2.2f, 1.1f, 4.4f, false )]
        [TestCase( 1.1f, 2.2f, 3.3f, 4.4f, false )]
        public void EqualityOperatorTest( float min1, float max1, float min2, float max2, bool areEqual )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.That( areEqual, Is.EqualTo(range1.Equals( range2 )));
            Assert.That( areEqual, Is.EqualTo(range1 == range2));
            Assert.That( !areEqual, Is.EqualTo(range1 != range2));
        }
    }
}
