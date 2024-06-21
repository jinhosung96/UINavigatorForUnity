using System;

namespace MoraeGames.Library.Hooker
{
    public class ActionHooker
    {
        #region Fields

        Func<bool> hookingCondition = () => false;
        Action hookAction;

        #endregion

        #region Public Methods

        public void SetHook(Func<bool> hookingCondition, Action hookAction)
        {
            this.hookingCondition = hookingCondition;
            this.hookAction = hookAction;
        }

        public void Hooking(Action defaultFunc)
        {
            if (hookingCondition()) hookAction();
            else defaultFunc();
        }

        #endregion
    }

    public class ActionHooker<T>
    {
        #region Fields

        Func<bool> hookingCondition = () => false;
        Action<T> hookAction;

        #endregion

        #region Public Methods

        public void SetHook(Func<bool> hookingCondition, Action<T> hookAction)
        {
            this.hookingCondition = hookingCondition;
            this.hookAction = hookAction;
        }

        public void Hooking(Action<T> defaultFunc, T param = default)
        {
            if (hookingCondition()) hookAction(param);
            else defaultFunc(param);
        }

        #endregion
    }

    public class FuncHooker<TResult>
    {
        #region Fields

        Func<bool> hookingCondition = () => false;
        Func<TResult> hookFunc;

        #endregion

        #region Public Methods

        public void SetHook(Func<bool> hookingCondition, Func<TResult> hookFunc)
        {
            this.hookingCondition = hookingCondition;
            this.hookFunc = hookFunc;
        }

        public TResult Hooking(Func<TResult> defaultFunc)
        {
            if (hookingCondition()) return hookFunc();
            return defaultFunc();
        }

        #endregion
    }

    public class FuncHooker<T, TResult>
    {
        #region Fields

        Func<bool> hookingCondition = () => false;
        Func<T, TResult> hookFunc;

        #endregion

        #region Public Methods

        public void SetHook(Func<bool> hookingCondition, Func<T, TResult> hookFunc)
        {
            this.hookingCondition = hookingCondition;
            this.hookFunc = hookFunc;
        }

        public TResult Hooking(Func<T, TResult> defaultFunc, T param = default)
        {
            if (hookingCondition()) return hookFunc(param);
            return defaultFunc(param);
        }

        #endregion
    }
}
