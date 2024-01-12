using TSD.ViewModels;

namespace TSD.Model.Setting
{
    public class TypeTransfer : ViewModelBase
    {
        private string _name = string.Empty;
        private bool _isNetwork = false;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsNetwork
        {
            get => _isNetwork;
            set => SetProperty(ref _isNetwork, value);
        }
    }
}
