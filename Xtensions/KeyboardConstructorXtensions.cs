using Buratino.Entities;
using Buratino.Helpers;

namespace Buratino.Xtensions
{
    public static class KeyboardConstructorXtensions
    {
        public static InlineKeyboardConstructor AddButtonIf(this InlineKeyboardConstructor constructor, Func<bool> func, string title, string callbackQuery)
            => func()
                ? constructor.AddButtonDown(title, callbackQuery)
                : constructor;
    }
}
