using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmMicro
{
    /// <summary>
    /// Provides the base class for objects that support property change notification.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// A convenience method that sets the specified backing field to the given value, and raises the
        /// <see cref="PropertyChanged"/> event if the value changed.
        /// </summary>
        /// <typeparam name="T">The type of the property and its backing field.</typeparam>
        /// <param name="field">The property backing field to set.</param>
        /// <param name="newValue">The new value to set.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns><c>true</c> if property value has changed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
