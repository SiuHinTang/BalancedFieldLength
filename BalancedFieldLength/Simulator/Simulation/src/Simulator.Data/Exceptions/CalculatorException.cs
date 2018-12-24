﻿using System;
using System.Runtime.Serialization;

namespace Simulator.Data.Exceptions
{
    /// <summary>
    /// Exception thrown when the calculator cannot continue the calculation.
    /// </summary>
    [Serializable]
    public class CalculatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorException"/> class.
        /// </summary>
        public CalculatorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CalculatorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public CalculatorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CalculatorException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        protected CalculatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}