﻿using Record.Recorder.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view page
    /// </summary>
    class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPage) value)
            {
                case ApplicationPage.MainPage:
                    return new MainPage();

                case ApplicationPage.SettingsPage:
                    return new SettingsPage();
              

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
