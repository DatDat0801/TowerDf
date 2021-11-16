using System;

namespace Zitga.Observables
{
    public interface IObservableProperty
    {
        Type Type { get; }

        object Value { get; set; }
        event EventHandler ValueChanged;
    }

    public interface IObservableProperty<T> : IObservableProperty
    {
        new T Value { get; set; }
    }
}