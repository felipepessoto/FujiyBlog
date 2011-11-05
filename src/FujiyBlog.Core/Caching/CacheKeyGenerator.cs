using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace FujiyBlog.Core.Caching
{
    internal static class CacheKeyGenerator
    {
        private static readonly ValidType[] ValidWrappingGenericTypes = new[] { new ValidType(typeof(Nullable<>)) };
        private static readonly ValidType[] ValidTypes = new[]
                             {
                                 new ValidType(typeof (byte)),
                                 new ValidType(typeof (sbyte)),
                                 new ValidType(typeof (int)),
                                 new ValidType(typeof (uint)),
                                 new ValidType(typeof (short)),
                                 new ValidType(typeof (ushort)),
                                 new ValidType(typeof (long)),
                                 new ValidType(typeof (ulong)),
                                 new ValidType(typeof (float)),
                                 new ValidType(typeof (double)),
                                 new ValidType(typeof (char)),
                                 new ValidType(typeof (bool)),
                                 new ValidType(typeof (string)),
                                 new ValidType(typeof (decimal)),
                                 new ValidType(typeof (DateTime)),
                                 new ValidType(typeof (DateTimeOffset)),
                                 new ValidType(typeof (Guid))
                             };

        internal static string GenerateKey(MethodCallExpression method)
        {
            ValidateArguments(method);

            string chave = method.Method.ReflectedType.FullName + ": " + method.Method + ". Param Count:" + method.Arguments.Count + ". Params:";

            object[] valoresArgumentos = new object[method.Arguments.Count];

            for (int i = 0; i < valoresArgumentos.Length; i++)
            {
                valoresArgumentos[i] = GetArgumentValue(method.Arguments[i]);
            }

            for (int i = 0; i < valoresArgumentos.Length; i++)
            {
                chave += FormatValue(valoresArgumentos[i]) + ",";
            }

            return chave.TrimEnd(new[] { ',' });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
        private static void ValidateArguments(MethodCallExpression method)
        {
            for (int i = 0; i < method.Arguments.Count; i++)
            {
                if (!IsValidType(method.Arguments[i].Type, true))
                {
                    throw new InvalidCacheArgumentException(string.Format(CultureInfo.CurrentCulture, "It was not possible to derive a key from the parameters. Index of the parameter: {0}. Type: {1}", i, method.Arguments[i].Type));
                }
            }
        }

        private static bool IsValidType(Type type, bool validateGeneric)
        {
            if (validateGeneric && type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();

                if (ValidWrappingGenericTypes.Any(t => genericType == t.Type))
                {
                    return type.GetGenericArguments().All(x => IsValidType(x, false));
                }
            }

            if (typeof(Enum).IsAssignableFrom(type))
            {
                return true;
            }

            return ValidTypes.Any(t => type == t.Type);
        }

        private static string FormatValue(object value)
        {
            if (value == null)
                return null;

            if (value is byte)
                return XmlConvert.ToString((byte)value);
            if (value is sbyte)
                return XmlConvert.ToString((sbyte)value);
            if (value is int)
                return XmlConvert.ToString((int)value);
            if (value is uint)
                return XmlConvert.ToString((uint)value);
            if (value is short)
                return XmlConvert.ToString((short)value);
            if (value is ushort)
                return XmlConvert.ToString((ushort)value);
            if (value is long)
                return XmlConvert.ToString((long)value);
            if (value is ulong)
                return XmlConvert.ToString((ulong)value);
            if (value is float)
                return XmlConvert.ToString((float)value);
            if (value is double)
                return XmlConvert.ToString((double)value);
            if (value is char)
                return XmlConvert.ToString((char)value);
            if (value is bool)
                return XmlConvert.ToString((bool)value);
            string stringValue = value as string;
            if (stringValue != null)
                return stringValue;
            if (value is decimal)
                return XmlConvert.ToString((decimal)value);
            if (value is DateTime)
                return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind);
            if (value is DateTimeOffset)
                return XmlConvert.ToString((DateTimeOffset)value);
            if (value is Guid)
                return XmlConvert.ToString((Guid)value);
            if (value is Enum)
                return value.ToString();

            throw new InvalidCacheArgumentException();
        }

        private static object GetArgumentValue(Expression element)
        {
            LambdaExpression l = Expression.Lambda(Expression.Convert(element, element.Type));
            return l.Compile().DynamicInvoke();
        }

        private class ValidType
        {
            public readonly Type Type;
            //public readonly bool AcceptSubClass;

            public ValidType(Type type)
            {
                Type = type;
            }

            //public ValidType(Type type, bool acceptSubClass)
            //{
            //    Type = type;
            //    AcceptSubClass = acceptSubClass;
            //}
        }
    }
}
