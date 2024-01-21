using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.DTOs
{
    public partial class ParameterElement : GodotObject
    {
        public string Key { get; set; }
        public object Element { get; set; }

        public ParameterElement(string key, object element)
        {
            Key = key;
            Element = element;
        }
    }

    public static class ArrayExtensions
    {
        public static Dictionary<string, object> ToParamDictionary(this Godot.Collections.Array<ParameterElement> array)
        {
            var dict = new Dictionary<string, object>();

            var groups = array.GroupBy(x => x.Key);

            if (groups.Any(g => g.Count() != 1))
            {
                return dict;
            }

            foreach (var element in array)
            {
                dict.Add(element.Key, element.Element);
            }

            return dict;
        }
    }
}
