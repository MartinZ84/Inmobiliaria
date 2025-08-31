using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using System.Reflection;

namespace Inmobiliaria.Helpers
{
    public static class HtmlEnumExtensions
    {
        // private static string GetDescription(Enum value)
        // {
        //     var field = value.GetType().GetField(value.ToString());
        //     var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
        //     return attribute?.Description ?? value.ToString();
        // }
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            string optionLabel,
            object htmlAttributes = null,
            params TEnum[] exclude)   // <-- nuevos parámetros para excluir
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            var values = Enum.GetValues(enumType).Cast<Enum>()
                .Where(e => !exclude.Contains((TEnum)e)) // <-- filtramos los excluidos
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetDescription(e)
                });

            return htmlHelper.DropDownListFor(expression, values, optionLabel, htmlAttributes);
        }

    }

    
}
