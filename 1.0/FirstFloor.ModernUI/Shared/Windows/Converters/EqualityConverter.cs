using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FirstFloor.ModernUI.Windows.Converters
{
    /// <summary>
    /// Converts multiple values to true when all values are equal, otherwise false.
    /// </summary>
    public class EqualityConverter
        : IMultiValueConverter
    {
        /// <summary>
        /// Converts source values to a value for the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings produces.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// True if all values are equal, otherwise false.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) {
                return false;
            }
            return values.Skip(1).All(v => Equals(values[0], v));
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
