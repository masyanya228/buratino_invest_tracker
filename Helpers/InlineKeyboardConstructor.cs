using Telegram.Bot.Types.ReplyMarkups;

namespace Buratino.Helpers
{
    public class InlineKeyboardConstructor
    {
        public bool IsVertical { get; set; }
        public List<List<InlineKeyboardButton>> KeyboardButtons { get; set; } = new();

        public InlineKeyboardConstructor()
        {
        }

        public InlineKeyboardConstructor(IEnumerable<InlineKeyboardButton> verticalStack)
        {
            foreach (var item in verticalStack)
            {
                KeyboardButtons.Add(new() { item });
            }
        }

        public InlineKeyboardConstructor AddButtonRight(string title, string callbackData)
        {
            if (!KeyboardButtons.Any())
            {
                KeyboardButtons.Add(new());
            }
            KeyboardButtons.Last().Add(new InlineKeyboardButton(title) { CallbackData = callbackData });
            return this;
        }

        public InlineKeyboardConstructor AddButtonDown(string title, string callbackData)
        {
            KeyboardButtons.Add(new() { new InlineKeyboardButton(title) { CallbackData = callbackData } });
            return this;
        }

        public InlineKeyboardMarkup GetMarkup()
        {
            return new InlineKeyboardMarkup(KeyboardButtons);
        }
    }
}
