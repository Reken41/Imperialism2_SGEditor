using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Imperialism_2_SGEditor
{
  public class Country : ByteManaged
  {
    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; NotifyPropertyChanged("Name"); }
    }

    private byte _number;
    public byte Number
    {
      get { return _number; }
      set { _number = value; NotifyPropertyChanged("Number"); }
    }

    private bool _isEuropePower;
    public bool IsEuropePower
    {
      get { return _isEuropePower; }
      set { _isEuropePower = value; NotifyPropertyChanged("IsEuropePower"); }
    }

    private Brush _color;
    public Brush Color
    {
      get { return _color; }
      set { _color = value; NotifyPropertyChanged("Color"); }
    }
  }
}
