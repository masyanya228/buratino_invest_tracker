namespace Buratino.Entities;

/// <summary>
/// Автоматический расчет баланса для банковских вкладов.
/// </summary>
public class DepositAutoPoint : InvestPoint
{
    public DepositAutoPoint()
    {
    }

    public DepositAutoPoint(decimal endBalance, decimal midBalance, decimal bVPS, bool isCapitalisation, decimal increment)
    {
        EndBalance = endBalance;
        MidBalance = midBalance;
        BVPS = bVPS;
        IsCapitalisation = isCapitalisation;

        Increment = increment;
        this.Amount = isCapitalisation
            ? EndBalance + Increment
            : EndBalance;
    }

    /// <summary>
    /// Последний баланс до расчета дохода
    /// </summary>
    public virtual decimal EndBalance { get; }

    /// <summary>
    /// Средний баланс на расчетный период
    /// </summary>
    public virtual decimal MidBalance { get; set; }

    /// <summary>
    /// Примененная процентная ставка
    /// </summary>
    public virtual decimal BVPS { get; set; }

    /// <summary>
    /// Была ли применена капитализация
    /// </summary>
    public virtual bool IsCapitalisation { get; set; }

    /// <summary>
    /// Доход
    /// </summary>
    public virtual decimal Increment { get; set; }
}
