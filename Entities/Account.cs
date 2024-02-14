using BCrypt.Net;
using Buratino.Entities.Abstractions;
using Buratino.Models.Attributes;
using Buratino.Models.Xtensions;

using System.ComponentModel.DataAnnotations;

namespace Buratino.Entities
{
    public class Account : GeoLoc
    {
        [TitleDescribe("Почта", "")]
        public virtual string Email { get; set; }

        [TitleDescribe("Пароль", "")]
        public virtual string Pass { get; set; }

        [TitleDescribe("Фамилия", "")]
        [RegularExpression(@"^([а-яА-Яё-]{2,24})$", ErrorMessage = "Фамилия может состоять только из русский букв")]
        public virtual string Surname { get; set; }

        [TitleDescribe("Имя", "")]
        [RegularExpression(@"^([а-яА-Яё-]{2,24})$", ErrorMessage = "Имя может состоять только из русский букв")]
        public virtual string Name { get; set; }

        [TitleDescribe("Отчество", "")]
        [RegularExpression(@"^([а-яА-Яё-]{2,24})$", ErrorMessage = "Отчество может состоять только из русский букв")]
        public virtual string LastName { get; set; }

        [TitleDescribe("Флаг 2FA")]
        public virtual bool TwoFAkeyAprooved { get; set; }

        [TitleDescribe("Ключ 2FA", "Ключ двухфакторной аутентификации")]
        public virtual string TwoFAkey { get; set; }

        [TitleDescribe("Сдвиг времени", "")]
        public virtual decimal TimeOffsetInHours { get; set; }

        [TitleDescribe("Последняя аутентификация")]
        public virtual DateTime LastEnter { get; set; }

        [TitleDescribe("Дата регистрации")]
        public virtual DateTime RegStamp { get; set; }

        [TitleDescribe("Заблокирован?")]
        public virtual bool IsBlocked { get; set; }

        [TitleDescribe("Временная блокировка истекает")]
        public virtual DateTime DynamicBlockExpiring { get; set; }

        [TitleDescribe("Причина блокировки")]
        public virtual string BlockReason { get; set; }

        [TitleDescribe("Путь до аватара")]
        public virtual string AvatarPath { get; set; } = null;

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
            return $"{Email}" + (Name != null ? $"({Name})" : "");
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