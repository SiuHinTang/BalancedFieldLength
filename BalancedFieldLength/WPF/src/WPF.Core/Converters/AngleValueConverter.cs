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
using System.Globalization;
using System.Windows.Data;
using Core.Common.Data;

namespace WPF.Core.Converters
{
    /// <summary>
    /// Value converter which converts <see cref="Angle"/>.
    /// </summary>
    public class AngleValueConverter : IValueConverter
    {
        private const NumberStyles numberStyle = NumberStyles.AllowLeadingSign
                                                 | NumberStyles.AllowExponent
                                                 | NumberStyles.AllowDecimalPoint
                                                 | NumberStyles.AllowThousands;

        /// <summary>
        /// Gets or sets whether the converter should convert degrees.
        /// </summary>
        public bool IsDegrees { get; set; }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="value"/> is not of type <see cref="Angle"/>.</item>
        /// <item>the <paramref name="targetType"/> is not of type <see cref="string"/>
        /// or of type <see cref="object"/>.</item>
        /// </list></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Angle))
            {
                throw new NotSupportedException($"Conversion from {value?.GetType().Name} is not supported.");
            }

            if (targetType != typeof(string) && targetType != typeof(object))
            {
                throw new NotSupportedException($"Conversion to {targetType.Name} is not supported.");
            }

            var angleToConvert = (Angle) value;
            double valueToConvert = IsDegrees
                                        ? angleToConvert.Degrees
                                        : angleToConvert.Radians;
            return double.IsNaN(valueToConvert)
                       ? string.Empty
                       : valueToConvert.ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="targetType"/>
        /// is not of type <see cref="Angle"/>.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Angle))
            {
                throw new NotSupportedException($"Conversion to {targetType.Name} is not supported.");
            }

            string stringValue = value as string;
            if (stringValue != null)
            {
                string trimmedString = stringValue.Trim();

                double parsedValue;
                if (double.TryParse(trimmedString, numberStyle, CultureInfo.InvariantCulture, out parsedValue))
                {
                    return IsDegrees ? Angle.FromDegrees(parsedValue) : Angle.FromRadians(parsedValue);
                }

                return Angle.FromRadians(double.NaN);
            }

            return null;
        }
    }
}