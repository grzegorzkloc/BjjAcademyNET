using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.Models
{
    [Table("TrainingPlans")]
    class TrainingPlan : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private string _name;
        private ObservableCollection<string> _trainingActivities;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public String Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> GetTrainingActivities()
        { return _trainingActivities; }

        public void SetTrainingActivities(ObservableCollection<string> value)
        {
            if (value != null)
            {
                _trainingActivities = value;
                OnPropertyChanged();
            }
        }

        public void AddTrainingActivity(string TrainingActivity)
        {
            _trainingActivities.Add(TrainingActivity);
        }

        public TrainingPlan()
        {
            _trainingActivities = new ObservableCollection<string>();
        }
    }
}
