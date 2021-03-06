﻿namespace Gu.Settings.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    [Serializable]
    public class ManualSaveSetting : INotifyPropertyChanged
    {
        public static readonly IReadOnlyList<StringComparison> AllComparisons =
            Enum.GetValues(typeof(StringComparison)).Cast<StringComparison>().ToArray();

        private int _value1 = 1;
        private int _value2 = 2;
        private StringComparison _comparison;

        private ManualSaveSetting()
        {
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get { return _value1; }
            set
            {
                if (value == _value1) return;
                _value1 = value;
                OnPropertyChanged();
            }
        }

        public int Value2
        {
            get { return _value2; }
            set
            {
                if (value == _value2) return;
                _value2 = value;
                OnPropertyChanged();
            }
        }

        public StringComparison Comparison
        {
            get { return _comparison; }
            set
            {
                if (value == _comparison) return;
                _comparison = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}