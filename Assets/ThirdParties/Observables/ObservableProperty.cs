using System;

namespace Zitga.Observables
{
    [Serializable]
    public class ObservablePropertyBase<T>
    {
        private readonly object _lock = new object();
        protected T _value;
        private EventHandler valueChanged;

        public ObservablePropertyBase() : this(default)
        {
        }

        public ObservablePropertyBase(T value)
        {
            _value = value;
        }

        public virtual Type Type => typeof(T);

        public event EventHandler ValueChanged
        {
            add
            {
                lock (_lock)
                {
                    valueChanged += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    valueChanged -= value;
                }
            }
        }

        protected void RaiseValueChanged()
        {
            EventHandler handler = valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual bool Equals(T x, T y)
        {
            if (x != null)
            {
                if (y != null)
                    return x.Equals(y);
                return false;
            }

            if (y != null)
                return false;

            return true;
        }
    }

    [Serializable]
    public class ObservableProperty : ObservablePropertyBase<object>, IObservableProperty
    {
        public ObservableProperty() : this(null)
        {
        }

        public ObservableProperty(object value) : base(value)
        {
        }

        public override Type Type => _value != null ? _value.GetType() : typeof(object);

        public virtual object Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;

                _value = value;
                RaiseValueChanged();
            }
        }

        public override string ToString()
        {
            object v = Value;
            if (v == null)
                return "";

            return v.ToString();
        }
    }

    [Serializable]
    public class ObservableProperty<T> : ObservablePropertyBase<T>, IObservableProperty<T>
    {
        public ObservableProperty() : this(default)
        {
        }

        public ObservableProperty(T value) : base(value)
        {
        }

        public virtual T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;

                _value = value;
                RaiseValueChanged();
            }
        }

        object IObservableProperty.Value
        {
            get => Value;
            set => Value = (T) value;
        }

        public override string ToString()
        {
            T v = Value;
            if (v == null)
                return "";

            return v.ToString();
        }

        public static implicit operator T(ObservableProperty<T> data)
        {
            return data.Value;
        }

        public static implicit operator ObservableProperty<T>(T data)
        {
            return new ObservableProperty<T>(data);
        }
    }
}