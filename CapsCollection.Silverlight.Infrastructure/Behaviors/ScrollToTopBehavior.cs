using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CapsCollection.Silverlight.Infrastructure.Behaviors
{
    public class ScrollToTopBehavior : Behavior<ListBox>
    {

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSourceCollection",
            typeof(IEnumerable), typeof(ScrollToTopBehavior),
            new PropertyMetadata(OnItemSourceChange));

        public IEnumerable ItemSourceCollection
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        static void OnItemSourceChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var items = ((ScrollToTopBehavior)dependencyObject).AssociatedObject.Items;
            if (items.Count > 0)
            {
                // Scroll to first item.
                ((ScrollToTopBehavior)dependencyObject).AssociatedObject.ScrollIntoView(items[0]);

            }
        }
    }
}
