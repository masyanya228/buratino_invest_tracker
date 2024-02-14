using Buratino.Entities.Abstractions;
using Buratino.Models.Xtensions;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Buratino.ViewDto.Crud
{
    public class CrudListDto
    {
        public string ListName { get; set; }
        
        public Type EntityType { get; set; }
        
        public IEnumerable<IEntityBase> EntityList { get; set; }

        public IEnumerable<ColumnSettings> ColumnSettings { get; set; }
    }

    public class ColumnSettings
    {
        public string Index { get; set; }

        public string Name { get; set; }

        public int WidthPx { get; set; }

        public double WidthPercent { get; set; }

        public Type PropertyType { get { return PropertyInfo.PropertyType; } }


        private PropertyInfo PropertyInfo;

        public ColumnSettings(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            var display = PropertyInfo.GetAttribute<DisplayAttribute>();
            Name = display?.Name ?? PropertyInfo.Name;
            Index = PropertyInfo.Name;
        }

        public ColumnSettings(PropertyInfo propertyInfo, int widthPx) : this(propertyInfo)
        {
            WidthPx = widthPx;
        }

        public ColumnSettings(PropertyInfo propertyInfo, double widthPercent) : this(propertyInfo)
        {
            WidthPercent = widthPercent;
        }

        public object GetValue(object item)
        {
            return PropertyInfo.GetValue(item);
        }
    }
}
