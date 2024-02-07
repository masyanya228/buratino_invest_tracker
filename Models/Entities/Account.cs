using BCrypt.Net;

using Buratino.Models.Attributes;
using Buratino.Models.Entities.Abstractions;
using Buratino.Models.Xtensions;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Models.Entities
{
    public class Account : GeoLoc
    {
        [TitleDescribe("Пароль", "")]
        public virtual string Pass { get; set; }

        [TitleDescribe("Фамилия","")]
        [RegularExpression(@"^([а-яА-Яё-]{2,24})$", ErrorMessage = "Фамилия может состоять только из русский букв")]
        public virtual string Surname { get; set; }

        [TitleDescribe("Отчество", "")]
        [RegularExpression(@"^([а-яА-Яё-]{2,24})$", ErrorMessage = "Отчество может состоять только из русский букв")]
        public virtual string LastName { get; set; }


        [TitleDescribe("Почта", "")]
        public virtual string Email { get; set; }


        [TitleDescribe("Ключ 2FA", "Ключ двухфакторной аутентификации")]
        public virtual string TwoFAkey { get; set; }

        [TitleDescribe("Сдвиг времени", "")]
        public virtual decimal TimeOffsetInHours { get; set; }


        [TitleDescribe("Последняя аутентификация")]
        public virtual DateTime LastEnter { get; set; }


        [TitleDescribe("Дата регистрации")]
        public virtual DateTime RegStamp { get; set; }


        [TitleDescribe("Подтвержден")]
        public virtual bool IsReg { get; set; }


        [TitleDescribe("Почта на подтверждении")]
        public virtual string Newmail { get; set; }

        
        [TitleDescribe("Код подтверждения почты")]
        public virtual string EmailVerificationToken { get; set; }


        [TitleDescribe("Код сброса пароля")]
        public virtual string PassResetToken { get; set; }


        [TitleDescribe("Флаг 2FA")]
        public virtual bool TwoFAkeyAprooved { get; set; }

        
        [TitleDescribe("Заблокирован?")]
        public virtual bool IsBlocked { get; set; }

        
        [TitleDescribe("Временная блокировка истекает")]
        public virtual DateTime DynamicBlockExpiring { get; set; }

        [TitleDescribe("Причина блокировки")]
        public virtual string BlockReason { get; set; }


        [TitleDescribe("Путь до аватара")]
        public virtual string AvatarPath { get; set; } = null;


        [TitleDescribe("Имя фотографа")]
        public virtual string WokerName { get; set; }


        [TitleDescribe("Привязанный Telegram-аккаунт")]
        public virtual string ChatId { get; set; }


        public virtual bool IsAdmin { get; set; }


        [TitleDescribe("Список городов")]
        public virtual IEnumerable<string> City { get; set; } = new string[0];


        [TitleDescribe("Вацап для связи", "Номер телефона, к которому привязан вацап. Используется как способ связи с пользователем для других пользователей")]
        [RegularExpression(@"^((\+7|7|8)?([0-9]){10})$", ErrorMessage = "Укажите номер телефона в верном формате")]
        public virtual string WhatsAppPhone { get; set; }


        [TitleDescribe("Telegram для связи", "Только Username. Используется как способ связи с пользователем для других пользователей")]
        [RegularExpression(@"^[^0-9]+\w{4,}", ErrorMessage = "Укажите Username из Телеграма в верном формате")]
        public virtual string TelegramUserName { get; set; }


        [TitleDescribe("Токен для привязки телеграма", "Используется 1 раз для привязки телеграмм-аккаунта")]
        public virtual string TGVerificationToken { get; set; }


        [TitleDescribe("Время начала регистрации", "")]
        public virtual DateTime FastRegStartTime { get; set; }


        [TitleDescribe("Продолжительность регистрации", "")]
        public virtual double FastRegLen { get; set; }


        public virtual void SetPass(string source)
        {
            SetHashedPass(source.GetSHA512());
        }

        public virtual void SetHashedPass(string hashpass)
        {
            string mySalt = "$2a$09$DR3j6r8kI6wCAYsPXbzQ3u";
            Pass = BCrypt.Net.BCrypt.HashPassword(hashpass, mySalt);
        }

        public override string ToString()
        {
            return $"{Login}" + (Name != null ? $"({Name})" : "");
        }

        public virtual bool TryAutenticate(string sourcePass)
        {
            if (sourcePass.NullOrEmpty() == null)
                return false;
            return CheckPass(sourcePass.GetSHA512());
        }

        public virtual bool CheckPass(string hashpass)
        {
            if (hashpass.NullOrEmpty() == null)
                return false;
            if (Pass.NullOrEmpty() == null)
                return false;
            if (IsBlocked)
                return false;
            try
            {
                return BCrypt.Net.BCrypt.Verify(hashpass, Pass);
            }
            catch (SaltParseException ex)
            {
                return false;
            }
        }
    }
}