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
using Simulator.Data;
using Simulator.Data.Exceptions;
using Simulator.Data.Helpers;

namespace Simulator.Components.TakeOffDynamics
{
    /// <summary>
    /// Base class which describes the standard aircraft dynamics
    /// during takeoff.
    /// </summary>
    public abstract class TakeOffDynamicsCalculatorBase
    {
        private readonly double gravitationalAcceleration;

        /// <summary>
        /// Creates a new instance of <see cref="ContinuedTakeOffDynamicsCalculator"/>.
        /// </summary>
        /// <param name="aircraftData">Tee <see cref="AircraftData"/> which holds
        /// all the information of the aircraft to simulate.</param>
        /// <param name="density">The air density. [kg/m3]</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration g0. [m/s^2]</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="aircraftData"/>
        /// is <c>null</c>.</exception>
        protected TakeOffDynamicsCalculatorBase(AircraftData aircraftData,
                                                double density,
                                                double gravitationalAcceleration)
        {
            if (aircraftData == null)
            {
                throw new ArgumentNullException(nameof(aircraftData));
            }

            AircraftData = aircraftData;
            AerodynamicsData = aircraftData.AerodynamicsData;
            Density = density;
            this.gravitationalAcceleration = gravitationalAcceleration;
        }

        /// <summary>
        /// Calculates the accelerations acting on the aircraft based
        /// on <see cref="AircraftState"/>.
        /// </summary>
        /// <param name="aircraftState">The <see cref="AircraftState"/>
        /// the aircraft is currently in.</param>
        /// <returns>The <see cref="AircraftAccelerations"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="aircraftState"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CalculatorException">Thrown when the <paramref name="aircraftState"/>
        /// results in a state where the calculator cannot continue the calculation.</exception>
        public AircraftAccelerations Calculate(AircraftState aircraftState)
        {
            if (aircraftState == null)
            {
                throw new ArgumentNullException(nameof(aircraftState));
            }

            try
            {
                return new AircraftAccelerations(CalculatePitchRate(aircraftState),
                                                 CalculateClimbRate(aircraftState),
                                                 CalculateTrueAirSpeedRate(aircraftState),
                                                 CalculateFlightPathAngleRate(aircraftState));
            }
            catch (InvalidCalculationException e)
            {
                throw new CalculatorException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.AircraftData"/> from this calculator.
        /// </summary>
        protected AircraftData AircraftData { get; }

        /// <summary>
        /// Gets the <see cref="Data.AerodynamicsData"/> from this calculator.
        /// </summary>
        protected AerodynamicsData AerodynamicsData { get; }

        /// <summary>
        /// Gets the density. [kg/m^3]
        /// </summary>
        protected double Density { get; }

        /// <summary>
        /// Gets the ground friction coefficient that's acting on the aircraft. [-]
        /// </summary>
        /// <returns>The ground friction coefficient.</returns>
        /// <remarks>Ground friction is also denoted as mu.</remarks>
        protected abstract double GetFrictionCoefficient();

        /// <summary>
        /// Calculates the thrust force. [N]
        /// </summary>
        /// <returns>The thrust force.</returns>
        protected abstract double CalculateThrustForce();

        /// <summary>
        /// Calculates the aerodynamic drag force that is acting on the aircraft. [N]
        /// </summary>
        /// <param name="state">The <see cref="AircraftState"/>
        /// the aircraft is currently in.</param>
        /// <returns>The drag force.</returns>
        protected abstract double CalculateAerodynamicDragForce(AircraftState state);

        /// <summary>
        /// Calculates the normal force. [N]
        /// </summary>
        /// <param name="state">The <see cref="AircraftState"/> the aircraft
        /// is currently in.</param>
        /// <returns>The normal force.</returns>
        protected double CalculateNormalForce(AircraftState state)
        {
            double normalForce = GetNewton(AircraftData.TakeOffWeight) - CalculateLiftForce(state);
            if (state.Height >= 0.01 || normalForce < 0)
            {
                return 0;
            }

            return normalForce;
        }

        /// <summary>
        /// Calculates the angle of attack based on <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The <see cref="AircraftState"/> the aircraft
        /// is currently in.</param>
        /// <returns>The angle of attack.</returns>
        protected static Angle CalculateAngleOfAttack(AircraftState state)
        {
            return state.PitchAngle - state.FlightPathAngle;
        }

        /// <summary>
        /// Converts the force in kilo Newton to Newton.
        /// </summary>
        /// <param name="kiloNewton">The amount of force in kilo Newton. [kN]</param>
        /// <returns>The amount of force in Newton.</returns>
        protected static double GetNewton(double kiloNewton)
        {
            return kiloNewton * 1000;
        }

        /// <summary>
        /// Calculates the pitch rate based on <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The <see cref="AircraftState"/> the aircraft
        /// is currently in.</param>
        /// <returns>The pitch rate.</returns>
        protected virtual Angle CalculatePitchRate(AircraftState state)
        {
            return ShouldRotate(state) ? AircraftData.PitchAngleGradient : new Angle();
        }

        private static double CalculateClimbRate(AircraftState aircraftState)
        {
            return aircraftState.TrueAirspeed * Math.Sin(aircraftState.FlightPathAngle.Radians);
        }

        private double CalculateTrueAirSpeedRate(AircraftState aircraftState)
        {
            return (gravitationalAcceleration * (CalculateThrustForce()
                                                 - CalculateAerodynamicDragForce(aircraftState) - CalculateRollDragForce(aircraftState)
                                                 - GetNewton(AircraftData.TakeOffWeight) * Math.Sin(aircraftState.FlightPathAngle.Radians)))
                   / GetNewton(AircraftData.TakeOffWeight);
        }

        private double CalculateRollDragForce(AircraftState aircraftState)
        {
            return GetFrictionCoefficient() * CalculateNormalForce(aircraftState);
        }

        private Angle CalculateFlightPathAngleRate(AircraftState state)
        {
            if (state.TrueAirspeed < 1)
            {
                return new Angle();
            }

            double acceleration = (gravitationalAcceleration * (CalculateLiftForce(state) - GetNewton(AircraftData.TakeOffWeight) + CalculateNormalForce(state)))
                                  / (GetNewton(AircraftData.TakeOffWeight) * state.TrueAirspeed);
            return Angle.FromRadians(acceleration);
        }

        private double CalculateLiftForce(AircraftState state)
        {
            return AerodynamicsHelper.CalculateLift(AircraftData.AerodynamicsData,
                                                    CalculateAngleOfAttack(state),
                                                    Density,
                                                    state.TrueAirspeed);
        }

        private bool ShouldRotate(AircraftState aircraftState)
        {
            double rotationSpeed = 1.2 * AerodynamicsHelper.CalculateStallSpeed(AerodynamicsData,
                                                                                GetNewton(AircraftData.TakeOffWeight),
                                                                                Density);

            return aircraftState.TrueAirspeed >= rotationSpeed
                   && aircraftState.PitchAngle < AircraftData.MaximumPitchAngle;
        }
    }
}