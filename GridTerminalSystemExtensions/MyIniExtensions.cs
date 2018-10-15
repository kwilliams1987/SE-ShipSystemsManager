using System;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public static class MyIniExtensions
    {
        public static T ToType<T>(this MyIniValue value)
        {
            var type = typeof(T);
            if (type == typeof(String))
            {
                return (T)Convert.ChangeType(value.ToString(), type);
            }
            else if (type == typeof(Int16))
            {
                return (T)Convert.ChangeType(value.ToInt16(), type);
            }
            else if (type == typeof(UInt16))
            {
                return (T)Convert.ChangeType(value.ToUInt16(), type);
            }
            else if (type == typeof(Int32))
            {
                return (T)Convert.ChangeType(value.ToInt32(), type);
            }
            else if (type == typeof(UInt32))
            {
                return (T)Convert.ChangeType(value.ToUInt32(), type);
            }
            else if (type == typeof(Int64))
            {
                return (T)Convert.ChangeType(value.ToInt64(), type);
            }
            else if (type == typeof(UInt64))
            {
                return (T)Convert.ChangeType(value.ToUInt64(), type);
            }
            else if (type == typeof(Single))
            {
                return (T)Convert.ChangeType(value.ToSingle(), type);
            }
            else if (type == typeof(Color))
            {
                return (T)Convert.ChangeType(new Color(value.ToSingle()), type);
            }
            else if (type == typeof(Boolean))
            {
                return (T)Convert.ChangeType(value.ToBoolean(), type);
            }
            else if (type == typeof(Double))
            {
                return (T)Convert.ChangeType(value.ToDouble(), type);
            }
            else if (type == typeof(Byte))
            {
                return (T)Convert.ChangeType(value.ToByte(), type);
            }
            else if (type == typeof(SByte))
            {
                return (T)Convert.ChangeType(value.ToSByte(), type);
            }
            else
            {
                return default(T);
            }
        }

        public static void Set(this MyIni ini, String section, String key, Object value)
        {
            var type = value.GetType();
            if (type == typeof(String))
            {
                ini.Set(section, key, value.ToString());
            }
            else if (type == typeof(Int16))
            {
                ini.Set(section, key, (Int16)value);
            }
            else if (type == typeof(UInt16))
            {
                ini.Set(section, key, (UInt16)value);
            }
            else if (type == typeof(Int32))
            {
                ini.Set(section, key, (Int32)value);
            }
            else if (type == typeof(UInt32))
            {
                ini.Set(section, key, (UInt32)value);
            }
            else if (type == typeof(Int64))
            {
                ini.Set(section, key, (Int64)value);
            }
            else if (type == typeof(UInt64))
            {
                ini.Set(section, key, (UInt64)value);
            }
            else if (type == typeof(Single))
            {
                ini.Set(section, key, (Single)value);
            }
            else if (type == typeof(Color))
            {
                ini.Set(section, key, ((Color)value).PackedValue);
            }
            else if (type == typeof(Boolean))
            {
                ini.Set(section, key, (Boolean)value);
            }
            else if (type == typeof(Double))
            {
                ini.Set(section, key, (Double)value);
            }
            else if (type == typeof(Byte))
            {
                ini.Set(section, key, (Byte)value);
            }
            else if (type == typeof(SByte))
            {
                ini.Set(section, key, (SByte)value);
            }
            else
            {
                throw new Exception($"{nameof(MyIni)} cannot serialize objects of type {type.FullName}.");
            }
        }
    }
}
