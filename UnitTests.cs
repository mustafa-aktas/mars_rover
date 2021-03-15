using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static mars_rover.Program;

namespace mars_rover
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void Test1()
        {
            Rover testRover1 = new Rover(new Position(1, 2, "N"), new List<string> { "L", "M", "L", "M", "L", "M", "L", "M", "M" });
            Rover testRover2 = new Rover(new Position(3, 3, "E"), new List<string> { "M", "M", "R", "M", "M", "R", "M", "R", "R", "M" });

            var actualOutput = RoverControl(
                new Point(5, 5),
                new List<Rover> { testRover1, testRover2 });
            var expectedOutput = "1 3 N\n5 1 E";

            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
