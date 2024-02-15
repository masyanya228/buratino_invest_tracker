using Buratino.DI;
using Buratino.Entities;
using Buratino.Models.DomainService.DomainStructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Buratino.Controllers
{
    /// <summary>
    /// Контроллер авторизации
    /// </summary>
    public class AutorityController : Controller
    {
        /// <summary>
        /// Список прав пользователя
        /// </summary>
        protected string[] PermissionList { get; set; } = new string[0];

        /// <summary>
        /// УЗ пользователя
        /// </summary>
        protected Account Account { get; set; }

        /// <summary>
        /// Домен-сервис для работу с УЗ
        /// </summary>
        protected IDomainService<Account> AccountDomainService { get; set; }

        protected IDomainService<Role> Roles { get; set; }
        protected IDomainService<RoleAccountLink> RAL { get; set; }

        public AutorityController()
        {
            AccountDomainService = Container.ResolveDomainService<Account>();
            Roles = Container.ResolveDomainService<Role>();
            RAL = Container.ResolveDomainService<RoleAccountLink>();
        }

        /// <summary>
        /// Обработчик события Перед вызовом метода
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.CL = GetOperator();
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Возвращает УЗ текущего пользователя
        /// </summary>
        /// <returns></returns>
        protected Account GetOperator()
        {
            var userId = this.User?.Claims?.FirstOrDefault()?.Value ?? "0";
            var id = Guid.Parse(userId);
            if (id != Guid.Empty)
            {
                Account = AccountDomainService.Get(id);
                PermissionList = RAL.GetAll().Where(x => x.Account == Account).Select(x => x.Role.Name).ToArray();
            }
            return Account;
        }
    }
}
 