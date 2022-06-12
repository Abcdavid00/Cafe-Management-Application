using System.Collections.Generic;

namespace CSWBManagementApplication.ViewModels
{
    internal class MenuEditorViewModel : ViewModelBase
    {
        private List<CategoryViewModel> categories;
        public List<CategoryViewModel> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                OnPropertyChanged();
            }
        }

        private List<ProductViewModel> unassignedProducts;
        public List<ProductViewModel> UnassignedProducts
        {
            get => unassignedProducts;
        }
        
        public MenuEditorViewModel()
        {
            
        }



        
    }
}