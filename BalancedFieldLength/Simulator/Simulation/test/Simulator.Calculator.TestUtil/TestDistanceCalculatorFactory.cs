﻿using System;
using Simulator.Calculator.Factories;
using Simulator.Calculator.Integrators;
using Simulator.Data;

namespace Simulator.Calculator.TestUtil
{
    public class TestDistanceCalculatorFactory : IDistanceCalculatorFactory
    {
        public IDistanceCalculator CreateContinuedTakeOffDistanceCalculator(ITakeOffDynamicsCalculatorFactory takeOffDynamicsCalculatorFactory,
                                                                           AircraftData data,
                                                                           IIntegrator integrator,
                                                                           int nrOfFailedEngines,
                                                                           double density,
                                                                           double gravitationalAcceleration,
                                                                           CalculationSettings calculationSettings)
        {
            throw new NotImplementedException();
        }

        public IDistanceCalculator CreateAbortedTakeOffDistanceCalculator(ITakeOffDynamicsCalculatorFactory takeOffDynamicsCalculatorFactory,
                                                                         AircraftData data,
                                                                         IIntegrator integrator,
                                                                         double density,
                                                                         double gravitationalAcceleration,
                                                                         CalculationSettings calculationSettings)
        {
            throw new NotImplementedException();
        }
    }
}