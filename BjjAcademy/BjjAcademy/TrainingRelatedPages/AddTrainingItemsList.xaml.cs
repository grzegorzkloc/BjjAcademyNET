using BjjAcademy.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BjjAcademy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTrainingItemsList : ContentPage
    {
        private TrainingPlan _trainingPlan;
        private ObservableCollection<Editor> _trainingItems;

        public AddTrainingItemsList()
        {
            _trainingPlan = new TrainingPlan();

            this.Entries = new StackLayout();
            _trainingItems = new ObservableCollection<Editor>();
            var editor = new Editor();
            _trainingItems.Add(editor);
            this.Entries.Children.Add(editor);
            InitializeComponent();
        }

        private void AddTrainingItem_Activated(object sender, EventArgs e)
        {
            Editor editor = new Editor()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 100
            };
            _trainingItems.Add(editor);
            this.Entries.Children.Add(editor);
        }

        private void DeleteTrainingItem_Activated(object sender, EventArgs e)
        {
            if (_trainingItems.Count == 0) return;
            this.Entries.Children.Remove(_trainingItems.Last<Editor>());
            _trainingItems.RemoveAt(_trainingItems.Count - 1);
        }
    }
}