using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Imperialism_2_SGEditor
{
  public class MapField : ByteManaged
  {
    public Double Shift { get { if (Row % 2 != 0 && Col == 1) return 18; else return 36; } }
    public int Row { get; set; }
    public int Col { get; set; }
    public short Idx { get; set; }
    public string Pos
    {
      get
      {
        byte[] intBytes = BitConverter.GetBytes(Idx);
        Array.Reverse(intBytes);
        return "Pos1: " + intBytes[0] + "  Pos2: " + intBytes[1];
      }
    }
    public string ProvinceName { get; set; }
    public bool IsSea
    {
      get
      {
        return RawData[DB.PROVINCE_ID] == DB.SEA;
      }
    }

    public byte Owner
    {
      get { return RawData[DB.OWNER1]; }
      set { RawData[DB.OWNER1] = RawData[DB.OWNER2] = value; UpdateOwner(); NotifyPropertyChanged("Owner"); }
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get { return _isSelected; }
      set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
    }

    private string _img;
    public string Img
    {
      get { return _img; }
      set { _img = value; NotifyPropertyChanged("Img"); }
    }

    private string _imgResource;
    public string ImgResource
    {
      get { return _imgResource; }
      set { _imgResource = value; NotifyPropertyChanged("ImgResource"); }
    }

    private Brush _ownerColor;
    public Brush OwnerColor
    {
      get { return _ownerColor; }
      set { _ownerColor = value; NotifyPropertyChanged("OwnerColor"); }
    }

    private short _provinceId;
    public short ProvinceId
    {
      get { return _provinceId; }
      set { _provinceId = value; NotifyPropertyChanged("Id"); }
    }

    private Visibility _resourceIconVis;
    public Visibility ResourceIconVis
    {
      get { return _resourceIconVis; }
      set { _resourceIconVis = value; NotifyPropertyChanged("ResourceIconVis"); }
    }

    public Thickness Border { get; set; }
    public Visibility TopLeftBorder { get; set; }
    public Visibility TopRightBorder { get; set; }

    public MapField()
    {
      Border = new Thickness();
      TopLeftBorder = Visibility.Collapsed;
      TopRightBorder = Visibility.Collapsed;
      ResourceIconVis = Visibility.Collapsed;
    }

    protected override void AfterRawDataUpdate()
    {
      base.AfterRawDataUpdate();
      Update();
    }

    public void Update()
    {
      UpdateImg();
      UpdateOwner();
    }

    public void UpdateRawData()
    {

    }

    void UpdateOwner()
    {
      //Owner = RawData[DB.OWNER1] < 6 ? RawData[DB.OWNER2] : DB.SEA;
      if (Owner < 6) OwnerColor = DB.COUNTRY_COLORS[Owner].Clone();
      else OwnerColor = null;
    }

    void UpdateImg()
    {
      if (IsSea) Img = "Resources/sea.jpg";
      else
      {
        var resource = DB.GetLandTypesById(RawData[DB.LAND_TYPE_GFX]);
        if (resource != null) Img = resource.Image;
        else Img = "Resources/plains.jpg";
      }

      ImgResource = "Resources/rnone.png";
      if (RawData[DB.LAND_TYPE_RES] != 255)
      {
        var resource = DB.GetResourceById(RawData[DB.LAND_TYPE_RES]);
        if (resource != null)
        {
          ResourceIconVis = Visibility.Visible;
          ImgResource = resource.Image;
        }
      }
    }
  }
}
