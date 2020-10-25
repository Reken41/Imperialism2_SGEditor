using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Imperialism_2_SGEditor
{
  public class Resource : INotifyPropertyChanged
  {
    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; NotifyPropertyChanged("Name"); }
    }

    private byte _id;
    public byte Id
    {
      get { return _id; }
      set { _id = value; NotifyPropertyChanged("Id"); }
    }

    private string _img;
    public string Image
    {
      get { return _img; }
      set { _img = value; NotifyPropertyChanged("Image"); }
    }
            
    public Resource(string name, string image, byte id)
    {
      Name = name;
      Image = "Resources/" + image;
      Id = id;
    }

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
