using System;
using System.Globalization;
using System.Windows.Input;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="bool"/> to a cursor
    /// </summary>
    class BooleanIsSelectedToCursorConverter : BaseValueConverter<BooleanIsSelectedToCursorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Cursors.Arrow : Cursors.Hand;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
