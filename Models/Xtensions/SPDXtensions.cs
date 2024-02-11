using Buratino.Entities.Abstractions;
using Buratino.Models.Helpers;

using LiteDB;

using System.Reflection;

namespace Buratino.Models.Xtensions
{
    public static class SPDXtensions
    {
        public static object StringValueCast(this string source, Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Name == "Nullable`1")
            {
                type = type.GenericTypeArguments.FirstOrDefault();
                if (source == null)
                    return null;
            }

            if (type.IsAssignableTo(typeof(EntityBase)))
            {
                var entity = Activator.CreateInstance(type) as EntityBase;
                if (Guid.TryParse(source, out Guid entityId))
                {
                    entity.Id = entityId;
                }
                else
                {
                    return null;
                }
                return entity;
            }
            else if (type.IsEnum)
            {
                if (Enum.TryParse(type, source, out object res))
                {
                    return res;
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return source;
                }
                else if (type == typeof(int))
                {
                    if (int.TryParse(source, out int res))
                        return res;
                }
                else if (type == typeof(long))
                {
                    if (long.TryParse(source, out long res))
                        return res;
                }
                else if (type == typeof(decimal))
                {
                    if (decimal.TryParse(source, out decimal res))
                        return res;
                }
                else if (type == typeof(double))
                {
                    if (double.TryParse(source, out double res))
                        return res;
                }
                
                else if (type == typeof(DateTime))
                {
                    if (DateTime.TryParse(source, out DateTime res))
                        return res;
                }
            }
            return null;
        }

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