namespace ERP_Admin_Panel.Services.Modal
{
    public class ModalResult
    {
        public object Data { get; }
        public Type? DateType { get; }
        public bool Cancelled { get; }
        public Type? ModalType { get; set; }

        protected ModalResult(object data, Type resultType, bool cancelled, Type modalType = null)
        {
            Data = data;
            DateType = resultType;
            Cancelled = cancelled;
            ModalType = modalType;
        }

        public static ModalResult Ok<T>(T result) => Ok(result, default);
        public static ModalResult Ok<T>(T result, Type? modalType) => new(result, typeof(T), false, modalType);
        public static ModalResult Cancel() => Cancel(default);
        public static ModalResult Cancel(Type? modalType) => new(default, typeof(object), true, modalType);

    }
}
