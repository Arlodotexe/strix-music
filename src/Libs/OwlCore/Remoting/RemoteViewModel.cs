using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OwlCore.Remoting
{
    /// <summary>
    /// A base ViewModel 
    /// </summary>
    public abstract class RemoteViewModel : ObservableObject
    {
        private Type _type;

        private IEnumerable<MethodInfo> _methods;
        private IEnumerable<EventInfo> _events;
        private IEnumerable<FieldInfo> _fields;
        private IEnumerable<PropertyInfo> _properties;

        // Do I even need this??
        // Replace with another class to hold the above info
        // but hold that instance in a service
        // we can just pass `this` into the service and go full reflection.
        protected RemoteViewModel()
        {
            _type = GetType();

            var members = _type.GetMembers();

            // https://github.com/tonerdo/pose
            // https://github.com/pardeike/Harmony

            _methods = members.Where(x => x.MemberType == MemberTypes.Method).Cast<MethodInfo>();
            _events = members.Where(x => x.MemberType == MemberTypes.Event).Cast<EventInfo>();
            _fields = members.Where(x => x.MemberType == MemberTypes.Field).Cast<FieldInfo>();
            _properties = members.Where(x => x.MemberType == MemberTypes.Property).Cast<PropertyInfo>();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
        }
    }
}
