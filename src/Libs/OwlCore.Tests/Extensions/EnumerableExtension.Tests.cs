using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.Extensions;
using System.Threading.Tasks;

namespace OwlCore.Tests.Extensions
{
    [TestClass]
    public class EnumerableExtensions
    {
        [DataRow(1, 0, 1)]
        [DataRow(7, 5, 6, 7)]
        [DataRow(9999, 100, 500, 1000, 9999)]
        [TestMethod]
        public void Pop(int expectedReturn, params int[] items)
        {
            var list = items.ToList();
            var lastItem = list.Pop();

            Assert.AreEqual(lastItem, expectedReturn);
            Assert.AreEqual(lastItem, items.Last());
        }

        [DataRow(-1, 1, 0)]
        [DataRow(0, 1, 0, 1)]
        [DataRow(1, 10, 5, 6, 7)]
        [DataRow(3, 0, 100, 500, 1000, 9999)]
        [DataRow(10, 0, 100, 500, 1000, 9999)]
        [TestMethod]
        public void ReplaceOrAdd(int indexToReplace, int newValue, params int[] items)
        {
            var list = items.ToList();

            if (indexToReplace > items.Length || indexToReplace < 0)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                {
                    list.ReplaceOrAdd(indexToReplace, newValue);
                });

                return;
            }

            list.ReplaceOrAdd(indexToReplace, newValue);

            Assert.IsTrue(list[indexToReplace] == newValue);
        }
    }
}
