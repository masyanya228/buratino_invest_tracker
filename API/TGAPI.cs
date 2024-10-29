using Buratino.API.Dto;
using Buratino.Attributes;
using Buratino.DI;
using Buratino.Entities;
using Buratino.Enums;
using Buratino.Helpers;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Services;
using Buratino.Services;
using Buratino.Xtensions;

using System.Reflection;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Buratino.API
{
    public class TGAPI
    {
        public TelegramBotClient client;
        private InvestSourceService SourceService;
        private InvestChargeService ChargeService;
        private InvestPointService PointService;
        private IDomainService<InvestBenifit> BenifitService;
        private InvestCalcService CalcService;
        private InvestIncomeService IncomeService;
        private Dictionary<long, Guid> ChatSourcePointer;
        private Dictionary<long, TGActionType> ChatActionPointer;
        private Dictionary<long, TInvestSourceUpdateInfoForAccept> ChatUpdateInfoPointer;

        public void Start(string token)
        {
            SourceService = Container.GetDomainService<InvestSource>() as InvestSourceService;
            ChargeService = Container.GetDomainService<InvestCharge>() as InvestChargeService;
            PointService = Container.GetDomainService<InvestPoint>() as InvestPointService;
            BenifitService = Container.GetDomainService<InvestBenifit>();
            CalcService = Container.Get<InvestCalcService>();
            IncomeService = Container.Get<InvestIncomeService>();

            ChatSourcePointer = new();
            ChatActionPointer = new();
            ChatUpdateInfoPointer = new();

            client = new TelegramBotClient(token, new HttpClient());
            var me = client.GetMeAsync().GetAwaiter().GetResult();
            client.StartReceiving(OnUpdate, HandlePollingError);
        }

        private Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            try
            {
                return OnUpdateWrapper(update);
            }
            catch (Exception e)
            {
                var chat = update.Message?.Chat?.Id ?? update.CallbackQuery?.Message?.Chat?.Id ?? 0;
                if (chat > 0)
                    botClient.SendOrUpdateMessage(chat, $"{e.Message}");
                return Task.CompletedTask;
            }
        }

        private Task OnUpdateWrapper(Update update)
        {
            if (update.Type == UpdateType.Message)
            {
                return ProcessMessage(update);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                return ProcessCallbackQuery(update);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        private Task ProcessCallbackQuery(Update update)
        {
            var com = ParseCommand(update.CallbackQuery.Data, out string[] args);
            var chat = update.CallbackQuery.Message.Chat.Id;
            var messageId = update.CallbackQuery.Message.MessageId;
            client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);

            var availablePointers = this.GetMethodsWithAttribute<TGPointerAttribute>();
            var method = availablePointers.SingleOrDefault(x => x.Value.Pointers.Contains(com));
            if (method.Key is not null)
            {
                InvokeCommand(method, chat, messageId, args);
            }
            else
            {
                throw new Exception("Не поддерживаемая команда");
            }
            return Task.CompletedTask;
        }

        private Task ProcessMessage(Update update)
        {
            var chat = update.Message.Chat.Id;
            string text = update.Message.Text;
            if (!text.StartsWith("/"))
            {
                return ProcessTextMessage(chat, text);
            }
            else
            {
                return ProcessCommandMessage(chat, text);
            }
        }

        private Task ProcessCommandMessage(long chat, string text)
        {
            var com = ParseCommand(text, out string[] args);
            var availablePointers = this.GetMethodsWithAttribute<TGPointerAttribute>();
            var method = availablePointers.SingleOrDefault(x => x.Value.Pointers.Contains(com));
            if (method.Key is not null)
            {
                InvokeCommand(method, chat, 0, args);
            }
            else
            {
                throw new Exception("Не поддерживаемая команда");
            }
            return Task.CompletedTask;
        }

        private Task ProcessTextMessage(long chat, string text)
        {
            var lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim('\r', ' ', '\t')).ToArray();
            if (!ChatActionPointer.TryGetValue(chat, out TGActionType actionType) || !ChatSourcePointer.TryGetValue(chat, out Guid id))
            {
                throw new ArgumentNullException(nameof(actionType));
            }

            if (actionType == TGActionType.AddCharge)
            {
                var source = SourceService.Get(id) ?? throw new ArgumentOutOfRangeException(nameof(id));
                ChargeService.Save(new InvestCharge()
                {
                    Source = source,
                    Increment = decimal.Parse(lines[0]),
                    TimeStamp = lines.Length > 1
                        ? DateTime.Parse(lines[1])
                        : DateTime.Now,
                });
                Com_Source(chat, id);
            }
            else if (actionType == TGActionType.AddPoint)
            {
                var source = SourceService.Get(id) ?? throw new ArgumentOutOfRangeException(nameof(id));
                PointService.Save(new InvestPoint()
                {
                    Source = source,
                    Amount = decimal.Parse(lines[0]),
                    TimeStamp = lines.Length > 1
                        ? DateTime.Parse(lines[1])
                        : DateTime.Now,
                });
                Com_Source(chat, id);
            }
            else if (actionType == TGActionType.AddBenifit)
            {
                var source = SourceService.Get(id) ?? throw new ArgumentOutOfRangeException(nameof(id));
                BenifitService.Save(new InvestBenifit()
                {
                    Source = source,
                    Value = decimal.Parse(lines[0]),
                    TimeStamp = lines.Length > 1
                        ? DateTime.Parse(lines[1])
                        : DateTime.Now,
                });
                Com_Source(chat, id);
            }
            else if (actionType == TGActionType.AddSource)
            {
                var newSource = new InvestSource()
                {
                    Name = lines[0],
                    Description = lines[1],
                    TimeStamp = lines.Length > 2
                        ? DateTime.Parse(lines[2])
                        : DateTime.Now,
                };
                if (SourceService.IsExists(newSource.Name))
                {
                    throw new ArgumentException("Источник с таким наименованием уже есть");
                }
                SourceService.Save(newSource);
                Com_Source(chat, newSource.Id);
            }
            else if (actionType == TGActionType.EnterVkladInfo)
            {
                var source = SourceService.Get(id) ?? throw new ArgumentOutOfRangeException(nameof(id));

                source.BVEndStamp = DateTime.Parse(lines[0]);
                source.BVPS = decimal.Parse(lines[1]);
                source.BVCapitalisation = lines[2].Trim().ToLower() == "да";

                SourceService.Save(source);
                Com_Source(chat, source.Id);
            }
            else if (actionType == TGActionType.EnterTInvestId)
            {
                var source = SourceService.Get(id) ?? throw new ArgumentOutOfRangeException(nameof(id));

                source.TInvestAccountId = long.Parse(lines[0]);

                SourceService.Save(source);
                Com_Source(chat, source.Id);
            }
            return Task.CompletedTask;
        }

        [TGPointer("get_capital_categories")]
        private void Com_Get_Capital_Categories(long chat, int messageId)
        {
            var categories = CalcService.GetCapitalCategories()
                                    .GroupBy(x => x.CategoryOfCapitalEnum)
                                    .Select(x => new KeyValuePair<CategoryEnum, decimal>(x.Key, x.Sum(x => x.Value)))
                                    .ToArray();
            client.SendOrUpdateMessage(chat, $"Распределение капитала по категориям ивестиционных инструментов" +
                $"\r\n{string.Join("\r\n", categories.OrderByDescending(x => x.Value).Select(x => $"{x.Key} {x.Value:C}"))}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Назад", $"/menu")
                    .GetMarkup());
        }

        [TGPointer("fact_income_all")]
        private void Com_Fact_Income_All(long chat, int messageId)
        {
            IEnumerable<InvestSource> activeSources = SourceService.GetAll().Where(x => !x.IsClosed);
            var lastPoint = activeSources.Sum(x => x.LastBalance);
            var totalCharged = activeSources
                .SelectMany(x => ChargeService.GetAll().Where(y => y.Source.Id == x.Id))
                .Sum(x => x.Increment);

            var incomesTotal = IncomeService.GetAllIncomeByAllTime();
            client.SendOrUpdateMessage(chat, $"Фактические доходы по всем источникам" +
                $"\r\nДоходы за всё время: {incomesTotal.SelectMany(x => x.Value).Sum():C}" +
                $"\r\nПроверка (вход выход): {lastPoint - totalCharged:C}" +
                $"\r\n\r\n{string.Join("\r\n", incomesTotal.ToSumList().Select((x, i) => $"{i + 1}) {x:C}"))}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Назад", $"/menu")
                    .GetMarkup());
        }

        [TGPointer("fact_income")]
        private void Com_Fact_Income(long chat, int messageId, Guid id)
        {
            var result = SourceService.Get(id);
            var totalCharged = ChargeService.GetAll().Where(x => x.Source.Id == id).Sum(x => x.Increment);
            var lastPoint = PointService.GetAll()
                .Where(x => x.Source.Id == id)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault()?.Amount ?? 0;

            var incomesTotal = IncomeService.GetIncomeByAllTime(result);
            var incomes = IncomeService.GetIncomeByLastMonths(result, 6);
            client.SendOrUpdateMessage(chat, $"{result.Name}" +
                $"\r\nДоходы за всё время: {incomesTotal.Sum():C}" +
                $"\r\nПроверка: {lastPoint - totalCharged:C}" +
                $"\r\n\r\n{string.Join("\r\n", incomes.Select((x, i) => $"{i + 1}) {x:C}"))}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Назад", $"/source/{id}")
                    .GetMarkup());
        }

        [TGPointer("set_category")]
        private void Com_Set_Category(long chat, int messageId, Guid id, CategoryEnum category)
        {
            var result = SourceService.Get(id);
            result.Category = category;
            SourceService.Save(result);

            if (result.Category == CategoryEnum.Deposit)
            {
                Com_To_EnterVkladInfo(chat, id, messageId);
            }
            else if (result.Category == CategoryEnum.TInvestAuto)
            {
                Com_To_EnterTInvestInfo(chat, id, messageId);
            }
            else
            {
                Com_Source(chat, id, messageId);
            }
        }

        [TGPointer("delete_benifit")]
        private void Com_Delete_Benifit(long chat, int messageId, Guid id)
        {
            var result = BenifitService.Get(id);
            BenifitService.Delete(id);
            Com_History(chat, result.Source.Id, messageId);
        }

        [TGPointer("try_delete_benifit")]
        private void Com_Try_Delete_Benifit(long chat, int messageId, Guid id)
        {
            var result = BenifitService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Удаление пополнения {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Value}" +
                $"\r\n\r\nДействительно удалить?!",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("🚫Удалить🚫", $"/delete_benifit/{id}")
                    .AddButtonDown("Назад", $"/history/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("view_benifit")]
        private void Com_View_Benifit(long chat, int messageId, Guid id)
        {
            var result = BenifitService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Пополнение {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Value:C}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Удалить", $"/try_delete_benifit/{id}")
                    .AddButtonDown("Назад", $"/source/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("delete_point")]
        private void Com_Delete_Point(long chat, int messageId, Guid id)
        {
            var result = PointService.Get(id);
            PointService.Delete(id);
            Com_History(chat, result.Source.Id, messageId);
        }

        [TGPointer("try_delete_point")]
        private void Com_Try_Delete_Point(long chat, int messageId, Guid id)
        {
            var result = PointService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Удаление отметки {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Amount}" +
                $"\r\n\r\nДействительно удалить?!",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("🚫Удалить🚫", $"/delete_point/{id}")
                    .AddButtonDown("Назад", $"/history/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("view_point")]
        private void Com_View_Point(long chat, int messageId, Guid id)
        {
            var result = PointService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Отметка {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Amount:C}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Удалить", $"/try_delete_point/{id}")
                    .AddButtonDown("Назад", $"/source/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("try_delete_charge")]
        private void Com_Try_Delete_Charge(long chat, int messageId, Guid id)
        {
            var result = ChargeService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Удаление пополнения {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Increment}" +
                $"\r\n\r\nДействительно удалить?!",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("🚫Удалить🚫", $"/delete_charge/{id}")
                    .AddButtonDown("Назад", $"/history/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("view_charge")]
        private void Com_View_Charge(long chat, int messageId, Guid id)
        {
            var result = ChargeService.Get(id);

            client.SendOrUpdateMessage(chat,
                $"Пополнение {result.TimeStamp.ToShortDateString()}" +
                $"\r\n{result.Increment:C}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Удалить", $"/try_delete_charge/{id}")
                    .AddButtonDown("Назад", $"/source/{result.Source.Id}")
                    .GetMarkup());
        }

        [TGPointer("accept_changes")]
        private void Com_Accept_Changes(long chat, Guid id)
        {
            if (!ChatUpdateInfoPointer.TryGetValue(chat, out TInvestSourceUpdateInfoForAccept infoForAccept) && infoForAccept.SourceId == id)
            {
                throw new ArgumentException("Значение в кэше не соответствует намерению. Сделайте операцию заново.");
            }
            var source = SourceService.Get(id);
            infoForAccept.HistoryOpsDiff.Added.Select(x =>
            {
                return ChargeService.Save(new InvestCharge()
                {
                    Source = source,
                    Increment = x.Increment,
                    TimeStamp = x.TimeStamp,
                });
            })
                .ToArray();


            var charges = ChargeService.GetAll().Where(x => x.Source.Id == id).ToArray();
            infoForAccept.HistoryOpsDiff.Removed.SelectMany(x =>
            {
                return charges.Where(y => y.TimeStamp.Date == x.TimeStamp.Date)
                    .Select(y =>
                    {
                        ChargeService.Delete(y.Id);
                        return y.Id;
                    });
            })
                .ToArray();

            infoForAccept.HistoryOpsDiff.Edited.Select(x =>
            {
                var exist = charges.SingleOrDefault(y => y.TimeStamp.Date == x.TimeStamp.Date);
                exist.Increment = x.Increment;
                return ChargeService.Save(exist);
            })
                .ToArray();

            PointService.Save(new InvestPoint()
            {
                Source = source,
                Amount = infoForAccept.NewValue,
            });
        }

        [TGPointer("update_info")]
        private void Com_Update_Info(long chat, Guid id)
        {
            client.SendTextMessageAsync(chat, "Эта операция может занять пару минут.");
            client.SendChatActionAsync(chat, ChatAction.Typing);
            var source = SourceService.Get(id);

            var newValue = new TInvestService().GetAccountValue(source.TInvestAccountId);
            var chargeDiff = new TInvestService().GetHistoryDiff(source);

            ChatUpdateInfoPointer[chat] = new TInvestSourceUpdateInfoForAccept()
            {
                SourceId = id,
                HistoryOpsDiff = chargeDiff,
                NewValue = newValue
            };

            var addedText = chargeDiff.Added.Any()
                ? "\r\n\r\nНовые операции:\r\n" + chargeDiff.Added.Select(x => $"{x.TimeStamp.ToShortDateString()}: {x.Increment:C}").Join("\r\n")
                : string.Empty;

            var editedText = chargeDiff.Edited.Any()
                ? "\r\n\r\nИзмененные операции:\r\n" + chargeDiff.Edited.Select(x => $"{x.TimeStamp.ToShortDateString()}: {x.Increment:C}").Join("\r\n")
                : string.Empty;

            var removedText = chargeDiff.Removed.Any()
                ? "\r\n\r\nУдаленные операции:\r\n" + chargeDiff.Removed.Select(x => $"{x.TimeStamp.ToShortDateString()}: {x.Increment:C}").Join("\r\n")
                : string.Empty;

            client.SendOrUpdateMessage(chat,
                $"{source.Name} оценивается в {newValue:C}" +
                $"{addedText}{editedText}{removedText}",
                0,
                new InlineKeyboardConstructor()
                .AddButtonDown("Применить изменения", $"/accept_changes/{id}")
                .GetMarkup());
            Com_Source(chat, id);
        }

        [TGPointer("delete_charge")]
        private void Com_Delete_Charge(long chat, int messageId, Guid id)
        {
            var result = ChargeService.Get(id);
            ChargeService.Delete(id);
            Com_History(chat, result.Source.Id, messageId);
        }

        private void Com_To_EnterVkladInfo(long chat, Guid id, int messageId)
        {
            ChatActionPointer[chat] = TGActionType.EnterVkladInfo;
            ChatSourcePointer[chat] = id;

            client.SendOrUpdateMessage(chat,
                $"Укажите параметры вклада" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\nДата закрытия вклада" +
                $"\r\nПроцентная ставка" +
                $"\r\nКапитализация(да/нет)",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonRight("Назад", "/sources")
                    .GetMarkup());
        }

        private void Com_To_EnterTInvestInfo(long chat, Guid id, int messageId)
        {
            ChatActionPointer[chat] = TGActionType.EnterTInvestId;
            ChatSourcePointer[chat] = id;

            client.SendOrUpdateMessage(chat,
                $"Укажите accountId у счета в Т-инвестициях" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\n102301293",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonRight("Назад", "/sources")
                    .GetMarkup());
        }

        [TGPointer("history")]
        private void Com_History(long chat, Guid id, int messageId)
        {
            var result = SourceService.Get(id);
            var charges = ChargeService.GetAll()
                .Where(x => x.Source.Id == result.Id)
                .ToArray()
                .Select(x => new KeyValuePair<DateTime, HistoryDto>(x.TimeStamp, new HistoryDto()
                {
                    Title = $"{x.Increment:C} ↗️",
                    Id = x.Id,
                    ItemType = HistoryItemType.Charge,
                }));

            var points = PointService.GetAll()
                .Where(x => x.Source.Id == result.Id)
                .ToArray()
                .Select(x => new KeyValuePair<DateTime, HistoryDto>(x.TimeStamp, new HistoryDto()
                {
                    Title = $"{x.Amount:C} ➡️",
                    Id = x.Id,
                    ItemType = HistoryItemType.Point,
                }));

            var benifits = BenifitService.GetAll()
                .Where(x => x.Source.Id == result.Id)
                .ToArray()
                .Select(x => new KeyValuePair<DateTime, HistoryDto>(x.TimeStamp, new HistoryDto()
                {
                    Title = $"{x.Value:C} 🌟",
                    Id = x.Id,
                    ItemType = HistoryItemType.Benifit,
                }));

            client.SendOrUpdateMessage(chat,
                $"{result.Name} - {result.Description}" +
                $"\r\nИстория:",
                messageId,
                new InlineKeyboardConstructor(charges.Concat(points).Concat(benifits)
                        .OrderByDescending(x => x.Key)
                        .Select(x =>
                            new InlineKeyboardButton($"{x.Key.ToShortDateString()} {x.Value.Title}")
                            {
                                CallbackData = $"/view_{x.Value.ItemType}/{x.Value.Id}"
                            }))
                .AddButtonDown("Назад", $"/source/{id}")
                .GetMarkup());
        }

        [TGPointer("to_source")]
        private void Com_To_Source(long chat, int messageId)
        {
            ChatActionPointer[chat] = TGActionType.AddSource;
            ChatSourcePointer[chat] = Guid.Empty;

            client.SendOrUpdateMessage(chat,
                $"Добавление источника" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\nОбозначение источника" +
                $"\r\nОписание источника" +
                $"\r\nДата открытия",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Отмена", "/sources")
                    .GetMarkup());
        }

        [TGPointer("to_point")]
        private void Com_To_Point(long chat, Guid id, int messageId = 0)
        {
            ChatSourcePointer[chat] = id;
            ChatActionPointer[chat] = TGActionType.AddPoint;

            var result = SourceService.Get(id);
            client.SendOrUpdateMessage(chat,
                $"{result.Name} - Отметка" +
                $"\r\nПоследний баланс: {result.LastBalance:C}" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\n1000 (текущий баланс)" +
                $"\r\n{DateTime.Now.AddDays(-3).ToShortDateString()} (если отметка сегодняшняя, можно пропустить)",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Отмена", $"/source/{id}")
                    .GetMarkup());
        }

        [TGPointer("to_charge")]
        private void Com_To_Charge(long chat, Guid id, int messageId = 0)
        {
            ChatSourcePointer[chat] = id;
            ChatActionPointer[chat] = TGActionType.AddCharge;

            var result = SourceService.Get(id);
            var lastCharge = ChargeService
                .GetAll()
                .Where(x => x.Source.Id == result.Id)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault();

            var lastChargeText = lastCharge is null
                ? "Пополнений не было"
                : $"Последние пополнение {lastCharge.TimeStamp.ToShortDateString()} на {lastCharge.Increment:C}";

            client.SendOrUpdateMessage(chat,
                $"{result.Name} - Пополнение" +
                $"\r\n{lastChargeText}" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\n1000 (сумма пополнения)" +
                $"\r\n{DateTime.Now.AddDays(-3).ToShortDateString()} (если пополнение сегодняшнее, можно пропустить)",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Отмена", $"/source/{id}")
                    .GetMarkup());
        }

        [TGPointer("to_benifit")]
        private void Com_To_Benifit(long chat, Guid id, int messageId = 0)
        {
            ChatSourcePointer[chat] = id;
            ChatActionPointer[chat] = TGActionType.AddBenifit;

            var result = SourceService.Get(id);
            var lastBenifit = BenifitService
                .GetAll()
                .Where(x => x.Source.Id == result.Id)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefault();

            var lastBenifitText = lastBenifit is null
                ? "Бенифитов не было"
                : $"Последний бенифит {lastBenifit.TimeStamp.ToShortDateString()} на {lastBenifit.Value:C}";

            client.SendOrUpdateMessage(chat,
                $"{result.Name} - Бенифит" +
                $"\r\n{lastBenifitText}" +
                $"\r\nОтправьте сообщение в следующем формате:\r\n" +
                $"\r\n1000 (сумма бенифита)" +
                $"\r\n{DateTime.Now.AddDays(-3).ToShortDateString()} (если бинифит сегодняшний, можно пропустить)",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Отмена", $"/source/{id}")
                    .GetMarkup());
        }

        [TGPointer("start", "menu")]
        private void Com_Menu(long chat, int messageId = 0)
        {
            var result = SourceService.GetStatsAll();
            client.SendOrUpdateMessage(chat,
                $"Доходность {result.Percent}%" +
                $"\r\nКапитал {result.Balance:C}" +
                $"\r\nДоходность в месяц {result.IncomePerMonth:C}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Источники", $"/sources")
                    .AddButtonDown("Планы", $"/plans")
                    .AddButtonDown("Факт", $"/fact_income_all")
                    .AddButtonRight("По категориям", $"/get_capital_categories")
                    .GetMarkup());
        }

        [TGPointer("plans")]
        private void Com_Plans(long chat, int messageId = 0, int target = 750000)
        {
            var result = ChargeService.ChargesByYear(target);
            client.SendOrUpdateMessage(chat, $"План пополнений: {target:C}" +
                $"\r\nПополнено: {result.TotalFact:C}({result.AmountOfCharges} раз)" +
                $"\r\nДолжно быть: {result.TodayPlan:C}" +
                $"\r\nВыполнение цели: {Math.Round(result.ChargeProgress)}" +
                $"\r\nЗавершение года: {Math.Round(result.YearProgress)}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonDown("Назад", "/menu")
                    .GetMarkup());
        }

        [TGPointer("source")]
        private void Com_Source(long chat, Guid id, int messageId = 0)
        {
            var result = SourceService.Get(id);
            if (result.Category == CategoryEnum.None)
            {
                client.SendOrUpdateMessage(chat, $"{result.Name} - {result.Description}" +
                $"\r\nВыберите категорию источника:",
                messageId,
                new InlineKeyboardConstructor(
                    Enum.GetNames(typeof(CategoryEnum))
                    .Select(x => new InlineKeyboardButton(x) { CallbackData = $"/set_category/{id}/{x}" }))
                    .AddButtonDown("Назад", "/sources")
                    .GetMarkup());
                return;
            }

            client.SendOrUpdateMessage(chat, $"{result.Name} - {result.Description}" +
                $"\r\nВложено: {result.TotalCharged:C}" +
                $"\r\nБаланс: {result.LastBalance:C}" +
                $"\r\nДоходность: {Math.Round(result.EffectiveBase, 1)}",
                messageId,
                new InlineKeyboardConstructor()
                    .AddButtonIf(() => result.TInvestAccountId != 0, "Актуализировать", $"/update_info/{result.Id}")
                    .AddButtonDown("Пополнение", $"/to_charge/{result.Id}")
                    .AddButtonRight("Отметка", $"/to_point/{result.Id}")
                    .AddButtonRight("Бенефит", $"/to_benefit/{result.Id}")
                    .AddButtonDown("История", $"/history/{result.Id}")
                    .AddButtonRight("Факт", $"/fact_income/{result.Id}")
                    .AddButtonDown("Назад", "/sources")
                    .GetMarkup());
        }

        [TGPointer("sources")]
        private void Com_Sources(long chat, int messageId = 0)
        {
            var points = PointService.GetAll().ToArray();
            var result = SourceService.GetAll().Where(x => !x.IsClosed).ToArray();
            client.SendOrUpdateMessage(chat, "Ваши источники дохода:", messageId,
                new InlineKeyboardConstructor(result
                    .OrderBy(x => x.LastBalance)
                    .ToArray()
                    .Select(x =>
                    {
                        var title = x.PrintList();
                        var lastPointing = points.Where(y => y.Source.Id == x.Id).OrderByDescending(y => y.TimeStamp).FirstOrDefault()?.TimeStamp ?? DateTime.MinValue;
                        var needPoint = x.Category != CategoryEnum.DepositAuto && DateTime.Now.Subtract(lastPointing).TotalDays >= 15;
                        if (needPoint)
                        {
                            title = "🌚 " + title;
                        }

                        return new InlineKeyboardButton(title)
                        {
                            CallbackData = $"/source/{x.Id}"
                        };
                    }))
                .AddButtonDown("+Добавить", "/to_source")
                .AddButtonDown("Назад", "/menu")
                .GetMarkup());
        }

        private void InvokeCommand(KeyValuePair<MethodInfo, TGPointerAttribute> method, long chat, int messageId, string[] args)
        {
            var parameters = method.Key.GetParameters();
            var arguments = new object[parameters.Length];
            var comArgs = new Queue<string>(args);
            for (int i = 0; i < parameters.Length; i++)
            {
                var item = parameters[i];
                if (item.Name == "chat")
                    arguments[i] = chat;
                else if (item.Name == "messageId")
                    arguments[i] = messageId;
                else if (comArgs.Any())
                {
                    arguments[i] = comArgs.Dequeue().Cast(item.ParameterType);
                }
                else if (item.IsOptional)
                {
                    arguments[i] = item.DefaultValue;
                }
                else
                {
                    throw new ArgumentException("Не хватает аргументов для вызова метода");
                }
            }
            method.Key.Invoke(this, arguments);
        }

        private string ParseCommand(string query, out string[] args)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException($"\"{nameof(query)}\" не может быть пустым или содержать только пробел.", nameof(query));
            }
            query = query.Substring(1);
            args = query.Split('/').Skip(1).ToArray();
            return query.Split('/').First().ToLower();
        }

        private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
