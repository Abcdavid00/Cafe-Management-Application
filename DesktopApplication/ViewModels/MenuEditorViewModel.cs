using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class MenuEditorViewModel : ViewModelBase
    {
        private ObservableCollection<CategoryViewModel> categories;
        public ObservableCollection<CategoryViewModel> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ProductViewModel> unassignedProducts;
        public ObservableCollection<ProductViewModel> UnassignedProducts
        {
            get => unassignedProducts;
            private set
            {
                unassignedProducts = value;
                OnPropertyChanged();
            }
        }
        
        public MenuEditorViewModel()
        {
            initiallized = false;
            categoriesInitiallized = false;
            unassignedProductsInitiallized = false;
            RefreshCategories();
            RefreshUnassignedProducts();
            
        }

        private bool initiallized;

        private bool categoriesInitiallized;

        private async void RefreshCategories()
        {
            List<Category> categories = (await Database.GetAllCategoriesAsync()).ToList();
            #region debug
//#if DEBUG
//            categories = new List<Category>();
//            categories.Add(new Category() { Name = "Coffee" });
//            categories.Last().Products = new List<Product>();
//            categories.Last().Products.Add(new Product() { Name = "White Coffee", CategoryID="1" , SPrice = 10000, MPrice = 20000, LPrice = 30000 });
//            categories.Last().Products.Add(new Product() { Name = "Black Coffee", CategoryID = "1", SPrice = 5000, MPrice = 15000, LPrice = 20000 });
//            categories.Add(new Category() { Name = "Milk Tea" });
//            categories.Last().Products = new List<Product>();
//            categories.Last().Products.Add(new Product() { Name = "Special", CategoryID = "1", SPrice = 10000, MPrice = 20000, LPrice = 30000 });
//            categories.Last().Products.Add(new Product() { Name = "Traditional", CategoryID = "1", SPrice = 5000, MPrice = 15000, LPrice = 20000 });
//#endif
            #endregion
            Categories?.Clear();
            Categories = new ObservableCollection<CategoryViewModel>(categories.Select(c => new CategoryViewModel(c)));
            categories.Clear();

            categoriesInitiallized = true;
            if (unassignedProductsInitiallized)
            {
                initiallized = true;
            }
            
        }

        private bool unassignedProductsInitiallized;

        private async void RefreshUnassignedProducts()
        {
            List<Product> products = (await Database.GetAllUnassignedProductsAsync()).ToList();
            #region debug
//#if DEBUG
//            products = new List<Product>();
//            products.Add(new Product() { Name = "White Coffee", SPrice = 10000, MPrice = 20000, LPrice = 30000 });
//            products.Add(new Product() { Name = "Black Coffee", SPrice = 5000, MPrice = 15000, LPrice = 20000 });
//            products.Add(new Product() { Name = "Special", SPrice = 10000, MPrice = 20000, LPrice = 30000 });
//            products.Add(new Product() { Name = "Traditional", SPrice = 5000, MPrice = 15000, LPrice = 20000 });
//#endif
            #endregion
            UnassignedProducts?.Clear();
            UnassignedProducts = new ObservableCollection<ProductViewModel>(products.Select(p => new ProductViewModel(p)));
            products.Clear();

            unassignedProductsInitiallized = true;
            if (categoriesInitiallized)
            {
                initiallized = true;
            }
        }

        public ICommand AddProductCommand
        {
            get => new CommandBase(CreateNewProduct);
        }

        private async void CreateNewProduct()
        {
            await Database.CreateProductAsync("New Product");
            RefreshUnassignedProducts();
        }

        public ICommand AddCategoryCommand
        {
            get => new CommandBase(CreateNewCategory);
        }

        private async void CreateNewCategory()
        {
            await Database.CreateCategoryAsync("New Category");
            RefreshCategories();
        }
    }
}