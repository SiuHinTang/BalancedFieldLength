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

using System;
using Core.Common.Data;
using Core.Common.Utils;

namespace Simulator.Data
{
    /// <summary>
    /// Class to hold all the relevant properties of an  aircraft.
    /// </summary>
    public class AircraftData
    {
        /// <summary>
        /// Creates a new instance of <see cref="AircraftData"/>.
        /// </summary>
        /// <param name="nrOfEngines">The number of engines.</param>
        /// <param name="maximumThrustPerEngine">The maximum thrust per engine. [kN]</param>
        /// <param name="takeOffWeight">The takeoff weight. [kN]</param>
        /// <param name="pitchAngleGradient">The rate of pitch angle during rotation. [1/s]</param>
        /// <param name="maximumPitchAngle">The maximum pitch angle during rotation.</param>
        /// <param name="rollingResistanceCoefficient">The rolling resistance coefficient. [-]</param>
        /// <param name="brakingResistanceCoefficient">The brake resistance coefficient. [-]</param>
        /// <param name="aerodynamicsData">The <see cref="AerodynamicsData"/>
        /// holding all aerodynamic properties of the aircraft.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="aerodynamicsData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any parameter, except <paramref name="nrOfEngines"/>, is
        /// <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="nrOfEngines"/> &lt;= 0</item>
        /// <item><paramref name="maximumThrustPerEngine"/> &lt;= 0</item>
        /// <item><paramref name="takeOffWeight"/> &lt;= 0</item>
        /// <item><paramref name="pitchAngleGradient"/> &lt;= 0</item>
        /// <item><paramref name="maximumPitchAngle"/> &lt;= 0</item>
        /// <item><paramref name="rollingResistanceCoefficient"/> &lt; 0</item>
        /// <item><paramref name="brakingResistanceCoefficient"/> &lt; 0</item>
        /// </list>
        /// </exception>
        public AircraftData(int nrOfEngines, double maximumThrustPerEngine,
                            double takeOffWeight,
                            Angle pitchAngleGradient, Angle maximumPitchAngle,
                            double rollingResistanceCoefficient, double brakingResistanceCoefficient,
                            AerodynamicsData aerodynamicsData)
        {
            if (aerodynamicsData == null)
            {
                throw new ArgumentNullException(nameof(aerodynamicsData));
            }

            ValidateInput(nrOfEngines, maximumThrustPerEngine, takeOffWeight,
                          pitchAngleGradient, maximumPitchAngle,
                          rollingResistanceCoefficient, brakingResistanceCoefficient);

            NrOfEngines = nrOfEngines;
            MaximumThrustPerEngine = maximumThrustPerEngine;
            TakeOffWeight = takeOffWeight;
            PitchAngleGradient = pitchAngleGradient;
            MaximumPitchAngle = maximumPitchAngle;
            RollingResistanceCoefficient = rollingResistanceCoefficient;
            BrakingResistanceCoefficient = brakingResistanceCoefficient;
            AerodynamicsData = aerodynamicsData;
        }

        /// <summary>
        /// Gets the number of engines of the aircraft.
        /// </summary>
        public int NrOfEngines { get; }

        /// <summary>
        /// Gets the maximum thrust per engine of the aircraft. [kN]
        /// </summary>
        public double MaximumThrustPerEngine { get; }

        /// <summary>
        /// Gets the take off weight of the aircraft. [kN]
        /// </summary>
        public double TakeOffWeight { get; }

        /// <summary>
        /// Gets the pitch angle gradient during rotation. [1/s]
        /// </summary>
        /// <remarks>Also denoted as dTheta/ds.</remarks>
        public Angle PitchAngleGradient { get; }

        /// <summary>
        /// Gets the maximum pitch angle during rotation.
        /// </summary>
        /// <remarks>Also denoted as theta.</remarks>
        public Angle MaximumPitchAngle { get; }

        /// <summary>
        /// Gets the rolling resistance coefficient. [-]
        /// </summary>
        /// <remarks>Also denoted as Mu_roll.</remarks>
        public double RollingResistanceCoefficient { get; }

        /// <summary>
        /// Gets the braking resistance coefficient. [-]
        /// </summary>
        /// <remarks>Also denoted as Mu_brake.</remarks>
        public double BrakingResistanceCoefficient { get; }

        /// <summary>
        /// Gets the <see cref="AerodynamicsData"/> containing all the aerodynamic
        /// data of the aircraft.
        /// </summary>
        public AerodynamicsData AerodynamicsData { get; }

        /// <summary>
        /// Validates the numeric input data.
        /// </summary>
        /// <param name="nrOfEngines">The number of engines.</param>
        /// <param name="maximumThrustPerEngine">The maximum thrust per engine. [kN]</param>
        /// <param name="takeOffWeight">The takeoff weight. [kN]</param>
        /// <param name="pitchAngleGradient">The rate of pitch angle during rotation. [1/s]</param>
        /// <param name="maximumPitchAngle">The maximum pitch angle during rotation.</param>
        /// <param name="rollingResistanceCoefficient">The rolling resistance coefficient. [-]</param>
        /// <param name="brakingResistanceCoefficient">The brake resistance coefficient. [-]</param>
        /// <exception cref="ArgumentException">Thrown when any parameter, except <paramref name="nrOfEngines"/>, is
        /// <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="nrOfEngines"/> &lt;= 0</item>
        /// <item><paramref name="maximumThrustPerEngine"/> &lt;= 0</item>
        /// <item><paramref name="takeOffWeight"/> &lt;= 0</item>
        /// <item><paramref name="pitchAngleGradient"/> &lt;= 0</item>
        /// <item><paramref name="maximumPitchAngle"/> &lt;= 0</item>
        /// <item><paramref name="rollingResistanceCoefficient"/> &lt; 0</item>
        /// <item><paramref name="brakingResistanceCoefficient"/> &lt; 0</item>
        /// </list>
        /// </exception>
        private static void ValidateInput(int nrOfEngines, double maximumThrustPerEngine, double takeOffWeight,
                                          Angle pitchAngleGradient, Angle maximumPitchAngle, double rollingResistanceCoefficient,
                                          double brakingResistanceCoefficient)
        {
            nrOfEngines.ArgumentIsLargerThanZero(nameof(nrOfEngines));

            maximumThrustPerEngine.ArgumentIsLargerThanZero(nameof(maximumThrustPerEngine));
            maximumThrustPerEngine.ArgumentIsConcreteNumber(nameof(maximumThrustPerEngine));

            takeOffWeight.ArgumentIsLargerThanZero(nameof(takeOffWeight));
            takeOffWeight.ArgumentIsConcreteNumber(nameof(takeOffWeight));

            pitchAngleGradient.ArgumentIsLargerThanZero(nameof(pitchAngleGradient));
            pitchAngleGradient.ArgumentIsConcreteNumber(nameof(pitchAngleGradient));

            maximumPitchAngle.ArgumentIsLargerThanZero(nameof(maximumPitchAngle));
            maximumPitchAngle.ArgumentIsConcreteNumber(nameof(maximumPitchAngle));

            rollingResistanceCoefficient.ArgumentIsLargerOrEqualToZero(nameof(rollingResistanceCoefficient));
            rollingResistanceCoefficient.ArgumentIsConcreteNumber(nameof(rollingResistanceCoefficient));

            brakingResistanceCoefficient.ArgumentIsLargerOrEqualToZero(nameof(brakingResistanceCoefficient));
            brakingResistanceCoefficient.ArgumentIsConcreteNumber(nameof(brakingResistanceCoefficient));
        }
    }
}