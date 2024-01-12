namespace ERP_Admin_Panel.Services.Modal
{
    public class ModalParameters
    {
        private readonly Dictionary<string, object> _parameters;

        public ModalParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        public void Add(string parameterName, object value) => _parameters.Add(parameterName, value);

        public T Get<T>(string parameterName)
        {
            if (!_parameters.ContainsKey(parameterName))
            {
                throw new KeyNotFoundException("Такого ключа не существует");
            }

            return (T)_parameters[parameterName];
        }

        public T? TryGet<T> (string parameterName)
        {
            if (_parameters.ContainsKey(parameterName))
            {
                return (T)_parameters[parameterName];
            }

            return default;
        }
    }
}
