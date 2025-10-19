namespace Buratino.Enums
{
    public enum OperationType
    {
        //Тип операции не определен.
        OPERATION_TYPE_UNSPECIFIED = 0,
        //Пополнение брокерского счета.
        OPERATION_TYPE_INPUT = 1,
        //Удержание НДФЛ по купонам.
        OPERATION_TYPE_BOND_TAX = 2,
        //Вывод ЦБ.
        OPERATION_TYPE_OUTPUT_SECURITIES = 3,
        //Доход по сделке РЕПО овернайт.
        OPERATION_TYPE_OVERNIGHT = 4,
        //Удержание налога.
        OPERATION_TYPE_TAX = 5,
        //Полное погашение облигаций.
        OPERATION_TYPE_BOND_REPAYMENT_FULL = 6,
        //Продажа ЦБ с карты.
        OPERATION_TYPE_SELL_CARD = 7,
        //Удержание налога по дивидендам.
        OPERATION_TYPE_DIVIDEND_TAX = 8,
        //Вывод денежных средств.
        OPERATION_TYPE_OUTPUT = 9,
        //Частичное погашение облигаций.
        OPERATION_TYPE_BOND_REPAYMENT = 10,
        //Корректировка налога.
        OPERATION_TYPE_TAX_CORRECTION = 11,
        //Удержание комиссии за обслуживание брокерского счета.
        OPERATION_TYPE_SERVICE_FEE = 12,
        //Удержание налога за материальную выгоду.
        OPERATION_TYPE_BENEFIT_TAX = 13,
        //Удержание комиссии за непокрытую позицию.
        OPERATION_TYPE_MARGIN_FEE = 14,
        //Покупка ЦБ.
        OPERATION_TYPE_BUY = 15,
        //Покупка ЦБ с карты.
        OPERATION_TYPE_BUY_CARD = 16,
        //Перевод ценных бумаг из другого депозитария.
        OPERATION_TYPE_INPUT_SECURITIES = 17,
        //Продажа в результате Margin-call.
        OPERATION_TYPE_SELL_MARGIN = 18,
        //Удержание комиссии за операцию.
        OPERATION_TYPE_BROKER_FEE = 19,
        //Покупка в результате Margin-call.
        OPERATION_TYPE_BUY_MARGIN = 20,
        //Выплата дивидендов.
        OPERATION_TYPE_DIVIDEND = 21,
        //Продажа ЦБ.
        OPERATION_TYPE_SELL = 22,
        //Выплата купонов.
        OPERATION_TYPE_COUPON = 23,
        //Удержание комиссии SuccessFee.
        OPERATION_TYPE_SUCCESS_FEE = 24,
        //Передача дивидендного дохода.
        OPERATION_TYPE_DIVIDEND_TRANSFER = 25,
        //Зачисление вариационной маржи.
        OPERATION_TYPE_ACCRUING_VARMARGIN = 26,
        //Списание вариационной маржи.
        OPERATION_TYPE_WRITING_OFF_VARMARGIN = 27,
        //Покупка в рамках экспирации фьючерсного контракта.
        OPERATION_TYPE_DELIVERY_BUY = 28,
        //Продажа в рамках экспирации фьючерсного контракта.
        OPERATION_TYPE_DELIVERY_SELL = 29,
        //Комиссия за управление по счету автоследования.
        OPERATION_TYPE_TRACK_MFEE = 30,
        //Комиссия за результат по счету автоследования.
        OPERATION_TYPE_TRACK_PFEE = 31,
        //Удержание налога по ставке 15%.
        OPERATION_TYPE_TAX_PROGRESSIVE = 32,
        //Удержание налога по купонам по ставке 15%.
        OPERATION_TYPE_BOND_TAX_PROGRESSIVE = 33,
        //Удержание налога по дивидендам по ставке 15%.
        OPERATION_TYPE_DIVIDEND_TAX_PROGRESSIVE = 34,
        //Удержание налога за материальную выгоду по ставке 15%.
        OPERATION_TYPE_BENEFIT_TAX_PROGRESSIVE = 35,
        //Корректировка налога по ставке 15%.
        OPERATION_TYPE_TAX_CORRECTION_PROGRESSIVE = 36,
        //Удержание налога за возмещение по сделкам РЕПО по ставке 15%.
        OPERATION_TYPE_TAX_REPO_PROGRESSIVE = 37,
        //Удержание налога за возмещение по сделкам РЕПО.
        OPERATION_TYPE_TAX_REPO = 38,
        //Удержание налога по сделкам РЕПО.
        OPERATION_TYPE_TAX_REPO_HOLD = 39,
        //Возврат налога по сделкам РЕПО.
        OPERATION_TYPE_TAX_REPO_REFUND = 40,
        //Удержание налога по сделкам РЕПО по ставке 15%.
        OPERATION_TYPE_TAX_REPO_HOLD_PROGRESSIVE = 41,
        //Возврат налога по сделкам РЕПО по ставке 15%.
        OPERATION_TYPE_TAX_REPO_REFUND_PROGRESSIVE = 42,
        //Выплата дивидендов на карту.
        OPERATION_TYPE_DIV_EXT = 43,
        //Корректировка налога по купонам.
        OPERATION_TYPE_TAX_CORRECTION_COUPON = 44,
        //Комиссия за валютный остаток.
        OPERATION_TYPE_CASH_FEE = 45,
        //Комиссия за вывод валюты с брокерского счета.
        OPERATION_TYPE_OUT_FEE = 46,
        //Гербовый сбор.
        OPERATION_TYPE_OUT_STAMP_DUTY = 47,
        //SWIFT-перевод.
        OPERATION_TYPE_OUTPUT_SWIFT = 50,
        //SWIFT-перевод.
        OPERATION_TYPE_INPUT_SWIFT = 51,
        //Перевод на карту.
        OPERATION_TYPE_OUTPUT_ACQUIRING = 53,
        //Перевод с карты.
        OPERATION_TYPE_INPUT_ACQUIRING = 54,
        //Комиссия за вывод средств.
        OPERATION_TYPE_OUTPUT_PENALTY = 55,
        //Списание оплаты за сервис Советов.
        OPERATION_TYPE_ADVICE_FEE = 56,
        //Перевод ценных бумаг с ИИС на брокерский счет.
        OPERATION_TYPE_TRANS_IIS_BS = 57,
        //Перевод ценных бумаг с одного брокерского счета на другой.
        OPERATION_TYPE_TRANS_BS_BS = 58,
        //Вывод денежных средств со счета.
        OPERATION_TYPE_OUT_MULTI = 59,
        //Пополнение денежных средств со счета.
        OPERATION_TYPE_INP_MULTI = 60,
        //Размещение биржевого овернайта.
        OPERATION_TYPE_OVER_PLACEMENT = 61,
        //Списание комиссии.
        OPERATION_TYPE_OVER_COM = 62,
        //Доход от оверанайта.
        OPERATION_TYPE_OVER_INCOME = 63,
        //Экспирация опциона.
        OPERATION_TYPE_OPTION_EXPIRATION = 64,
        //Экспирация фьючерса.
        OPERATION_TYPE_FUTURE_EXPIRATION = 65
    }
}