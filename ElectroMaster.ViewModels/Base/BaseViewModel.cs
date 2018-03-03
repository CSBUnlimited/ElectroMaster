using System.ComponentModel;

namespace ElectroMaster.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region IDataErrorInfo

        string IDataErrorInfo.this[string propertyName] => null;

        string IDataErrorInfo.Error => null;

        #endregion

        #region Validations

        public string[] ValidationProperties { get; protected set; }

        public virtual bool IsValid
        {
            get
            {
                foreach (string property in ValidationProperties)
                {
                    if (GetValidationError(property) != null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public virtual string GetValidationError(string propertyName) => null;

        #endregion
    }
}
