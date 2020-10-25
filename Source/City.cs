using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imperialism_2_SGEditor
{
  public class City : ByteManaged
  {
    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; NotifyPropertyChanged("Name"); }
    }

    public byte Fortlevel
    {
      get { return RawData[DB.FORT_LEVEL]; }
      set { RawData[DB.FORT_LEVEL] = value; NotifyPropertyChanged("Fortlevel"); }
    }

    public byte Owner
    {
      get { return RawData[DB.CITY_OWNER1]; }
      set { RawData[DB.CITY_OWNER1] = RawData[DB.CITY_OWNER2] = value; NotifyPropertyChanged("Owner"); }
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set { _isExpanded = value; NotifyPropertyChanged("IsExpanded"); }
    }

    public short Idx
    {
      get { return GetValueShort(DB.CITY_POS1); }
      set { UpdateValue(value, DB.CITY_POS1); NotifyPropertyChanged("Idx"); }
    }

    private short _provinceId;
    public short ProvinceId
    {
      get { return _provinceId; }
      set { _provinceId = value; NotifyPropertyChanged("ProvinceId"); }
    }
    
    public List<byte[]> RowItem { get; set; }

    internal void Fill()
    {
      if (RawData != null && RawData.Length > 0)
      {
        int nameLength = RawData[208];
        Name = GetValueString(209, nameLength);
        IsExpanded = RawData[DB.IS_EXPANDED] == 1;
        RowItem = new List<byte[]>();
        RowItem.Add(RawData);
        NotifyPropertyChanged("RowItem");
      }
    }
  }
}
