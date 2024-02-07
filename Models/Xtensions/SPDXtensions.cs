using Buratino.Models.Helpers;

using LiteDB;

using System.Reflection;

namespace Buratino.Models.Xtensions
{
    public static class SPDXtensions
    {
        public static object Cast(this object obj, Type target)
        {
            if (obj == null)
            {
                return null;
            }

            Type source = obj.GetType();
            if (source == target)
            {
                return obj;
            }
            else if (target == typeof(object))
            {
                return obj;
            }
            else if (target.IsAssignableFrom(source))
            {
                return obj;
            }
            else if (target == typeof(string))
            {
                return obj.ToString();
            }
            else if (target.IsEnum)
            {
                if (int.TryParse(obj.ToString(), out int valInt))
                {
                    foreach (var item in Enum.GetValues(target))
                    {
                        if ((int)item == valInt)
                        {
                            return item;
                        }
                    }
                    throw new Exception("Не получилось преобразовать числовой ключ в Enum");
                }
                else
                {
                    if (Enum.TryParse(target, obj.ToString(), out object res))
                    {
                        return res;
                    }
                    else
                    {
                        throw new Exception("Не получилось преобразовать текстовый ключ в Enum");
                    }
                }
            }
            else if (source == typeof(string))
            {
                if (obj is null || obj.ToString() == "")
                {
                    return null;
                }

                var enumerable = target.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == "TryParse").ToArray();
                var method = enumerable.First();
                var data = new object[] { obj, null };
                var res = (bool)enumerable[0].Invoke(null, data);
                if (res)
                {
                    return data[1];
                }
                else
                {
                    throw new Exception($"Не удалось преобразовать {obj} в {target}");
                }
            }
            else
            {
                var resolve = Convertation.Convertations.FirstOrDefault(x => x.A == source && x.B == target);
                if (resolve == null)
                    throw new Exception($"Нет преобразования для {source} в {target} ({obj?.ToString() ?? "|e|"})");
                else
                {
                    object res = resolve.GetResult(obj);
                    return res;
                }
            }
        }
    }
}