using System.Collections.ObjectModel;

namespace TSD.Model.Setting
{
    public static class SettingTSD
    {
        public static ObservableCollection<Device> Devices => new ObservableCollection<Device>() 
        {
            new Device { Name = "По умолчанию", TypeDevice = TypeDevice.Manual},
            new Device { Name = "Атол Smart.Slim Plus", TypeDevice = TypeDevice.Atol },
            new Device { Name = "Urovo RT40", TypeDevice = TypeDevice.Urovo },
            new Device { Name = "Honeywell EDA50K", TypeDevice = TypeDevice.Honeywell }
        };

        public static ObservableCollection<TypeTransfer> TypeTransfers => new ObservableCollection<TypeTransfer>()
        {
            new TypeTransfer { Name = "USB", IsNetwork = false},
            new TypeTransfer { Name = "REST API", IsNetwork = true },
        };

        public static int SelectTypeTransfer { get; set; }

        public static int SelectDevice { get; set; }
    }
}
