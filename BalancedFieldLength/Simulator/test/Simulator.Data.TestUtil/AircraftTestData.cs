﻿// Copyright (C) 2018 Dennis Tang. All rights reserved.
//
// This file is part of Balanced Field Length.
//
// Balanced Field Length is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Data;
using NUnit.Framework;

namespace Simulator.Data.TestUtil
{
    /// <summary>
    /// Class which can be used to provide real test data with aircraft properties.
    /// </summary>
    public static class AircraftTestData
    {
        /// <summary>
        /// Gets the test cases containing real aircraft data.
        /// </summary>
        /// <returns>The test cases with actual aircraft data.</returns>
        /// <remarks>This test case data can be used as:
        /// <code>
        /// [Test]
        /// [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
        /// public static void SomeTest(AircraftData aircraftData)
        /// {
        ///     // Test
        /// }
        /// </code>
        /// </remarks>
        public static IEnumerable<TestCaseData> GetAircraftData()
        {
            TestAerodynamicsData[] aerodynamicsTestData = GetAerodynamicsData();
            yield return new TestCaseData(new AircraftData(2, 75, 500, Angle.FromDegrees(6), Angle.FromDegrees(16), 0.02, 0.2, aerodynamicsTestData[0].Data))
                .SetName(aerodynamicsTestData[0].TestName);
            yield return new TestCaseData(new AircraftData(3, 120, 1200, Angle.FromDegrees(5), Angle.FromDegrees(15), 0.02, 0.2, aerodynamicsTestData[1].Data))
                .SetName(aerodynamicsTestData[1].TestName);
            yield return new TestCaseData(new AircraftData(4, 300, 3500, Angle.FromDegrees(4), Angle.FromDegrees(14), 0.02, 0.2, aerodynamicsTestData[2].Data))
                .SetName(aerodynamicsTestData[2].TestName);
        }

        /// <summary>
        /// Gets the test cases containing real aerodynamic data.
        /// </summary>
        /// <returns>The test cases with aerodynamic data.</returns>
        /// <remarks>This test case data can be used as:
        /// <code>
        /// [Test]
        /// [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAerodynamicsDataTestCases))]
        /// public static void SomeTest(AerodynamicsData aerodynamicsData)
        /// {
        ///     // Test
        /// }
        /// </code>
        /// </remarks>
        public static IEnumerable<TestCaseData> GetAerodynamicsDataTestCases()
        {
            return GetAerodynamicsData().Select(testData => new TestCaseData(testData.Data)
                                                    .SetName(testData.TestName));
        }

        private static TestAerodynamicsData[] GetAerodynamicsData()
        {
            return new[]
            {
                new TestAerodynamicsData(new AerodynamicsData(15, 100, Angle.FromDegrees(-3), 4.85, 1.60, 0.021, 0.026, 0.85), "Two Jet Power Engine"),
                new TestAerodynamicsData(new AerodynamicsData(14, 200, Angle.FromDegrees(-4), 4.32, 1.45, 0.024, 0.028, 0.80), "Three Jet Power Engine"),
                new TestAerodynamicsData(new AerodynamicsData(12, 500, Angle.FromDegrees(-5), 3.95, 1.40, 0.026, 0.029, 0.82), "Four Jet Power Engine")
            };
        }

        private class TestAerodynamicsData
        {
            public TestAerodynamicsData(AerodynamicsData data, string testName)
            {
                Data = data;
                TestName = testName;
            }

            public AerodynamicsData Data { get; }
            public string TestName { get; }
        }
    }
}