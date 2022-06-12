using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class CategoryViewModel : ViewModelBase
    {
        private Category category;

        public CategoryViewModel(Category category)
        {
            this.category = category;
        }
    }
}
