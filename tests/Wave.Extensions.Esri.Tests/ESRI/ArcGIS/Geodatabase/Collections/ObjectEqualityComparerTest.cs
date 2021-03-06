﻿using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class ObjectEqualityComparerTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void ObjectEqualityComparer_Equals_False()
        {
            ObjectEqualityComparer comparer = new ObjectEqualityComparer();

            var testClass = base.GetPointFeatureClass();
            var rows = testClass.Fetch(1, 2);

            Assert.AreEqual(2, rows.Count);

            var equals = comparer.Equals(rows.First(), rows.Last());
            Assert.IsFalse(equals);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ObjectEqualityComparer_Equals_True()
        {
            ObjectEqualityComparer comparer = new ObjectEqualityComparer();

            var testClass = base.GetPointFeatureClass();
            var row = testClass.Fetch(1);

            var equals = comparer.Equals(row, row);
            Assert.IsTrue(equals);
        }

        #endregion
    }
}