namespace Buratino.API.Dto
{
    public enum CouponTypes
    {
        //Неопределенное значение.
        COUPON_TYPE_UNSPECIFIED = 0,
        //Постоянный.
        COUPON_TYPE_CONSTANT = 1,
        //Плавающий.
        COUPON_TYPE_FLOATING = 2,
        //Дисконт.
        COUPON_TYPE_DISCOUNT = 3,
        //Ипотечный.
        COUPON_TYPE_MORTGAGE = 4,
        //Фиксированный.
        COUPON_TYPE_FIX = 5,
        //Переменный.
        COUPON_TYPE_VARIABLE = 6,
        // Прочее.
        COUPON_TYPE_OTHER = 7
    }
}