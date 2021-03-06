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

using Simulator.Kernel;

namespace Application.BalancedFieldLength.KernelWrapper
{
    /// <summary>
    /// Factory that creates balanced field length kernels.
    /// </summary>
    public class BalancedFieldLengthKernelFactory : IBalancedFieldLengthKernelFactory
    {
        private static IBalancedFieldLengthKernelFactory instance;

        private BalancedFieldLengthKernelFactory() {}

        /// <summary>
        /// Gets the current instance of <see cref="BalancedFieldLengthKernelFactory"/>.
        /// </summary>
        public static IBalancedFieldLengthKernelFactory Instance
        {
            get
            {
                return instance ?? (instance = new BalancedFieldLengthKernelFactory());
            }
            internal set
            {
                instance = value;
            }
        }

        public IAggregatedDistanceCalculatorKernel CreateDistanceCalculatorKernel()
        {
            return new AggregatedDistanceCalculatorKernel();
        }
    }
}