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

namespace Application.BalancedFieldLength.KernelWrapper.TestUtils
{
    /// <summary>
    /// Implementation of <see cref="IBalancedFieldLengthCalculationModuleFactory"/> which can be used for testing.
    /// </summary>
    public class TestBalancedFieldLengthCalculationModuleFactory : IBalancedFieldLengthCalculationModuleFactory
    {
        public TestBalancedFieldLengthCalculationModuleFactory()
        {
            TestModule = new TestBalancedFieldLengthCalculationModule();
        }

        /// <summary>
        /// Gets the <see cref="TestBalancedFieldLengthCalculationModule"/> that is generated when calling
        /// <see cref="CreateModule"/>.
        /// </summary>
        public TestBalancedFieldLengthCalculationModule TestModule { get; }

        public IBalancedFieldLengthCalculationModule CreateModule()
        {
            return TestModule;
        }
    }
}