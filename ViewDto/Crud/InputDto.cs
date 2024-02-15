namespace Buratino.ViewDto.Crud
{
    public class InputDto
    {
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public object Value { get; set; }
        
        public bool IsEditable { get; set; }

        public InputDto(string name, string displayName, object value, bool isEditable)
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
            IsEditable = isEditable;
        }

        public string GetHtmlDecimal()
        {
            return Value?.ToString()?.Replace(".", ",") ?? "0,0";
        }
    }
}
