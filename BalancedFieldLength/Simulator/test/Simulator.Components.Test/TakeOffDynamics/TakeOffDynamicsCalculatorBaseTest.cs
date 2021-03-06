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
using Core.Common.TestUtil;
using NUnit.Framework;
using Simulator.Components.TakeOffDynamics;
using Simulator.Data;
using Simulator.Data.Exceptions;
using Simulator.Data.Helpers;
using Simulator.Data.TestUtil;

namespace Simulator.Components.Test.TakeOffDynamics
{
    [TestFixture]
    public class TakeOffDynamicsCalculatorBaseTest
    {
        private const double gravitationalAcceleration = SimulationConstants.GravitationalAcceleration;
        private const double airDensity = SimulationConstants.Density;
        private const double tolerance = SimulationConstants.Tolerance;

        [Test]
        public void Constructor_AircraftDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new TestTakeOffDynamicsCalculator(null, random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("aircraftData", exception.ParamName);
        }

        [Test]
        public void Calculate_AircraftStateNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            AircraftData aircraftData = AircraftDataTestFactory.CreateRandomAircraftData();
            var calculator = new TestTakeOffDynamicsCalculator(aircraftData, random.NextDouble(), random.NextDouble());

            // Call
            TestDelegate call = () => calculator.Calculate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("aircraftState", exception.ParamName);
        }

        [Test]
        public static void Calculate_Always_SendsCorrectInput()
        {
            // Setup
            var random = new Random(21);

            AircraftData aircraftData = AircraftDataTestFactory.CreateRandomAircraftData();
            var calculator = new TestTakeOffDynamicsCalculator(aircraftData, random.NextDouble(), random.NextDouble());
            Angle angleOfAttack = AerodynamicsDataTestHelper.GetValidAngleOfAttack(aircraftData.AerodynamicsData);

            var state = new AircraftState(angleOfAttack,
                                          new Angle(),
                                          random.NextDouble(),
                                          random.NextDouble(),
                                          random.NextDouble());

            // Call 
            calculator.Calculate(state);

            // Assert
            Assert.AreSame(state, calculator.CalculateDragInput);
        }

        [Test]
        public static void Calculate_WithAircraftStateAlways_ReturnsExpectedClimbRate()
        {
            // Setup
            var random = new Random(21);
            AircraftData aircraftData = AircraftDataTestFactory.CreateRandomAircraftData();
            Angle angleOfAttack = AerodynamicsDataTestHelper.GetValidAngleOfAttack(aircraftData.AerodynamicsData);

            var aircraftState = new AircraftState(angleOfAttack,
                                                  new Angle(),
                                                  random.NextDouble(),
                                                  random.NextDouble(),
                                                  random.NextDouble());

            var calculator = new TestTakeOffDynamicsCalculator(aircraftData, random.NextDouble(), random.NextDouble());

            // Call 
            AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

            // Assert
            double expectedClimbRate = aircraftState.TrueAirspeed * Math.Sin(aircraftState.FlightPathAngle.Radians);
            Assert.AreEqual(expectedClimbRate, accelerations.ClimbRate, tolerance);
        }

        [Test]
        public void GivenAircraftStateResultsIntoInvalidCalculationException_WhenCalculateCalled_ThenCalculatorExceptionThrown()
        {
            // Given
            var random = new Random(21);

            AircraftData aircraftData = AircraftDataTestFactory.CreateRandomAircraftData();
            var calculator = new TestTakeOffDynamicsCalculator(aircraftData, random.NextDouble(), random.NextDouble());

            var state = new AircraftState(aircraftData.AerodynamicsData.ZeroLiftAngleOfAttack - Angle.FromRadians(1),
                                          new Angle(),
                                          random.NextDouble(),
                                          random.NextDouble(),
                                          random.NextDouble());

            // When
            TestDelegate call = () => calculator.Calculate(state);

            // Then
            var exception = Assert.Throws<CalculatorException>(call);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<InvalidCalculationException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
        }

        [TestFixture]
        public class CalculateTrueAirspeedRate
        {
            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAircraftStateHeightEqualToThresholdAndLiftLowerThanTakeOffWeight_ReturnsExpectedTrueAirspeedRate(AircraftData aircraftData)
            {
                // Setup
                const double airspeed = 10.0;
                const double threshold = 0.01;

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      airspeed,
                                                      threshold,
                                                      random.NextDouble());

                Angle angleOfAttack = aircraftState.PitchAngle - aircraftState.FlightPathAngle;

                // Precondition
                double lift = AerodynamicsHelper.CalculateLift(aircraftData.AerodynamicsData,
                                                               angleOfAttack,
                                                               airDensity,
                                                               airspeed);
                double takeOffWeightNewton = aircraftData.TakeOffWeight * 1000; // N
                Assert.IsTrue(lift < takeOffWeightNewton);

                double thrust = random.NextDouble() * 1000;
                double drag = random.NextDouble() * 100;
                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, gravitationalAcceleration)
                {
                    Thrust = thrust,
                    Drag = drag
                };

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                double horizontalWeightComponent = takeOffWeightNewton * Math.Sin(aircraftState.FlightPathAngle.Radians);
                double expectedAcceleration = (gravitationalAcceleration * (thrust - drag - horizontalWeightComponent))
                                              / takeOffWeightNewton;
                Assert.AreEqual(expectedAcceleration, accelerations.TrueAirSpeedRate, tolerance);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAircraftStateHeightLowerThanThresholdAndLiftHigherThanTakeOffWeight_ReturnsExpectedTrueAirspeedRate(AircraftData aircraftData)
            {
                // Setup
                const double airspeed = 100.0;
                const double threshold = 0.01;

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      airspeed,
                                                      threshold - random.NextDouble(),
                                                      random.NextDouble());

                Angle angleOfAttack = aircraftState.PitchAngle - aircraftState.FlightPathAngle;

                // Precondition
                double lift = AerodynamicsHelper.CalculateLift(aircraftData.AerodynamicsData,
                                                               angleOfAttack,
                                                               airDensity,
                                                               airspeed);
                double takeOffWeightNewton = aircraftData.TakeOffWeight * 1000; // N
                Assert.IsTrue(lift > takeOffWeightNewton);

                double thrust = random.NextDouble() * 1000;
                double drag = random.NextDouble() * 100;
                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, gravitationalAcceleration)
                {
                    Thrust = thrust,
                    Drag = drag
                };

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                double horizontalWeightComponent = takeOffWeightNewton * Math.Sin(aircraftState.FlightPathAngle.Radians);
                double expectedAcceleration = (gravitationalAcceleration * (thrust - drag - horizontalWeightComponent))
                                              / takeOffWeightNewton;
                Assert.AreEqual(expectedAcceleration, accelerations.TrueAirSpeedRate, tolerance);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAircraftStateHeightSmallerThanThresholdAndLiftSmallerThanTakeOffWeight_ReturnsExpectedTrueAirspeedRate(AircraftData aircraftData)
            {
                // Setup
                const double airspeed = 10.0;
                const double threshold = 0.01;

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      airspeed,
                                                      threshold - random.NextDouble(),
                                                      random.NextDouble());

                Angle angleOfAttack = aircraftState.PitchAngle - aircraftState.FlightPathAngle;

                // Precondition
                double lift = AerodynamicsHelper.CalculateLift(aircraftData.AerodynamicsData,
                                                               angleOfAttack,
                                                               airDensity,
                                                               airspeed);
                double takeOffWeightNewton = aircraftData.TakeOffWeight * 1000; // N
                Assert.IsTrue(lift < takeOffWeightNewton);

                double thrust = random.NextDouble() * 1000;
                double drag = random.NextDouble() * 100;
                double frictionCoefficient = random.NextDouble();
                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, gravitationalAcceleration)
                {
                    Thrust = thrust,
                    Drag = drag,
                    RollDrag = frictionCoefficient
                };

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                double horizontalWeightComponent = takeOffWeightNewton * Math.Sin(aircraftState.FlightPathAngle.Radians);
                double groundDrag = frictionCoefficient * (aircraftData.TakeOffWeight * 1000 - lift);
                double expectedAcceleration = (gravitationalAcceleration * (thrust - drag - groundDrag - horizontalWeightComponent))
                                              / takeOffWeightNewton;
                Assert.AreEqual(expectedAcceleration, accelerations.TrueAirSpeedRate, tolerance);
            }
        }

        [TestFixture]
        public class CalculateFlightPathRate
        {
            [Test]
            public static void Calculate_WithAirspeedLowerThanThreshold_ReturnsExpectedZeroRate()
            {
                // Setup
                var random = new Random(21);
                AircraftData aircraftData = AircraftDataTestFactory.CreateRandomAircraftData();
                Angle angleOfAttack = AerodynamicsDataTestHelper.GetValidAngleOfAttack(aircraftData.AerodynamicsData);

                var aircraftState = new AircraftState(angleOfAttack,
                                                      new Angle(),
                                                      random.NextDouble(),
                                                      random.NextDouble(),
                                                      random.NextDouble());

                // Precondition
                Assert.IsTrue(aircraftState.TrueAirspeed < 1);

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, random.NextDouble(), random.NextDouble());

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                Assert.Zero(accelerations.FlightPathRate.Degrees);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAirspeedEqualToThresholdAndNormalForcePresent_ReturnsExpectedFlightPathAngleRate(AircraftData aircraftData)
            {
                // Setup
                const double velocityThreshold = 1.0;
                const double heightThreshold = 0.01;

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      velocityThreshold,
                                                      heightThreshold - random.NextDouble(),
                                                      random.NextDouble());

                Angle angleOfAttack = aircraftState.PitchAngle - aircraftState.FlightPathAngle;

                // Precondition
                double lift = AerodynamicsHelper.CalculateLift(aircraftData.AerodynamicsData,
                                                               angleOfAttack,
                                                               airDensity,
                                                               aircraftState.TrueAirspeed);
                double takeOffWeightNewton = aircraftData.TakeOffWeight * 1000; // N
                Assert.IsTrue(lift < takeOffWeightNewton);

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, gravitationalAcceleration);

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                Assert.Zero(accelerations.FlightPathRate.Degrees);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAirspeedEqualToThresholdAndNoNormalForcePresent_ReturnsExpectedFlightPathAngleRate(AircraftData aircraftData)
            {
                // Setup
                const double heightThreshold = 0.01;

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      100,
                                                      heightThreshold - random.NextDouble(),
                                                      random.NextDouble());

                Angle angleOfAttack = aircraftState.PitchAngle - aircraftState.FlightPathAngle;

                // Precondition
                double lift = AerodynamicsHelper.CalculateLift(aircraftData.AerodynamicsData,
                                                               angleOfAttack,
                                                               airDensity,
                                                               aircraftState.TrueAirspeed);
                double takeOffWeightNewton = aircraftData.TakeOffWeight * 1000; // N
                Assert.IsTrue(lift > takeOffWeightNewton);

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, gravitationalAcceleration);

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                double expectedRate = (gravitationalAcceleration * (lift - takeOffWeightNewton))
                                      / (takeOffWeightNewton * aircraftState.TrueAirspeed);
                Assert.AreEqual(expectedRate, accelerations.FlightPathRate.Radians, tolerance);
            }
        }

        [TestFixture]
        public class CalculatePitchRate
        {
            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public void Calculate_WithAircraftStateAndSpeedHigherThanRotationSpeedAndPitch_ReturnsExpectedPitchRate(AircraftData aircraftData)
            {
                // Setup
                var random = new Random(21);
                double rotationSpeed = GetRotationSpeed(aircraftData);
                double pitchAngle = aircraftData.MaximumPitchAngle.Degrees - random.NextDouble();

                var aircraftState = new AircraftState(Angle.FromDegrees(pitchAngle),
                                                      random.NextAngle(),
                                                      rotationSpeed + random.NextDouble(),
                                                      random.NextDouble(),
                                                      random.NextDouble());

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, random.NextDouble());

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                Angle expectedPitchRate = aircraftData.PitchAngleGradient;
                Assert.AreEqual(expectedPitchRate, accelerations.PitchRate);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WitAircraftStateSpeedLowerThanRotationSpeed_ReturnsExpectedZeroPitchRate(AircraftData aircraftData)
            {
                // Setup
                double rotationSpeed = GetRotationSpeed(aircraftData);

                var random = new Random(21);
                var aircraftState = new AircraftState(random.NextAngle(),
                                                      random.NextAngle(),
                                                      rotationSpeed - random.NextDouble(),
                                                      random.NextDouble(),
                                                      random.NextDouble());

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, random.NextDouble());

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                Assert.Zero(accelerations.PitchRate.Degrees);
            }

            [Test]
            [TestCaseSource(typeof(AircraftTestData), nameof(AircraftTestData.GetAircraftData))]
            public static void Calculate_WithAircraftStateSpeedHigherThanRotationSpeedAndPitchAngleAtMaximumPitch_ReturnsExpectedZeroPitchRate(AircraftData aircraftData)
            {
                // Setup
                double rotationSpeed = GetRotationSpeed(aircraftData);

                var random = new Random(21);
                var aircraftState = new AircraftState(aircraftData.MaximumPitchAngle,
                                                      random.NextAngle(),
                                                      rotationSpeed + random.NextDouble(),
                                                      random.NextDouble(),
                                                      random.NextDouble());

                var calculator = new TestTakeOffDynamicsCalculator(aircraftData, airDensity, random.NextDouble());

                // Call 
                AircraftAccelerations accelerations = calculator.Calculate(aircraftState);

                // Assert
                Assert.Zero(accelerations.PitchRate.Degrees);
            }

            private static double GetRotationSpeed(AircraftData aircraftData)
            {
                double stallSpeed = AerodynamicsHelper.CalculateStallSpeed(aircraftData.AerodynamicsData,
                                                                           aircraftData.TakeOffWeight * 1000,
                                                                           airDensity);
                double rotationSpeed = stallSpeed * 1.2;
                return rotationSpeed;
            }
        }
    }

    public class TestTakeOffDynamicsCalculator : TakeOffDynamicsCalculatorBase
    {
        public TestTakeOffDynamicsCalculator(AircraftData aircraftData, double density, double gravitationalAcceleration)
            : base(aircraftData, density, gravitationalAcceleration) {}

        public double Thrust { private get; set; }

        public double RollDrag { private get; set; }
        public double Drag { private get; set; }
        public AircraftState CalculateDragInput { get; private set; }

        protected override double GetFrictionCoefficient()
        {
            return RollDrag;
        }

        protected override double CalculateThrustForce()
        {
            return Thrust;
        }

        protected override double CalculateAerodynamicDragForce(AircraftState state)
        {
            CalculateDragInput = state;
            return Drag;
        }
    }
}