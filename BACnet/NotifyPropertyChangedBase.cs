using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BACnetAPA
{ 
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            InvokePropertyChanged(propertyName);
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Value must be a lamda expression ");
            }

            if (!(expression.Body is MemberExpression body))
            {
                throw new ArgumentException("x should be a member expression");
            }

            var propertyName = body.Member.Name;

            InvokePropertyChanged(propertyName);
        }

        /// <summary>
        /// Fires property changed event with default property or method name. Do not pass name to
        /// this method.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="propertyName">Do not use - it is only here to satisfy the compiler.</param>
        /// <param name="storage">Reference to value holder.</param>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            InvokePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Fires property changed event with default property or method name. Do not pass name to
        /// this method.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="checkForEquality">If true method checks for equality</param>
        /// <param name="propertyName">Do not use - it is only here to satisfy the compiler.</param>
        /// <param name="storage">Reference to value holder.</param>
        protected virtual bool SetProperty<T>(ref T storage, T value, bool checkForEquality, [CallerMemberName] string propertyName = null)
        {
            if (checkForEquality && Equals(storage, value))
            {
                return false;
            }
            storage = value;
            InvokePropertyChanged(propertyName);
            return true;
        }

        private void InvokePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Warns the developer if this Object does not have a public property with the specified
        /// name. This method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        private void VerifyPropertyName(string propertyName)
        {
            // verify that the property name matches a real, public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Debug.Fail("Invalid property name: " + propertyName);
            }
        }
    }
}