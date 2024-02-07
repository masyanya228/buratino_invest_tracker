using Buratino.DI;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Entities;

using LiteDB;

using Microsoft.AspNetCore.Mvc;

namespace Buratino.Models.Xtensions
{
    public static class CockieXtensions
    {
        private static object LockObject = string.Empty;
        private static IDomainService<Account> account;
        public static IDomainService<Account> Account
        {
            get
            {
                if (account is null)
                {
                    lock (LockObject)
                    {
                        if (account is null)
                        {
                            account = Container.Resolve<IDomainService<Account>>();
                        }
                    }
                }
                return account;
            }
        }

        public static List<ClientQuene> ClientQuenes = new List<ClientQuene>();

        public static ClientQuene GetClient(HttpRequest request)
        {
            if (request.Cookies.TryGetValue("id", out string sessionId))
            {
                return GetClientBySession(sessionId);
            }
            else
            {
                return null;
            }
        }
        public static Account GetOperator(long accId)
        {
            if (accId <= 0)
                return null;
            var acc = Container.ResolveDomainService<Account>().Get(accId);
            return acc;
        }
        public static Account GetOperator(HttpContext httpContext)
        {
            var userId = long.Parse(httpContext.User?.Identity?.Name ?? "0");
            return GetOperator(userId);
        }
        public static ClientQuene GetClientBySession(string ses)
        {
            var client = ClientQuenes.FirstOrDefault(x => x.Session == ses);
            if (client != null && client.Accaunt == null)
            {
                return null;
            }
            return client;
        }
        public static ClientQuene GetClient(Controller carsController)
        {
            if (carsController.Request.Cookies.TryGetValue("id", out string sessionId))
            {
                return GetClientBySession(sessionId);
            }
            else
            {
                return null;
            }
        }
        public static ClientQuene GetClient(long id)
        {
            return ClientQuenes.FirstOrDefault(x => x.Accaunt != null && x.Accaunt.Id == id);
        }


        /// <summary>
        /// Создает сессию и привязывает к ней аккаунт
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="curAcc"></param>
        /// <returns></returns>
        public static ClientQuene SetCookies(this Controller controller, Account curAcc)
        {
            ClientQuene clientQuene = GetClient(curAcc.Id);
            if (clientQuene == null)
            {
                clientQuene = new ClientQuene()
                {
                    Accaunt = curAcc,
                    Session = DataXtensions.GetRandom(16)
                };
                ClientQuenes.Add(clientQuene);
            }
            else
            {
                clientQuene.Accaunt = curAcc;
                clientQuene.Session = DataXtensions.GetRandom(16);
            }

            controller.Response.Cookies.Append(
                "id",
                clientQuene.Session,
                new CookieOptions()
                {
                    Path = "/",
                    Expires = DateTimeOffset.Now.AddHours(10)
                }
            );
            return clientQuene;
        }
    }
}