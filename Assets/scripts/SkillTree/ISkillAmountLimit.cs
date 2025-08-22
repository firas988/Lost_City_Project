using System;
using UniRx;

public interface ISkillAmountLimit
{
    void UpdateSpent(int spent);
    bool CanSpend();
    bool CanSpend(int amount);
    int GetAvailable();
    IObservable<Unit> ObserveAmountChanged();
}
