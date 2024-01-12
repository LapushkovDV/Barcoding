using Xamarin.Forms;

namespace TSD.Services.DependencyProperties.Behaviors
{
    /// <summary>
    /// Behavior поисковой строки
    /// </summary>
    public class SearchTextChangedBehavior : Behavior<SearchBar>
    {
        protected override void OnAttachedTo(SearchBar bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((SearchBar)sender).SearchCommand?.Execute(e.NewTextValue);
        }
    }
}
