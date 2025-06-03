using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.ViewModels
{
    public class BubbleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BubbleViewModel()
        {

        }

    }
}
