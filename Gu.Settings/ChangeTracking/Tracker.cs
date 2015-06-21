﻿namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    public abstract class Tracker : ITracker
    {
        public static readonly string ChangesPropertyName = "Changes";
        protected static readonly PropertyInfo ChangesPropertyInfo = typeof(Tracker).GetProperty(ChangesPropertyName);
        protected static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(ChangesPropertyName);
        private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> TrackPropertiesMap = new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();
        private int _changes;
        private bool _disposed;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Changed;

        public int Changes
        {
            get { return _changes; }
            set
            {
                if (value == _changes)
                {
                    return;
                }
                _changes = value;
                OnPropertyChanged(ChangesEventArgs);
                OnChanged();
            }
        }

        public static IValueTracker Track(INotifyPropertyChanged root)
        {
            Ensure.NotNull(root, "root");
            return new PropertyChangeTracker(typeof(Tracker), ChangesPropertyInfo, root);
        }

        internal static void Verify(Type parentType, PropertyInfo parentProperty, object item)
        {
            if (Attribute.IsDefined(parentType, typeof(TrackingAttribute), true))
            {
                return;
            }

            if (Attribute.IsDefined(parentProperty, typeof(TrackingAttribute), true))
            {
                return;
            }

            var propertyType = parentProperty.PropertyType;
            //if (Attribute.IsDefined(propertyType, typeof(TrackingAttribute), true))
            //{
            //    return;
            //}

            if (!IsTrackType(propertyType))
            {
                return;
            }

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(propertyType))
            {
                return;
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(propertyType))
            {
                return;
            }

            var message =
                string.Format(
                    @"Create tracker failed for {1}.{2}{0}. Solve the problem by:{0}1) Implementing INotifyPropertyChanged {0}2) Implementing INotifyCollectionChanged{0}3) Add TrackingAttribute: Immutable to type:{1}{0}4) AddTrackingAttribute: Explicit to {2} ",
                    Environment.NewLine,
                    parentType.Name,
                    parentProperty.Name);
            throw new ArgumentException(message);
        }

        internal static bool CanTrack(Type parentType, PropertyInfo parentProperty, object item)
        {
            Verify(parentType, parentProperty, item);

            var incc = item as INotifyCollectionChanged;
            if (incc != null)
            {
                return true;
            }
            var inpc = item as INotifyPropertyChanged;
            if (inpc != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected static IPropertyTracker Create(Type parentType, PropertyInfo parentProperty, object child)
        {
            if (!CanTrack(parentType, parentProperty, child))
            {
                return null;
            }

            if (child == null)
            {
                return null;
            }
            var incc = child as INotifyCollectionChanged;
            if (incc != null)
            {
                return new CollectionTracker(parentType, parentProperty, (IEnumerable)incc);
            }
            var inpc = child as INotifyPropertyChanged;
            if (inpc != null)
            {
                return new PropertyChangeTracker(parentType, parentProperty, inpc);
            }
            throw new ArgumentException();
        }

        protected static IReadOnlyList<PropertyInfo> GetTrackProperties(INotifyPropertyChanged item)
        {
            if (item == null)
            {
                return null;
            }

            var trackProperties = TrackPropertiesMap.GetOrAdd(item.GetType(), TrackPropertiesFor);
            return trackProperties;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                // Free any other managed objects here. 
            }

            // Free any unmanaged objects here. 
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnChanged()
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected static bool IsTrackType(Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }
            if (type.IsPrimitive)
            {
                return false;
            }
            return true;
        }

        private static IReadOnlyList<PropertyInfo> TrackPropertiesFor(Type type)
        {
            var propertyInfos = type.GetProperties()
                                    .Where(x => IsTrackType(x.PropertyType))
                                    .ToArray();
            return propertyInfos;
        }
    }
}