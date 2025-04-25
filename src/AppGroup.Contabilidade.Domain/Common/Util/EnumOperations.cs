using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AppGroup.Contabilidade.Domain.Common.Util;

/// <summary>
/// Classe de operações com enum
/// </summary>
public static class EnumOperations
{
    /// <summary>
    /// Converte o enum para um array de inteiro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int[] ToIntArray<T>()
    {
        return (int[])Enum.GetValues(typeof(T));
    }

    public static TEnum GetEnumValueFromDefaultValue<TEnum>(string defaultValue)
    {
        foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                DefaultValueAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

                if (attributes != null && attributes.Length > 0 && attributes[0].Value.ToString() == defaultValue)
                {
                    return enumValue;
                }
            }
        }

        throw new ArgumentException($"No enum value found with the default value: {defaultValue}");
    }

    /// <summary>
    /// Converte o enum para um array de string das DefaultValue
    /// </summary>
    /// <returns></returns>
    public static List<string> ToDefaultValueArray<T>()
    {
        List<string> enumNames = new List<string>();
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            FieldInfo? field = typeof(T).GetField(value.ToString() ?? throw new InvalidOperationException());
            DefaultValueAttribute attribute = (field ?? throw new InvalidOperationException("")).GetCustomAttribute<DefaultValueAttribute>() ?? throw new InvalidOperationException();

            if (attribute.Value != null) enumNames.Add(attribute.Value.ToString() ?? throw new InvalidOperationException());
        }

        return enumNames;
    }

    /// <summary>
    /// recupera a descrição do enum
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum @enum)
    {
        var e = @enum.GetType().GetField(@enum.ToString());
        var attributes = (DescriptionAttribute[])e?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;
        return attributes != null && attributes.Length > 0 ? attributes[0].Description : @enum.ToString();
    }

    /// <summary>
    /// Recupera o valor default do enum
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string GetDefaultValue(this Enum @enum)
    {
        var e = @enum.GetType().GetField(@enum.ToString());
        var attributes = (DefaultValueAttribute[])e?.GetCustomAttributes(typeof(DefaultValueAttribute), false)!;
        return attributes != null && attributes.Length > 0 ? attributes[0].Value?.ToString() : @enum.ToString();
    }

    /// <summary>
    /// Recupera um valor do inteiro do enum
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>

    public static int GetIntValue(this Enum @enum)
    {
        return Convert.ToInt32(@enum);
    }

    /// <summary>
    /// Converte uma string em um enum
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>

    public static bool TryConvertToEnum<TEnum>(this string value, out TEnum result) where TEnum : struct, Enum
    {
        result = default(TEnum);
        return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out result);
    }
}

public class EnumDataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedEnumValuesAttribute : ValidationAttribute
    {
        private readonly object[] _allowedValues;

        public AllowedEnumValuesAttribute(Type enumType, params object[] allowedValues)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                throw new ArgumentException("The type needs to be enum.");
            }

            _allowedValues = allowedValues;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (_allowedValues.Contains(value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(string.IsNullOrWhiteSpace(ErrorMessage) ? $"O valor '{value}' não é um valor permitido." : ErrorMessage);
        }
    }
}