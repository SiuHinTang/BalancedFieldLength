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

using NUnit.Framework;

namespace Simulator.Data.TestUtil.Test
{
    [TestFixture]
    public class CalculationSettingsTestFactoryTest
    {
        [Test]
        public void CreateDistanceCalculatorSettings_Always_ReturnsExpectedValues()
        {
            // Call
            CalculationSettings settings = CalculationSettingsTestFactory.CreateDistanceCalculatorSettings();

            // Assert
            Assert.IsNotNull(settings);
            Assert.Greater(settings.FailureSpeed, 0);
            Assert.Greater(settings.MaximumNrOfTimeSteps, 0);
            Assert.IsTrue(IsConcreteNumber(settings.TimeStep));
        }

        private static bool IsConcreteNumber(double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }
    }
}