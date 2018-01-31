using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy.EventRelatedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromotionPage : ContentPage
    {
        public PromotionPage()
        {
            InitializeComponent();
        }

        public PromotionPage(Models.BjjEvent Promotion)
        {
            InitializeComponent();

        }
    }
}