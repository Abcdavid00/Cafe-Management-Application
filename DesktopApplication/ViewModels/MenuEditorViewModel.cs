using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
                OnPropertyChanged(nameof(UnassignedDisplayProducts));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductViewModel> UnassignedDisplayProducts
        {
            get
            {
                if (UnassignedProducts == null)
                {
                    return null;
                }
                    
                if (string.IsNullOrEmpty(productSearchText))
                {
                    return new ObservableCollection<ProductViewModel>(UnassignedProducts);
                }
                else
                {
                    return new ObservableCollection<ProductViewModel>(UnassignedProducts.Where(p => p.Product.Name.ToLower().Contains(productSearchText.ToLower())));
                }
            }
        }
        
        public MenuEditorViewModel()
        {
            Initiallized = false;
            categoriesInitiallized = false;
            unassignedProductsInitiallized = false;
            RefreshCategories();
            RefreshUnassignedProducts();
            ProductSearchText = "";
        }

        private CategoryViewModel currentCategory;
        private CategoryViewModel CurrentCategory
        {
            get => currentCategory;
            set
            {
                currentCategory = value;
                CurrentCategoryName = (currentCategory == null ? "" : currentCategory.Category.Name);
                OnPropertyChanged(nameof(RemoveCategoryButtonVisibility));
                OnPropertyChanged(nameof(CategoryNameTextBoxVisibility));
                OnPropertyChanged(nameof(CategoryNameSaveButtonVisibility));
                
            }
        }

        private ProductViewModel currentProduct;
        private ProductViewModel CurrentProduct
        {
            get => currentProduct;
            set
            {
                currentProduct = value;               
                OnPropertyChanged(nameof(ProductDetailsVisibility));
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
                CurrentProductName = (currentProduct == null ? "" : currentProduct.Product.Name);
                CurrentProductSPrice = (currentProduct == null ? "" : currentProduct.Product.SPrice.ToString());
                CurrentProductMPrice = (currentProduct == null ? "" : currentProduct.Product.MPrice.ToString());
                CurrentProductLPrice = (currentProduct == null ? "" : currentProduct.Product.LPrice.ToString());
            }
        }

        private bool initiallized;
        private bool Initiallized
        {
            get => initiallized;
            set
            {
                initiallized = value;
                OnPropertyChanged(nameof(RemoveCategoryButtonVisibility));
                OnPropertyChanged(nameof(CategoryNameTextBoxVisibility));
                OnPropertyChanged(nameof(CategoryNameSaveButtonVisibility));
                OnPropertyChanged(nameof(CategoryNameSaveCommand));
                OnPropertyChanged(nameof(ProductDetailsVisibility));
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
                OnPropertyChanged();
            }
        }
        

        private bool categoriesInitiallized;

        private async void RefreshCategories()
        {
            List<Category> categories = (await Database.GetAllCategoriesAsync()).ToList();

            Categories?.Clear();
            Categories = new ObservableCollection<CategoryViewModel>(categories.Select(c => new CategoryViewModel(c)));
            categories.Clear();

            string currentCategoryID = (CurrentCategory == null ? "" : CurrentCategory.Category.CategoryID);
            CurrentCategory = null;
            foreach (CategoryViewModel category in Categories)
            {
                if (category.Category.CategoryID == currentCategoryID)
                {
                    CurrentCategory = category;
                }
                category.OnTitleClicked += ((object o, Category c) => OnCategoryClick(o as CategoryViewModel));
                category.OnProductEditButtonClicked += ((object o, ProductViewModel p) => OnProductEditClick(p));
                category.OnProductsListChanged += ((object o, EventArgs e) => RefreshUnassignedProducts());
            }
            
            categoriesInitiallized = true;
            if (unassignedProductsInitiallized)
            {
                Initiallized = true;
            }
            
        }

        private string OnCategoryClick(CategoryViewModel category)
        {
            if (CurrentCategory != null)
            {
                CurrentCategory.Activated = false;
            }
            CurrentCategory = category;
            CurrentCategory.Activated = true;
            return category.Name;
        }

        private void OnProductEditClick(ProductViewModel product)
        {
            if (CurrentProduct != null)
            {
                CurrentProduct.Activated = false;
            }
            CurrentProduct = product;
            CurrentProduct.Activated = true;
        }

        public async void OnProductAddButtonClicked(Product product)
        {
            if (CurrentCategory != null)
            {
                await CurrentCategory.Category.AddProduct(product);               
            }
        }

        private bool unassignedProductsInitiallized;

        private async void RefreshUnassignedProducts()
        {
            List<Product> products = (await Database.GetAllUnassignedProductsAsync()).ToList();

            UnassignedProducts?.Clear();
            UnassignedProducts = new ObservableCollection<ProductViewModel>(products.Select(p => new ProductViewModel(p)));
            products.Clear();

            string currentProductID = (CurrentProduct == null ? "" : CurrentProduct.Product.ProductID);
            CurrentProduct = null;
            foreach (ProductViewModel product in UnassignedProducts)
            {
                if (product.Product.ProductID == currentProductID)
                {
                    CurrentProduct = product;
                }
                product.OnAddButtonClicked += ((object o, Product p) => OnProductAddButtonClicked(p));
                product.OnEditButtonClicked += ((object o, Product p) => OnProductEditClick(o as ProductViewModel));
            }

            unassignedProductsInitiallized = true;
            if (categoriesInitiallized)
            {
                Initiallized = true;
            }
        }

        public ICommand AddProductCommand
        {
            get => new CommandBase(AddProduct);
        }

        private async void AddProduct()
        {
            string result = await CreateNewProduct();
            Console.WriteLine(result);
            RefreshUnassignedProducts();
        }

        private async Task<string> CreateNewProduct()
        {
            await Database.CreateProductAsync("New Product");
            return "New Product has just been created";
        }


        public ICommand AddCategoryCommand
        {
            get => new CommandBase(AddCategory);
        }

        private async void AddCategory()
        {
            string r = await CreateNewCategory();
            Console.WriteLine(r);
            RefreshCategories();
        }

        private async Task<string> CreateNewCategory()
        {
            await Database.CreateCategoryAsync("New Category");
            return "New Category has just been created";
        }

        public Visibility RemoveCategoryButtonVisibility
        {
            get
            {
                if (Initiallized && CurrentCategory != null) 
                {
                    if (CurrentCategory.Category.Products != null)
                    {
                        return CurrentCategory.Category.Products.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    }                 
                }
                return Visibility.Collapsed;
            } 
        }

        public ICommand RemoveCategoryCommand
        {
            get => new CommandBase(RemoveCategory);
        }

        private async void RemoveCategory()
        {
            if (Initiallized && CurrentCategory != null)
            {
                if (CurrentCategory.Category.Products.Count == 0)
                {
                    await Database.RemoveCategoryAsync(CurrentCategory.Category.CategoryID);
                    RefreshCategories();
                }
            }
        }

        private string productSearchText;
        public string ProductSearchText
        {
            get => productSearchText;
            set
            {
                productSearchText = value;
                OnPropertyChanged(nameof(UnassignedDisplayProducts));
                OnPropertyChanged();               
            }
        }

        private string currentCategoryName;
        public string CurrentCategoryName
        {
            get => currentCategoryName;
            set
            {
                currentCategoryName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CategoryNameSaveButtonVisibility));
                OnPropertyChanged(nameof(CategoryNameSaveCommand));
            }
        }
        
        public Visibility CategoryNameTextBoxVisibility
        {
            get => (Initiallized && CurrentCategory != null) ? Visibility.Visible : Visibility.Collapsed;
        }
        
        public Visibility CategoryNameSaveButtonVisibility
        {
            get => (Initiallized && CurrentCategory != null && CurrentCategoryName!=CurrentCategory.Category.Name) ? Visibility.Visible : Visibility.Collapsed;
        }
        
        public ICommand CategoryNameSaveCommand
        {
            get => new CommandBase(() =>
            {
                Console.WriteLine(SaveCategoryName());
            });
        }

        public string SaveCategoryName()
        {
            if (Initiallized && CurrentCategory != null && CurrentCategoryName != CurrentCategory.Category.Name)
            {
                CurrentCategory.Category.UpdateCategoryName(CurrentCategoryName);
                return "Category name has been changed to: " + CurrentCategoryName;
            }
            return "Category name is failed to save";
        }

        
        #region Product details
        
        public Visibility ProductDetailsVisibility
        {
            get => (Initiallized && CurrentProduct != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility ProductDetailsSaveButtonVisibility
        {
            get => (Initiallized && CurrentProduct != null && isCurrentPriceChangedAndValid) ? Visibility.Visible : Visibility.Collapsed;
        }

        public ICommand ProductDetailsSaveCommand
        {
            get => new CommandBase(() =>
            {
                Console.WriteLine(SaveProductDetails());
            });
        }

        public string SaveProductDetails()
        {
            if (!int.TryParse(CurrentProductSPrice, out int sprice))
            {
                return "Small size price is invalid";
            }
            if (!int.TryParse(CurrentProductMPrice, out int mprice))
            {
                return "Medium size price is invalid";
            }
            if (!int.TryParse(CurrentProductLPrice, out int lprice))
            {
                return "Large size price is invalid";
            }
            CurrentProduct.Product.UpdateProductInfo(CurrentProductName, sprice, mprice, lprice);
            return "Product details save successfully";
        }

        private string currentProductName;
        public string CurrentProductName
        {
            get => currentProductName;
            set
            {
                currentProductName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
            }
        }

        private bool isCurrentPriceChangedAndValid
        {
            get
            {
                bool changed = (CurrentProductName!=CurrentProduct.Product.Name);
                if (int.TryParse(CurrentProductSPrice, out int sPrice))
                {
                    if (sPrice != CurrentProduct.Product.SPrice)
                    {
                        changed = true;
                    }
                } else
                {
                    return false;
                }
                if (int.TryParse(CurrentProductMPrice, out int mPrice))
                {
                    if (mPrice != CurrentProduct.Product.MPrice)
                    {
                        changed = true;
                    }
                }
                else
                {
                    return false;
                }
                if (int.TryParse(CurrentProductLPrice, out int lPrice))
                {
                    if (lPrice != CurrentProduct.Product.LPrice)
                    {
                        changed = true;
                    }
                }
                else
                {
                    return false;
                }
                return changed;
            }
        }

        private string currentProductSPrice;
        public string CurrentProductSPrice
        {
            get => currentProductSPrice;
            set
            {
                currentProductSPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
            }
        }

        private string currentProductMPrice;
        public string CurrentProductMPrice
        {
            get => currentProductMPrice;
            set
            {
                currentProductMPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
            }
        }

        private string currentProductLPrice;
        public string CurrentProductLPrice
        {
            get => currentProductLPrice;
            set
            {
                currentProductLPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductDetailsSaveButtonVisibility));
            }
        }

        #endregion
    }
}