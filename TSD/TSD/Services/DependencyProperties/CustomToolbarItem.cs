using Xamarin.Forms;

namespace TSD.Services.DependencyProperties
{
    /// <summary>
    /// Контрол тулбар кнопок со свойством видимости
    /// </summary>
    public class CustomToolbarItem : ToolbarItem
    {
        #region Присоединяемые свойства
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(CustomToolbarItem), true, BindingMode.TwoWay, propertyChanged: OnIsVisibleChanged);
        #endregion

        #region Свойства
        /// <summary>
        /// Видимость кнопки
        /// </summary>
        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }
        #endregion

        #region Методы-события
        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is CustomToolbarItem item) || item.Parent == null)
                return;

            var toolbarItems = ((ContentPage)item.Parent).ToolbarItems;

            if ((bool)newvalue && !toolbarItems.Contains(item))
            {
                Device.BeginInvokeOnMainThread(() => { toolbarItems.Add(item); });
            }
            else if (!(bool)newvalue && toolbarItems.Contains(item))
            {
                Device.BeginInvokeOnMainThread(() => { toolbarItems.Remove(item); });
            }
        }
        #endregion
    }
}
