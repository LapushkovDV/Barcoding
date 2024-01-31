using TSD.ViewModels;

namespace TSD.Model
{
    public class ElementPosition : ViewModelBase
    {
        private DirectionEnum _direction;
        private AbstractColumns _currentColumns;
        
        public DirectionEnum Direction
        {
            get => _direction;
            set => SetProperty(ref _direction, value);
        }

        public AbstractColumns CurrentColumns
        {
            get => _currentColumns;
            set => SetProperty(ref _currentColumns, value);
        }
    }
}
