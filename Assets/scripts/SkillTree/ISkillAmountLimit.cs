using System;
using UniRx;

    public interface ISkillAmountLimit
    {
        void UpdateSpent(int spent);
        bool CanSpend();
        bool CanTakeBack();
        int GetAvailable();
        IObservable<Unit> ObserveAmountChanged();
    }
