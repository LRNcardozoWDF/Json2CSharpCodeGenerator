using System;
using System.Text;

namespace Xamasoft.JsonClassGenerator
{
    public interface IJsonClassGeneratorConfig
    {
        string       Namespace                  { get; set; }
        string       SecondaryNamespace         { get; set; }
        bool         UseFields                  { get; set; }
        bool         InternalVisibility         { get; set; }
        bool         ExplicitDeserialization    { get; set; }
        bool         NoHelperClass              { get; set; }
        string       MainClass                  { get; set; }
        string       InputFileName                  { get; set; }
        string       InputFileAreaName                  { get; set; }
        string       InputFileProductName                  { get; set; }
        bool       isInput                  { get; set; }
        bool         UseProperties              { get; set; }
        bool         UsePascalCase              { get; set; }
        
        /// <summary>Use the <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on generated C# class properties (as opposed to not rendering any attributes, or using <see cref="UseJsonPropertyName"/>).</summary>
        bool         UseJsonAttributes          { get; set; }
        bool UseJsonPropertyNamesForSDK { get; set; }
        bool UseJsonPropertyNamesForMSA { get; set; }

        /// <summary>Use the <c>[JsonPropertyName]</c> attribute on generated C# class properties (as opposed to not rendering any attributes, or using <see cref="UseJsonAttributes"/>).</summary>
        bool         UseJsonPropertyName        { get; set; }

        bool         UseNestedClasses           { get; set; }
        bool         ApplyObfuscationAttributes { get; set; }
        bool         SingleFile                 { get; set; }
        ICodeBuilder CodeWriter                 { get; set; }
        bool         HasSecondaryClasses        { get; }
        bool         AlwaysUseNullableValues    { get; set; }
        bool         UseNamespaces              { get; }
        bool         ExamplesInDocumentation    { get; set; }
        bool         ImmutableClasses           { get; set; }
        bool         NoSettersForCollections    { get; set; }

        bool ArrayAsList();
    }

    public static class JsonClassGeneratorConfigExtensions
    {
        /// <summary>Never returns <see langword="null"/>. Returns either &quot;<c>[JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; or &quot;<c>[JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; - or an empty string depending on <paramref name="config"/> and <see cref="FieldInfo.ContainsSpecialChars"/>.</summary>
        /// <param name="config">Required. Cannot be <see langword="null"/>.</param>
        /// <param name="field">Required. Cannot be <see langword="null"/>.</param>
        /// <returns></returns>
        public static string GetCSharpJsonAttributeCode(this IJsonClassGeneratorConfig config, FieldInfo field)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));
            if (field is null) throw new ArgumentNullException(nameof(field));

           // if (config.UseJsonAttributes && config.UseJsonPropertyName) throw new ArgumentException(message: "Cannot use both " + nameof(config.UseJsonPropertyName) + " and " + nameof(config.UseJsonAttributes) + ".", paramName: nameof(config));

            string name = field.JsonMemberName;

            if (config.UseJsonPropertyNamesForSDK)
            {
                if (name.Contains("_"))
                {
                    name = ToTitleCase(name);
                    name = char.ToLowerInvariant(name[0]) + name.Substring(1);
                }

                return $"[JsonProperty(\"{name}\")]";
            }
            if (config.UseJsonPropertyName || config.UseJsonPropertyNamesForMSA)
            {
                return "[JsonPropertyName(\"" + name + "\")]";
            }
            else if (config.UseJsonAttributes || field.ContainsSpecialChars)
            {
                return "[JsonProperty(\"" + name + "\")]";
            }
            else
            {
                return String.Empty;
            }
        }

        internal static string ToTitleCase(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            Boolean flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                Char c = str[i];
                string specialCaseFirstCharIsNumber = string.Empty;

                // Handle the case where the first character is a number
                if (i == 0 && char.IsDigit(c))
                    specialCaseFirstCharIsNumber = "_" + c;

                if (char.IsLetterOrDigit(c))
                {
                    if (string.IsNullOrEmpty(specialCaseFirstCharIsNumber))
                        sb.Append(flag ? char.ToUpper(c) : c);
                    else
                        sb.Append(flag ? specialCaseFirstCharIsNumber.ToUpper() : specialCaseFirstCharIsNumber);

                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }
    }
}
