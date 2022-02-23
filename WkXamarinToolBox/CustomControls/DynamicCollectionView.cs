using System;
using System.Linq;
using Xamarin.Forms;

namespace WkXamarinToolBox.CustomControls
{
    //ABOUT: This CollectionView can't be used as a normal CollectionView! Only use it when it can freely expand vertically. It's a perfect tool for Photo Albums.
    public class DynamicCollectionView : CollectionView
    {
        private int _counteredItems;

        public DynamicCollectionView()
        {
            ChildAdded += new EventHandler<ElementEventArgs>(CollectionView_OnChildAdded);
            ChildRemoved += new EventHandler<ElementEventArgs>(CollectionView_OnChildRemoved);
        }

        private void CollectionView_OnChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is VisualElement ve)
            {
                //Colocando um rastreador
                ve.SizeChanged += new EventHandler(ChildVisualElement_OnHeightChanged);

                if (ve.Height > 0)
                    TrySetHeightDynamically(ve);
            }
        }

        private void CollectionView_OnChildRemoved(object sender, ElementEventArgs e)
        {
            if (e.Element is VisualElement ve)
            {
                ve.SizeChanged -= new EventHandler(ChildVisualElement_OnHeightChanged);

                if (ve.Height > 0)
                    TrySetHeightDynamically(ve);
            }
        }

        private void ChildVisualElement_OnHeightChanged(object sender, EventArgs e)
        {
            if (sender is VisualElement ve)
                TrySetHeightDynamically(ve);
        }

        private void TrySetHeightDynamically(VisualElement childVisualElement)
        {
            var itemsCount = ItemsSource.Cast<object>().Count();

            if (_counteredItems != itemsCount)
            {
                var horizontalSpan = 1;
                double verticalItemsSpacing = 0;
                double rows;

                if (ItemsLayout is GridItemsLayout gridItemsLayout && gridItemsLayout.Span > 1)
                {
                    horizontalSpan = gridItemsLayout.Span;
                    rows = Math.Ceiling((double)itemsCount / horizontalSpan);
                    verticalItemsSpacing = gridItemsLayout.VerticalItemSpacing;
                }
                else
                    rows = itemsCount;

                HeightRequest = rows * childVisualElement.Height + ((rows - 1) * verticalItemsSpacing);

                _counteredItems = itemsCount;
            }
        }
    }
}
