using OwlCore.WinUI.Converters.Text;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace OwlCore.WinUI.Attached
{
    /// <summary>
    /// Enables adding multiple group of visual state setters to a VisualState.
    /// </summary>
    public partial class CompositeVisualStateSetter : DependencyObject
    {
        /// <summary>
        /// Gets the <see cref="CharacterCasing"/> value for this dependency object.
        /// </summary>
        public static IList<SetterBaseCollection> GetSetters(DependencyObject obj) => (IList<SetterBaseCollection>)obj.GetValue(SettersProperty);

        /// <summary>
        /// Sets the <see cref="IList{SetterBaseCollection}"/> value for this dependency object.
        /// </summary>
        public static void SetSetters(DependencyObject obj, IList<SetterBaseCollection> value)
        {
            if (obj is not VisualState visualState)
                return;

            obj.SetValue(SettersProperty, value);

            foreach (var collection in value)
                foreach (var item in collection)
                    visualState.Setters.Add(item);

        }

        /// <summary>
        /// Backing dependency property for <see cref="CharacterCasing"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SettersProperty = DependencyProperty.RegisterAttached("Setters", typeof(CompositeVisualStateSetter), typeof(IList<SetterBaseCollection>), new PropertyMetadata(new List<SetterBaseCollection>(), OnSettersChanged));

        private static void OnSettersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
