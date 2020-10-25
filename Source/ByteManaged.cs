using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace Imperialism_2_SGEditor
{
  public abstract class ByteManaged : INotifyPropertyChanged
  {
    private byte[] _rawData;

    public byte[] RawData
    {
      get { return _rawData; }
      set { _rawData = value; AfterRawDataUpdate(); NotifyPropertyChanged("RawData"); }
    }

    protected bool isOpening;

    private string _status = "Click open to load saved game...";

    public string Status
    {
      get { return _status; }
      set { if (_status != value) { _status = value; NotifyPropertyChanged("Status"); } }
    }

    private Brush _statusColor;

    public Brush StatusColor
    {
      get { return _statusColor; }
      set { _statusColor = value; NotifyPropertyChanged("StatusColor"); }
    }

    protected virtual void AfterRawDataUpdate()
    {

    }

    protected void UpdateValue(int value, int index)
    {
      if (isOpening == false)
      {
        var byteValue = BitConverter.GetBytes(value);
        byteValue.CopyTo(RawData, index);
      }
    }

    protected void UpdateValue(byte value, int index)
    {
      if (isOpening == false)
        RawData[index] = value;
    }

    protected void UpdateValue(short value, int index)
    {
      if (isOpening == false)
      {
        var byteValue = BitConverter.GetBytes(value);
        byteValue.CopyTo(RawData, index);
      }
    }

    protected void UpdateValue(bool value, int index)
    {
      if (isOpening == false)
      {
        var byteValue = BitConverter.GetBytes(value);
        byteValue.CopyTo(RawData, index);
      }
    }

    protected void UpdateValue3xByte(string value, int index)
    {
      if (isOpening == false)
      {
        RawData[index] = Convert.ToByte(value[0].ToString());
        RawData[index + 1] = Convert.ToByte(value[1].ToString());
        RawData[index + 2] = Convert.ToByte(value[2].ToString());
      }
    }

    protected void UpdateValueXByte(long index, byte[] array)
    {
      if (isOpening == false)
      {
        Array.Copy(array, 0, RawData, index, array.Length);
      }
    }

    protected int GetValueInt(int index)
    {
      return BitConverter.ToInt32(RawData, index);
    }

    protected string GetValue3xByte(int index)
    {
      return RawData[index].ToString() + RawData[index + 1].ToString() + RawData[index + 2].ToString();
    }

    protected short GetValueShort(int index)
    {
      return BitConverter.ToInt16(RawData, index);
    }

    protected bool GetValueBool(int index)
    {
      return BitConverter.ToBoolean(RawData, index);
    }

    protected byte GetValueByte(int index)
    {
      return RawData[index];
    }

    protected byte[] GetXBytes(long index, int length)
    {
      byte[] partArray = new byte[length];
      Array.Copy(RawData, index, partArray, 0, length);
      return partArray;
    }

    protected bool HasValue(byte[] arr, byte value)
    {
      return Array.IndexOf(arr, value) > -1;
    }

    protected string GetValueString(long index, int length, bool breakOnFirstZeroValue = false)
    {
      //BitConverter.ToString(partArray) to hex
      byte[] partArray = new byte[length];
      Array.Copy(RawData, index, partArray, 0, length);

      if (breakOnFirstZeroValue)
      {
        int breakIdx = Array.IndexOf(partArray, (byte)0);
        if (breakIdx > 0)
        {
          partArray = new byte[breakIdx];
          Array.Copy(RawData, index, partArray, 0, breakIdx);
        }
      }

      return Encoding.UTF8.GetString(partArray);
    }

    protected bool ByteArrayToFile(string fileName, byte[] byteArray)
    {
      try
      {
        FileStream _FileStream = new FileStream(fileName, FileMode.Create);
        _FileStream.Write(byteArray, 0, byteArray.Length);
        _FileStream.Close();
        return true;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    protected void FileToByteArray(string fileName)
    {
      RawData = File.ReadAllBytes(fileName);
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
      //if (propertyName != "Status" && propertyName != "StatusColor")
      //{
      //  Status = "Modified";
      //  StatusColor = Brushes.Orange;
      //}

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      //DB.NotifyCounter++;
    }

    #endregion
  }
}
