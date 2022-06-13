using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        private void OnCategoryClick(CategoryViewModel category)
        {
            if (CurrentCategory != null)
            {
                CurrentCategory.Activated = false;
            }
            CurrentCategory = category;
            CurrentCategory.Activated = true;
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
                if (Initiallized && CurrentCategory != null && CurrentCategoryName != CurrentCategory.Category.Name)
                {
                    CurrentCategory.Category.UpdateCategoryName(CurrentCategoryName);
                }
            });
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
                if (!int.TryParse(CurrentProductSPrice, out int sprice))
                {
                    return;
                }
                if (!int.TryParse(CurrentProductMPrice, out int mprice))
                {
                    return;
                }
                if (!int.TryParse(CurrentProductLPrice, out int lprice))
                {
                    return;
                }
                CurrentProduct.Product.UpdateProductInfo(CurrentProductName, sprice, mprice, lprice);
            });
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