﻿using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FieldsEqualityComparerTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void FieldsEqualityComparer_Equals_False()
        {
            FieldsEqualityComparer comparer = new FieldsEqualityComparer();

            var testTable = base.GetTestTable();
            var rows = testTable.Fetch(1, 2);

            Assert.AreEqual(2, rows.Count);

            var equals = comparer.Equals(rows.First(), rows.Last());
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void FieldsEqualityComparer_Equals_True()
        {
            FieldsEqualityComparer comparer = new FieldsEqualityComparer();

            var testTable = base.GetTestTable();
            var rows = testTable.Fetch(1);

            Assert.AreEqual(1, rows.Count);

            var equals = comparer.Equals(rows.First(), rows.Last());
            Assert.IsTrue(equals);
        }

        #endregion
    }
}