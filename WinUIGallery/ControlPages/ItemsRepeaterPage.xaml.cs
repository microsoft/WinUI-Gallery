using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using WinUIGallery.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.Helper;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ItemsRepeaterPage : ItemsPageBase
    {
        private Random random = new Random();
        private int MaxLength = 425;

        public ObservableCollection<Bar> BarItems;
        public MyItemsSource filteredRecipeData = new MyItemsSource(null);
        public List<Recipe> staticRecipeData;
        private bool IsSortDescending = false;

        private Button LastSelectedColorButton;
        private int PreviouslyFocusedAnimatedScrollRepeaterIndex = -1;

        public ItemsRepeaterPage()
        {
            this.InitializeComponent();
            InitializeData();
            repeater2.ItemsSource = Enumerable.Range(0, 500);
        }

        public List<String> ColorList = new List<String>()
        {
                "Blue",
                "BlueViolet",
                "Crimson",
                "DarkCyan",
                "DarkGoldenrod",
                "DarkMagenta",
                "DarkOliveGreen",
                "DarkRed",
                "DarkSlateBlue",
                "DeepPink",
                "IndianRed",
                "MediumSlateBlue",
                "Maroon",
                "MidnightBlue",
                "Peru",
                "SaddleBrown",
                "SteelBlue",
                "OrangeRed",
                "Firebrick",
                "DarkKhaki"
        };
        private void InitializeData()
        {
            if (BarItems == null)
            {
                BarItems = new ObservableCollection<Bar>();
            }
            BarItems.Add(new Bar(300, this.MaxLength));
            BarItems.Add(new Bar(25, this.MaxLength));
            BarItems.Add(new Bar(175, this.MaxLength));

            List<object> basicData = new List<object>
            {
                64,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                128,
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                256,
                "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                512,
                "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                1024
            };
            MixedTypeRepeater.ItemsSource = basicData;

            List<NestedCategory> nestedCategories = new List<NestedCategory>
            {
                new NestedCategory("Fruits", GetFruits()),
                new NestedCategory("Vegetables", GetVegetables()),
                new NestedCategory("Grains", GetGrains()),
                new NestedCategory("Proteins", GetProteins())
            };


            outerRepeater.ItemsSource = nestedCategories;

            // Set sample code to display on page's initial load
            SampleCodeLayout.Value = @"<StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";

            SampleCodeDT.Value = @"<DataTemplate x:Key=""HorizontalBarTemplate"" x:DataType=""l:Bar"">
    <Border Background=""{ThemeResource SystemChromeLowColor}"" Width=""{x:Bind MaxLength}"" >
        <Rectangle Fill=""{ThemeResource SystemAccentColor}"" Width=""{x:Bind Length}"" 
                   Height=""24"" HorizontalAlignment=""Left""/> 
    </Border>
</DataTemplate>";

            SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";


            animatedScrollRepeater.ItemsSource = ColorList;

            animatedScrollRepeater.ElementPrepared += OnElementPrepared;

            // Initialize custom MyItemsSource object with new recipe data
            List<Recipe> RecipeList = GetRecipeList();
            filteredRecipeData.InitializeCollection(RecipeList);
            // Save a static copy to compare to while filtering
            staticRecipeData = RecipeList;
            VariedImageSizeRepeater.ItemsSource = filteredRecipeData;

        }

        private ObservableCollection<string> GetFruits()
        {
            return new ObservableCollection<string> { "Apricots", "Bananas", "Grapes", "Strawberries", "Watermelon", "Plums", "Blueberries" };
        }

        private ObservableCollection<string> GetVegetables()
        {
            return new ObservableCollection<string> { "Broccoli", "Spinach", "Sweet potato", "Cauliflower", "Onion", "Brussels sprouts", "Carrots" };
        }
        private ObservableCollection<string> GetGrains()
        {
            return new ObservableCollection<string> { "Rice", "Quinoa", "Pasta", "Bread", "Farro", "Oats", "Barley" };
        }
        private ObservableCollection<string> GetProteins()
        {
            return new ObservableCollection<string> { "Steak", "Chicken", "Tofu", "Salmon", "Pork", "Chickpeas", "Eggs" };
        }

        // ==========================================================================
        // Basic, non-interactive ItemsRepeater
        // ==========================================================================
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

        private void RadioBtn_Click(object sender, SelectionChangedEventArgs e)
        {
            string itemTemplateKey = string.Empty;
            var selected = (sender as Microsoft.UI.Xaml.Controls.RadioButtons).SelectedItem;
            if (selected == null)
            {
                // No point in continuing if selected element is null
                return;
            }
            var layoutKey = ((FrameworkElement)selected).Tag as string;

            if (layoutKey.Equals(nameof(this.VerticalStackLayout))) // we used x:Name in the resources which both acts as the x:Key value and creates a member field by the same name
            {
                layout.Value = layoutKey;
                itemTemplateKey = "HorizontalBarTemplate";

                repeater.MaxWidth = MaxLength + 12;

                SampleCodeLayout.Value = @"<StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";
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

                SampleCodeLayout.Value = @"<StackLayout x:Name=""HorizontalStackLayout"" Orientation=""Horizontal"" Spacing=""8""/> ";
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

                SampleCodeLayout.Value = @"<UniformGridLayout x:Name=""UniformGridLayout"" MinRowSpacing=""8"" MinColumnSpacing=""8""/>";
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
        // Virtualizing, scrollable list of items laid out by ItemsRepeater
        // ==========================================================================
        private void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = ((FrameworkElement)sender).Tag as string;

            repeater2.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;

            layout2.Value = layoutKey;

            if (layoutKey == "UniformGridLayout2")
            {
                SampleCodeLayout2.Value = @"<UniformGridLayout x:Key=""UniformGridLayout2"" MinItemWidth=""108"" MinItemHeight=""108""
                   MinRowSpacing=""12"" MinColumnSpacing=""12""/>";
            }
            else if (layoutKey == "MyFeedLayout")
            {
                SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";
            }
        }

        // ==========================================================================
        // Animated Scrolling ItemsRepeater with Content Sample
        // ==========================================================================

        private void OnAnimatedItemGotFocus(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;

            // Store the last focused Index so we can land back on it when focus leaves
            // and comes back to the repeater.
            PreviouslyFocusedAnimatedScrollRepeaterIndex = animatedScrollRepeater.GetElementIndex(sender as UIElement);

            item.StartBringIntoView(new BringIntoViewOptions()
            {
                VerticalAlignmentRatio = 0.5,
                AnimationDesired = true,
            });
        }
        private void OnAnimatedScrollRepeaterGettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            // If we have a previously focused index and focus moving from outside the repeater to inside,
            // then we can pick the previously focused index and land on that item again.
            var lastFocus = args.OldFocusedElement as UIElement;
            if (PreviouslyFocusedAnimatedScrollRepeaterIndex != -1 &&
                lastFocus != null && animatedScrollRepeater.GetElementIndex(lastFocus) == -1)
            {
                var item = animatedScrollRepeater.TryGetElement(PreviouslyFocusedAnimatedScrollRepeaterIndex);
                args.NewFocusedElement = item;
            }
        }

        private void OnAnimatedItemClicked(object sender, RoutedEventArgs e)
        {
            // Update corresponding rectangle with selected color
            Button senderBtn = sender as Button;
            colorRectangle.Fill = senderBtn.Background;
            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, $"Rectangle color set to {(sender as ContentControl).Content}", "RectangleChangedNotificationActivityId");
            SetUIANamesForSelectedEntry(senderBtn);
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

            // Scale the item based on the distance of the item relative to the center of the viewport.
            scaleExpresion.Expression = "1 - abs((svVisual.Size.Y/2 - scrollProperties.Translation.Y) - (item.Offset.Y + item.Size.Y/2))*(.25/(svVisual.Size.Y/2))";

            // Animate the item to change size based on distance from center of viewpoint
            item.StartAnimation("Scale.X", scaleExpresion);
            item.StartAnimation("Scale.Y", scaleExpresion);
            var centerPointExpression = scrollProperties.Compositor.CreateExpressionAnimation();
            centerPointExpression.SetReferenceParameter("item", item);
            centerPointExpression.Expression = "Vector3(item.Size.X/2, item.Size.Y/2, 0)";
            item.StartAnimation("CenterPoint", centerPointExpression);
        }

        private void SetUIANamesForSelectedEntry(Button selectedItem)
        {
            if (LastSelectedColorButton != null && LastSelectedColorButton.Content is string content)
            {
                AutomationProperties.SetName(LastSelectedColorButton, content);
            }

            AutomationProperties.SetName(selectedItem, (string)selectedItem.Content + " , selected");
            LastSelectedColorButton = selectedItem;
        }


        // ==========================================================================
        // VariedImageSize Layout with Filtering/Sorting
        // ==========================================================================
        private List<Recipe> GetRecipeList()
        {
            // Initialize list of recipes for varied image size layout sample
            var rnd = new Random();
            List<Recipe> tempList = new List<Recipe>(
                                        Enumerable.Range(0, 1000).Select(k =>
                                            new Recipe
                                            {
                                                Num = k,
                                                Name = "Recipe " + k.ToString(),
                                                Color = ColorList[rnd.Next(0, 19)]
                                            }));

            foreach (Recipe rec in tempList)
            {
                // Add one food from each option into the recipe's ingredient list and ingredient string
                string fruitOption = GetFruits()[rnd.Next(0, 6)];
                string vegOption = GetVegetables()[rnd.Next(0, 6)];
                string grainOption = GetGrains()[rnd.Next(0, 6)];
                string proteinOption = GetProteins()[rnd.Next(0, 6)];
                rec.Ingredients = "\n" + fruitOption + "\n" + vegOption + "\n" + grainOption + "\n" + proteinOption;
                rec.IngList = new List<string>() { fruitOption, vegOption, grainOption, proteinOption };

                // Add extra ingredients so items have varied heights in the layout
                rec.RandomizeIngredients();
            }

            return tempList;
        }
        private void OnEnableAnimationsChanged(object sender, RoutedEventArgs e)
        {
        }

        public void FilterRecipes_FilterChanged(object sender, RoutedEventArgs e)
        {
            UpdateSortAndFilter();
        }

        private void OnSortAscClick(object sender, RoutedEventArgs e)
        {
            if (IsSortDescending == true)
            {
                IsSortDescending = false;
                UpdateSortAndFilter();
            }
        }

        private void OnSortDesClick(object sender, RoutedEventArgs e)
        {
            if (!IsSortDescending == true)
            {
                IsSortDescending = true;
                UpdateSortAndFilter();
            }
        }

        private void UpdateSortAndFilter()
        {
            // Find all recipes that ingredients include what was typed into the filtering text box
            var filteredTypes = staticRecipeData.Where(i => i.Ingredients.Contains(FilterRecipes.Text, StringComparison.InvariantCultureIgnoreCase));
            // Sort the recipes by whichever sorting mode was last selected (least to most ingredients by default)
            var sortedFilteredTypes = IsSortDescending ?
                filteredTypes.OrderByDescending(i => i.IngList.Count()) :
                filteredTypes.OrderBy(i => i.IngList.Count());
            // Re-initialize MyItemsSource object with this newly filtered data
            filteredRecipeData.InitializeCollection(sortedFilteredTypes);

            var peer = FrameworkElementAutomationPeer.FromElement(VariedImageSizeRepeater);

            peer.RaiseNotificationEvent(AutomationNotificationKind.Other, AutomationNotificationProcessing.ImportantMostRecent, $"Filtered recipes, {sortedFilteredTypes.Count()} results.", "RecipesFilteredNotificationActivityId");
        }

        private void OnAnimatedScrollRepeaterKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Handled != true)
            {
                var targetIndex = -1;
                if (e.Key == Windows.System.VirtualKey.Home)
                {
                    targetIndex = PreviouslyFocusedAnimatedScrollRepeaterIndex != 0 ? 0 : -1;
                }
                else if (e.Key == Windows.System.VirtualKey.End)
                {
                    targetIndex = PreviouslyFocusedAnimatedScrollRepeaterIndex != animatedScrollRepeater.ItemsSourceView.Count - 1 ?
                        animatedScrollRepeater.ItemsSourceView.Count - 1 : -1;
                }

                if (targetIndex != -1)
                {
                    var element = animatedScrollRepeater.GetOrCreateElement(targetIndex);
                    element.StartBringIntoView();
                    (element as Control).Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }
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


    public partial class MyDataTemplateSelector : DataTemplateSelector
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

    public partial class StringOrIntTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(string))
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
        public int Num { get; set; }
        public string Ingredients { get; set; }
        public List<string> IngList { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int NumIngredients
        {
            get
            {
                return IngList.Count();
            }
        }

        public void RandomizeIngredients()
        {
            // To give the items different heights, give recipes random numbers of random ingredients
            Random rndNum = new Random();
            Random rndIng = new Random();

            ObservableCollection<string> extras = new ObservableCollection<string>{
                                                         "Garlic",
                                                         "Lemon",
                                                         "Butter",
                                                         "Lime",
                                                         "Feta Cheese",
                                                         "Parmesan Cheese",
                                                         "Breadcrumbs"};
            for (int i = 0; i < rndNum.Next(0, 4); i++)
            {
                string newIng = extras[rndIng.Next(0, 6)];
                if (!IngList.Contains(newIng))
                {
                    Ingredients += "\n" + newIng;
                    IngList.Add(newIng);
                }
            }

        }
    }

    // Custom data source class that assigns elements unique IDs, making filtering easier
    public partial class MyItemsSource : IList, Microsoft.UI.Xaml.Controls.IKeyIndexMapping, INotifyCollectionChanged
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
                inner.AddRange(collection);
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
        public string KeyFromIndex(int index)
        {
            return inner[index].Num.ToString();
        }

        public int IndexFromKey(string key)
        {
            foreach (Recipe item in inner)
            {
                if (item.Num.ToString() == key)
                {
                    return inner.IndexOf(item);
                }
            }
            return -1;
        }

        #endregion

        #region Unused List methods
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
