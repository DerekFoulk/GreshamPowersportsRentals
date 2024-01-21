﻿using System.ComponentModel;
using System.Globalization;
using Data.Specialized.Entities;
using Data.Specialized.Models;

namespace Data.Specialized.Converters
{
    public class DimensionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string)
            {
                var name = "ConverterFromName";
                var imageUrl = "ConverterFromImageUrl";

                return new Dimension(name, imageUrl);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var name = "ConverterToName";
                var imageUrl = "ConverterToImageUrl";

                return new Dimension(name, imageUrl);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
