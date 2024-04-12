using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinUIGallery.ControlPages
{
    public sealed partial class TreeViewPage : Page
    {
        TreeViewNode personalFolder;
        TreeViewNode personalFolder2;
        private ObservableCollection<ExplorerItem> DataSource;

        public TreeViewPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            DataSource = GetData();

            InitializeSampleTreeView();
            InitializeSampleTreeView2();
        }

        private void InitializeSampleTreeView()
        {
            TreeViewNode workFolder = new TreeViewNode() { Content = "Work Documents" };
            workFolder.IsExpanded = true;

            workFolder.Children.Add(new TreeViewNode() { Content = "XYZ Functional Spec" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Schedule" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Overall Project Plan" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Resources Allocation" });

            TreeViewNode remodelFolder = new TreeViewNode() { Content = "Home Remodel" };
            remodelFolder.IsExpanded = true;

            remodelFolder.Children.Add(new TreeViewNode() { Content = "Contractor Contact Info" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Paint Color Scheme" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Flooring woodgrain type" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Kitchen cabinet style" });

            personalFolder = new TreeViewNode() { Content = "Personal Documents" };
            personalFolder.IsExpanded = true;
            personalFolder.Children.Add(remodelFolder);

            sampleTreeView.RootNodes.Add(workFolder);
            sampleTreeView.RootNodes.Add(personalFolder);
        }
        private void InitializeSampleTreeView2()
        {
            TreeViewNode workFolder = new TreeViewNode() { Content = "Work Documents" };
            workFolder.IsExpanded = true;

            workFolder.Children.Add(new TreeViewNode() { Content = "XYZ Functional Spec" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Schedule" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Overall Project Plan" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Resources Allocation" });

            TreeViewNode remodelFolder = new TreeViewNode() { Content = "Home Remodel" };
            remodelFolder.IsExpanded = true;

            remodelFolder.Children.Add(new TreeViewNode() { Content = "Contractor Contact Info" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Paint Color Scheme" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Flooring woodgrain type" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Kitchen cabinet style" });

            personalFolder2 = new TreeViewNode() { Content = "Personal Documents" };
            personalFolder2.IsExpanded = true;
            personalFolder2.Children.Add(remodelFolder);

            sampleTreeView2.RootNodes.Add(workFolder);
            sampleTreeView2.RootNodes.Add(personalFolder2);
        }

        private void sampleTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            return;
        }
        
        private ObservableCollection<ExplorerItem> GetData()
        {
            var list = new ObservableCollection<ExplorerItem>();
            ExplorerItem folder1 = new ExplorerItem()
            {
                Name = "Work Documents",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem()
                    {
                        Name = "Functional Specifications",
                        Type = ExplorerItem.ExplorerItemType.Folder,
                        Children =
                        {
                            new ExplorerItem()
                            {
                                Name = "TreeView spec",
                                Type = ExplorerItem.ExplorerItemType.File,
                              }
                        }
                    },
                    new ExplorerItem()
                    {
                        Name = "Feature Schedule",
                        Type = ExplorerItem.ExplorerItemType.File,
                    },
                    new ExplorerItem()
                    {
                        Name = "Overall Project Plan",
                        Type = ExplorerItem.ExplorerItemType.File,
                    },
                    new ExplorerItem()
                    {
                        Name = "Feature Resources Allocation",
                        Type = ExplorerItem.ExplorerItemType.File,
                    }
                }
            };
            ExplorerItem folder2 = new ExplorerItem()
            {
                Name = "Personal Folder",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                        {
                            new ExplorerItem()
                            {
                                Name = "Home Remodel Folder",
                                Type = ExplorerItem.ExplorerItemType.Folder,
                                Children =
                                {
                                    new ExplorerItem()
                                    {
                                        Name = "Contractor Contact Info",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Paint Color Scheme",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Flooring Woodgrain type",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Kitchen Cabinet Style",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    }
                                }
                            }
                        }
            };

            list.Add(folder1);
            list.Add(folder2);
            return list;
        }

    }

    public partial class ExplorerItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public enum ExplorerItemType { Folder, File };
        public string Name { get; set; }
        public ExplorerItemType Type { get; set; }
        private ObservableCollection<ExplorerItem> m_children;
        public ObservableCollection<ExplorerItem> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new ObservableCollection<ExplorerItem>();
                }
                return m_children;
            }
            set
            {
                m_children = value;
            }
        }

        private bool m_isExpanded;
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    partial class ExplorerItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var explorerItem = (ExplorerItem)item;
            return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
        }
    }
}
