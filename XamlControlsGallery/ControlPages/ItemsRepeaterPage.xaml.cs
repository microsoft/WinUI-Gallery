using AppUIBasics.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsRepeaterPage : ItemsPageBase
    {
        private Random random = new Random();
        private int MaxLength = 425;
        private bool isHorizontal = false;

        public ObservableCollection<Bar> BarItems;
        public MyItemsSource filteredRecipeData = new MyItemsSource(null);
        public List<Recipe> staticRecipeData;
        //public List<Recipe> tempFilteredRecipeData;

        private double AnimatedBtnHeight;
        private Windows.UI.Xaml.Thickness AnimatedBtnMargin;
        public ItemsRepeaterPage()
        {
            this.InitializeComponent();
            InitializeData();
            repeater2.ItemsSource = Enumerable.Range(0, 500);
        }

        private void InitializeData()
        {
            if (BarItems == null)
            {
                BarItems = new ObservableCollection<Bar>();
            }
            BarItems.Add(new Bar(300, this.MaxLength));
            BarItems.Add(new Bar(25, this.MaxLength));
            BarItems.Add(new Bar(175, this.MaxLength));

            List<object> basicData = new List<object>();
            basicData.Add(64);
            basicData.Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
            basicData.Add(128);
            basicData.Add("Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.");
            basicData.Add(256);
            basicData.Add("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.");
            basicData.Add(512);
            basicData.Add("Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            basicData.Add(1024);
            MixedTypeRepeater.ItemsSource = basicData;

            List<NestedCategory> nestedCategories = new List<NestedCategory>();
            nestedCategories.Add(
                new NestedCategory("Fruits", new ObservableCollection<string>{
                                                            "Apricots",
                                                            "Bananas",
                                                            "Grapes",
                                                            "Strawberries",
                                                            "Watermelon",
                                                            "Plums",
                                                            "Blueberries"
                }));

            nestedCategories.Add(
                new NestedCategory("Vegetables", new ObservableCollection<string>{
                                                            "Broccoli",
                                                            "Spinach",
                                                            "Sweet potato",
                                                            "Cauliflower",
                                                            "Onion",
                                                            "Brussel sprouts",
                                                            "Carrots"
                }));

            nestedCategories.Add(
                new NestedCategory("Grains", new ObservableCollection<string>{
                                                            "Rice",
                                                            "Quinoa",
                                                            "Pasta",
                                                            "Bread",
                                                            "Farro",
                                                            "Oats",
                                                            "Barley"
                }));

            nestedCategories.Add(
                new NestedCategory("Proteins", new ObservableCollection<string>{
                                                            "Steak",
                                                            "Chicken",
                                                            "Tofu",
                                                            "Salmon",
                                                            "Pork",
                                                            "Chickpeas",
                                                            "Eggs"
                }));

            outerRepeater.ItemsSource = nestedCategories;

            // Set sample code to display on page's initial load
            SampleCodeLayout.Value = @"<muxc:StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";

            SampleCodeDT.Value = @"<DataTemplate x:Key=""HorizontalBarTemplate"" x:DataType=""l:Bar"">
    <Border Background=""{ThemeResource SystemChromeLowColor}"" Width=""{x:Bind MaxLength}"" >
        <Rectangle Fill=""{ThemeResource SystemAccentColor}"" Width=""{x:Bind Length}"" 
                   Height=""24"" HorizontalAlignment=""Left""/> 
    </Border>
</DataTemplate>";

            SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";


            IList<string> colors = new List<String>();
            colors.Add("BlueViolet");
            colors.Add("BurlyWood");
            colors.Add("Crimson");
            colors.Add("DarkCyan");
            colors.Add("DarkSalmon");
            colors.Add("MediumAquamarine");
            colors.Add("DodgerBlue");
            colors.Add("Firebrick");
            colors.Add("Goldenrod");
            colors.Add("Orange");
            colors.Add("PaleVioletRed");
            colors.Add("Lime");
            colors.Add("MediumOrchid");
            colors.Add("SeaGreen");
            colors.Add("SandyBrown");
            colors.Add("SteelBlue");

            animatedScrollRepeater.ItemsSource = colors;
            animatedScrollRepeater.ElementPrepared += OnElementPrepared;

            PinterestRepeater.ItemTemplate = VirtualizingItemFactory;
            string _lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam laoreet erat vel massa rutrum, eget mollis massa vulputate. Vivamus semper augue leo, eget faucibus nulla mattis nec. " +
                "Donec scelerisque lacus at dui ultricies, eget auctor ipsum placerat. Integer aliquet libero sed nisi eleifend, nec rutrum arcu lacinia. Sed a sem et ante gravida congue sit amet ut augue. " +
                "Donec quis pellentesque urna, non finibus metus. Proin sed ornare tellus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam laoreet erat vel massa rutrum, eget mollis massa vulputate." +
                " Vivamus semper augue leo, eget faucibus nulla mattis nec. Donec scelerisque lacus at dui ultricies, eget auctor ipsum placerat. Integer aliquet libero sed nisi eleifend, nec rutrum arcu lacinia. " +
                "Sed a sem et ante gravida congue sit amet ut augue. Donec quis pellentesque urna, non finibus metus. Proin sed ornare tellus.";
        
            var rnd = new Random();
            List<Recipe> tempList = new List<Recipe>(
                                        Enumerable.Range(0, 100).Select(k =>
                                           new Recipe
                                           {
                                               PrimaryKey = k.ToString(),
                                               ImageUri = string.Format("/Assets/SampleMedia/LandscapeImage{0}.jpg", k % 8 + 1),
                                               Description = k + " - " + _lorem.Substring(0, rnd.Next(50, 350))
                                           }));

            filteredRecipeData.InitializeCollection(tempList);

            staticRecipeData = new List<Recipe>(tempList);
            PinterestRepeater.ItemsSource = filteredRecipeData;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            BarItems.Add(new Bar(random.Next(this.MaxLength), this.MaxLength));
            DeleteBtn.IsEnabled = true;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BarItems.Count > 0)
            {
                BarItems.RemoveAt(0);
                if (BarItems.Count == 0)
                {
                    DeleteBtn.IsEnabled = false;
                }
            }
        }

        private void OrientationBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = String.Empty, itemTemplateKey = String.Empty;

            if (isHorizontal)
            {
                layoutKey = "VerticalStackLayout";
                itemTemplateKey = "HorizontalBarTemplate";
            }
            else
            {
                layoutKey = "HorizontalStackLayout";
                itemTemplateKey = "VerticalBarTemplate";
            }

            repeater.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;
            repeater.ItemTemplate = Resources[itemTemplateKey] as DataTemplate;
            repeater.ItemsSource = BarItems;

            layout.Value = layoutKey;
            elementGenerator.Value = itemTemplateKey;

            isHorizontal = !isHorizontal;
        }

        private void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = ((FrameworkElement)sender).Tag as string;

            repeater2.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;

            layout2.Value = layoutKey;

            if (layoutKey == "UniformGridLayout2")
            {
                SampleCodeLayout2.Value = @"<muxc:UniformGridLayout x:Key=""UniformGridLayout2"" MinItemWidth=""108"" MinItemHeight=""108""
                   MinRowSpacing=""12"" MinColumnSpacing=""12""/>";
            }
            else if (layoutKey == "MyFeedLayout")
            {
                SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";
            }
        }

        private void RadioBtn_Click(object sender, RoutedEventArgs e)
        {
            string itemTemplateKey = String.Empty;
            var layoutKey = ((FrameworkElement)sender).Tag as string;

            if (layoutKey.Equals(nameof(this.VerticalStackLayout))) // we used x:Name in the resources which both acts as the x:Key value and creates a member field by the same name
            {
                layout.Value = layoutKey;
                itemTemplateKey = "HorizontalBarTemplate";

                repeater.MaxWidth = MaxLength + 12;

                SampleCodeLayout.Value = @"<muxc:StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""HorizontalBarTemplate"" x:DataType=""l:Bar"">
    <Border Background=""{ThemeResource SystemChromeLowColor}"" Width=""{x:Bind MaxLength}"" >
        <Rectangle Fill=""{ThemeResource SystemAccentColor}"" Width=""{x:Bind Length}""
                   Height=""24"" HorizontalAlignment=""Left""/> 
    </Border>
</DataTemplate>";
            }
            else if (layoutKey.Equals(nameof(this.HorizontalStackLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "VerticalBarTemplate";

                repeater.MaxWidth = 6000;

                SampleCodeLayout.Value = @"<muxc:StackLayout x:Name=""HorizontalStackLayout"" Orientation=""Horizontal"" Spacing=""8""/> ";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""VerticalBarTemplate"" x:DataType=""l:Bar"">
    <Border Background=""{ThemeResource SystemChromeLowColor}"" Height=""{x:Bind MaxHeight}"">
        <Rectangle Fill=""{ThemeResource SystemAccentColor}"" Height=""{x:Bind Height}"" 
                   Width=""48"" VerticalAlignment=""Top""/>
    </Border>
</DataTemplate>";
            }
            else if (layoutKey.Equals(nameof(this.UniformGridLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "CircularTemplate";

                repeater.MaxWidth = 540;

                SampleCodeLayout.Value = @"<muxc:UniformGridLayout x:Name=""UniformGridLayout"" MinRowSpacing=""8"" MinColumnSpacing=""8""/>";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""CircularTemplate"" x:DataType=""l:Bar"">
    <Grid>
        <Ellipse Fill=""{ThemeResource SystemChromeLowColor}"" Height=""{x:Bind MaxDiameter}"" 
                 Width=""{x:Bind MaxDiameter}"" VerticalAlignment=""Center"" HorizontalAlignment=""Center""/>
        <Ellipse Fill=""{ThemeResource SystemAccentColor}"" Height=""{x:Bind Diameter}"" 
                 Width=""{x:Bind Diameter}"" VerticalAlignment=""Center"" HorizontalAlignment=""Center""/>
    </Grid>
</DataTemplate>";
            }
            repeater.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;
            repeater.ItemTemplate = Resources[itemTemplateKey] as DataTemplate;
            repeater.ItemsSource = BarItems;

            elementGenerator.Value = itemTemplateKey;
        }

        // ==========================================================================
        // Animated Scrolling ItemsRepeater with Content Sample
        // ==========================================================================

        private void Animated_GotItem(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            item.StartBringIntoView(new BringIntoViewOptions()
            {
                VerticalAlignmentRatio = 0.5,
                AnimationDesired = true,
            });

            // Update corresponding rectangle with selected color
            Button senderBtn = sender as Button;
            colorRectangle.Fill = senderBtn.Background;
        }


        /* This function occurs each time an element is made ready for use.
         * This is necessary for virtualization. */
        private void OnElementPrepared(Microsoft.UI.Xaml.Controls.ItemsRepeater sender, Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs args)
        {
            var item = ElementCompositionPreview.GetElementVisual(args.Element);
            var svVisual = ElementCompositionPreview.GetElementVisual(Animated_ScrollViewer);
            var scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(Animated_ScrollViewer);

            var scaleExpresion = scrollProperties.Compositor.CreateExpressionAnimation();
            scaleExpresion.SetReferenceParameter("svVisual", svVisual);
            scaleExpresion.SetReferenceParameter("scrollProperties", scrollProperties);
            scaleExpresion.SetReferenceParameter("item", item);

            // scale the item based on the distance of the item relative to the center of the viewport.
            scaleExpresion.Expression = "1 - abs((svVisual.Size.Y/2 - scrollProperties.Translation.Y) - (item.Offset.Y + item.Size.Y/2))*(.25/(svVisual.Size.Y/2))";

            item.StartAnimation("Scale.X", scaleExpresion);
            item.StartAnimation("Scale.Y", scaleExpresion);
            var centerPointExpression = scrollProperties.Compositor.CreateExpressionAnimation();
            centerPointExpression.SetReferenceParameter("item", item);
            centerPointExpression.Expression = "Vector3(item.Size.X/2, item.Size.Y/2, 0)";
            item.StartAnimation("CenterPoint", centerPointExpression);
        }
        private void GetButtonSize(object sender, RoutedEventArgs e)
        {
            Button AnimatedBtn = sender as Button;
            AnimatedBtnHeight = AnimatedBtn.ActualHeight;
            AnimatedBtnMargin = AnimatedBtn.Margin;

        }

        private void Animated_ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            Button SelectedItem = GetSelectedItemFromViewport() as Button;
            // Update corresponding rectangle with selected color
            colorRectangle.Fill = SelectedItem.Background;
        }

        // Find centerpoint of ScrollViewer
        private double CenterPointOfViewportInExtent()
        {
            return Animated_ScrollViewer.VerticalOffset + Animated_ScrollViewer.ViewportHeight / 2;
        }

        // Find index of the item that's at the center of the viewport
        private int GetSelectedIndexFromViewport()
        {
            int selectedItemIndex = (int)Math.Floor(CenterPointOfViewportInExtent() / ((double)AnimatedBtnMargin.Top + AnimatedBtnHeight));
            selectedItemIndex %= animatedScrollRepeater.ItemsSourceView.Count;
            return selectedItemIndex;
        }

        private object GetSelectedItemFromViewport()
        {
            var selectedIndex = GetSelectedIndexFromViewport();
            var selectedElement = animatedScrollRepeater.TryGetElement(selectedIndex) as Button;
            return selectedElement;
        }

        // ==========================================================================
        // Pinterest Layout with Filtering/Sorting
        // ==========================================================================
        public void FilterRecipes_FilterChanged(object sender, RoutedEventArgs e)
        {
            IList<Recipe> newFilteredData = new List<Recipe>();
            // Linq query that selects only items that return True after being passed through Filter function
            foreach (Recipe element in staticRecipeData)
            {
                if (element.Description.Contains(FilterRecipes.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    newFilteredData.Add(element);
                }
            }
            //Remove_NonMatching(newFilteredData);
            //AddBack_Elements(newFilteredData);
            filteredRecipeData.InitializeCollection(newFilteredData);
        }

        //private void Remove_NonMatching(IEnumerable<Recipe> tempFiltered)
        //{
        //    for (int i = filteredRecipeData.Count - 1; i >= 0; i--)
        //    {
        //        var item = filteredRecipeData[i];
        //        // If contact is not in the filtered argument list, remove it from the ListView's source.
        //        if (!tempFiltered.Contains(item))
        //        {
        //            filteredRecipeData.Remove(item);
        //        }
        //    }
        //}

        //private void AddBack_Elements(IEnumerable<Recipe> tempFiltered)
        //// When a user hits backspace, more contacts may need to be added back into the list
        //{
        //    foreach (var item in tempFiltered)
        //    {
        //        // If item in filtered list is not currently in ListView's source collection, add it back in
        //        if (!filteredRecipeData.Contains(item))
        //        {
        //            filteredRecipeData.Add(item);
        //        }
        //    }
        //}
    }

    public class NestedCategory
    {
        public string CategoryName { get; set; }
        public ObservableCollection<string> CategoryItems { get; set; }
        public NestedCategory(string catName, ObservableCollection<string> catItems)
        {
            CategoryName = catName;
            CategoryItems = catItems;
        }
    }


    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Accent { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if ((int)item % 2 == 0)
            {
                return Normal;
            }
            else
            {
                return Accent;
            }
        }
    }

    public class StringOrIntTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(String))
            {
                return StringTemplate;
            }
            else if (item.GetType() == typeof(int))
            {
                return IntTemplate;
            }
            else
            {
                return null;
            }
        }
    }

    public class Bar
    {
        public Bar(double length, int max)
        {
            Length = length;
            MaxLength = max;

            Height = length / 4;
            MaxHeight = max / 4;

            Diameter = length / 6;
            MaxDiameter = max / 6;
        }
        public double Length { get; set; }
        public int MaxLength { get; set; }

        public double Height { get; set; }
        public double MaxHeight { get; set; }

        public double Diameter { get; set; }
        public double MaxDiameter { get; set; }
    }

    public class Recipe
    { 
        public string ImageUri { get; set; }
        public string Description { get; set; }
        public string PrimaryKey { get; set; }
    }

    // Custom data source class that assigns elements unique IDs, making filtering easier
    public class MyItemsSource : IList, Microsoft.UI.Xaml.Controls.IKeyIndexMapping, INotifyCollectionChanged
    {
        private List<Recipe> inner = new List<Recipe>();

        public MyItemsSource(IEnumerable<Recipe> collection)
        {
            InitializeCollection(collection);
        }

        public void InitializeCollection(IEnumerable<Recipe> collection)
        {
            inner.Clear();
            if (collection != null)
            {
                Debug.Print("adding to collection\n");
                inner.AddRange(collection);
            }

            if (CollectionChanged != null)
            {
                Debug.Print("collection changed\n");
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #region IReadOnlyList<T>


        public int Count => this.inner != null ? this.inner.Count : 0;

        public object this[int index]
        {
            get
            {
                return inner[index] as Recipe;
            }

            set
            {
                inner[index] = (Recipe)value;
            }
        }


        public IEnumerator<Recipe> GetEnumerator() => this.inner.GetEnumerator();

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IKeyIndexMapping

        private int lastRequestedIndex = IndexNotFound;
        private const int IndexNotFound = -1;

        public int IndexFromKey(string key)
        {
            // We'll try to increase our odds of finding a match sooner by starting from the
            // position that we know was last requested and search forward.
            var start = lastRequestedIndex;
            for (int i = start; i < this.Count; i++)
            {
                if (((Recipe)this[i]).PrimaryKey.Equals(key))
                    return i;
            }

            // Then try searching backward.
            start = Math.Min(this.Count - 1, lastRequestedIndex);
            for (int i = start; i >= 0; i--)
            {
                if (((Recipe)this[i]).PrimaryKey.Equals(key))
                    return i;
            }

            return IndexNotFound;
        }

        public string KeyFromIndex(int index)
        {
            var key = ((Recipe)this[index]).PrimaryKey;
            lastRequestedIndex = index;
            return key;
        }
        
        // Unused List methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();


        #endregion
    }


}
