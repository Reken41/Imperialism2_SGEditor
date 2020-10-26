using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Imperialism_2_SGEditor
{
  public class MainWindowVM : ByteManaged
  {
    private readonly ICommand _loadSavedGameCommand;
    private readonly ICommand _saveSavedGameCommand;
    private readonly ICommand _revealMapForPlayerCommand;
    private readonly ICommand _revealResourcesForPlayerCommand;
    private readonly ICommand _hideMapForPlayerCommand;
    private long loadIdx = 0;
    private string fileNameAdnPath;
    private long mapKeyIdx = 0;
    private long mapFieldsStartIdx = 0;
    private long citiesStartIdx = 0;

    public ICommand LoadSavedGameCommand { get { return _loadSavedGameCommand; } }
    public ICommand SaveSavedGameCommand { get { return _saveSavedGameCommand; } }
    public ICommand RevealMapForPlayerCommand { get { return _revealMapForPlayerCommand; } }
    public ICommand RevealResourcesForPlayerCommand { get { return _revealResourcesForPlayerCommand; } }
    public ICommand HideMapForPlayerCommand { get { return _hideMapForPlayerCommand; } }

    public List<Country> WorldCountries { get; set; }
    public MapField[] WorldFields { get; set; }
    public List<City> Cities { get; set; }
    public ObservableCollection<MapField> RowItem { get; set; }

    private MapField _selectedMapField;
    public MapField SelectedMapField
    {
      get { return _selectedMapField; }
      set { _selectedMapField = value; NotifyPropertyChanged("SelectedMapField"); }
    }

    private string _img;
    public string Img
    {
      get { return _img; }
      set { _img = value; NotifyPropertyChanged("Img"); }
    }

    private string _fileName;
    public string FileName
    {
      get { return _fileName; }
      set { _fileName = value; NotifyPropertyChanged("FileName"); }
    }

    private string _mapKey;
    public string MapKey
    {
      get { return _mapKey; }
      set { _mapKey = value; NotifyPropertyChanged("MapKey"); }
    }

    private string _playerCountryName;
    public string PlayerCountryName
    {
      get { return _playerCountryName; }
      set { _playerCountryName = value; NotifyPropertyChanged("PlayerCountryName"); }
    }

    private byte _playerCountryNumber;
    public byte PlayerCountryNumber
    {
      get { return _playerCountryNumber; }
      set { _playerCountryNumber = value; NotifyPropertyChanged("PlayerCountryNumber"); }
    }

    private int _worldSize;
    public int WorldSize
    {
      get { return _worldSize; }
      set { _worldSize = value; NotifyPropertyChanged("WorldSize"); }
    }

    private int _citiesCount;
    public int CitiesCount
    {
      get { return _citiesCount; }
      set { _citiesCount = value; NotifyPropertyChanged("CitiesCount"); }
    }

    private Brush _playerColor;
    public Brush PlayerColor
    {
      get { return _playerColor; }
      set { _playerColor = value; NotifyPropertyChanged("PlayerColor"); }
    }

    public MainWindowVM()
    {
      _loadSavedGameCommand = new DelegateCommand(LoadSavedGame);
      _saveSavedGameCommand = new DelegateCommand(SaveSavedGame);
      _revealMapForPlayerCommand = new DelegateCommand<byte>(RevealMapForPlayer);
      _revealResourcesForPlayerCommand = new DelegateCommand<byte>(RevealResourcesForPlayer);
      _hideMapForPlayerCommand = new DelegateCommand<byte>(HideMapForPlayer);
      RowItem = new ObservableCollection<MapField>();
      NotifyPropertyChanged("RowItem");
    }

    private void RevealMapForPlayer(byte player)
    {
      foreach (var field in WorldFields)
      {
        if (player < 255)
        {
          if(field.RawData[DB.BUILDINGS]>0) continue;
          byte val = (byte)(255 - field.RawData[DB.MAP_VISIBILITY]);
          string binary = Convert.ToString(val, 2).PadLeft(6, '0');
          if (binary[5 - player] == '0') field.RawData[DB.MAP_VISIBILITY] -= (byte)Math.Pow(2, player);
        }
        else field.RawData[DB.MAP_VISIBILITY] = 0;
      }
      if (player < 255) Status = "Map revealing finished for player: " + DB.COUNTRY_NAMES[player];
      else Status = "Map revealed for all players!";
    }

    private void RevealResourcesForPlayer(byte player)
    {
      foreach (var field in WorldFields)
      {
        if (player < 255)
        {
          if (IsExplorable(field)) continue;
          if (HasPreciousResource(field)) continue;
          byte val = field.RawData[DB.RESOURCE_DISCOVERED_BY];
          string binary = Convert.ToString(val, 2).PadLeft(6, '0');
          if (binary[5 - player] == '0') field.RawData[DB.RESOURCE_DISCOVERED_BY] += (byte) Math.Pow(2, player);
        }
        else field.RawData[DB.RESOURCE_DISCOVERED_BY] = 255;
      }
      if (player < 255) Status = "Resource revealing finished for player: " + DB.COUNTRY_NAMES[player];
      else Status = "Resources revealed for all players!";
    }

    private static bool IsExplorable(MapField field)
    {
      return field.RawData[DB.LAND_TYPE_GFX] < 8 || field.RawData[DB.LAND_TYPE_GFX] > 11;
    }

    private static bool HasPreciousResource(MapField field)
    {
      return field.RawData[DB.LAND_TYPE_RES] >= 24 && field.RawData[DB.LAND_TYPE_RES] <= 27;
    }

    private void HideMapForPlayer(byte player)
    {
      foreach (var field in WorldFields)
      {
        if (player < 255)
        {
          byte val = (byte)(255 - field.RawData[DB.MAP_VISIBILITY]);
          string binary = Convert.ToString(val, 2).PadLeft(6, '0');
          if (binary[5 - player] == '1') field.RawData[DB.MAP_VISIBILITY] += (byte)Math.Pow(2, player);
        }
        else field.RawData[DB.MAP_VISIBILITY] = 255;
      }
      if (player < 255) Status = "Map hiding finished for player: " + DB.COUNTRY_NAMES[player];
      else Status = "Map hidden for all players!";
    }

    private void SaveSavedGame()
    {
      long idx = mapFieldsStartIdx;
      foreach (var field in WorldFields)
      {
        UpdateValueXByte(idx, field.RawData);
        idx += field.RawData.Length;
      }

      idx = citiesStartIdx;
      foreach (var city in Cities)
      {
        UpdateValueXByte(idx, city.RawData);
        idx += city.RawData.Length;
      }

      ByteArrayToFile(fileNameAdnPath, RawData);
      Status = "Save successful";
      StatusColor = Brushes.DarkGreen;
    }

    public void LoadSlot0()
    {
      fileNameAdnPath = @"E:\Gry\Imperialism II\Save\slot0.imp";
      FileToByteArray(fileNameAdnPath);
      FileName = fileNameAdnPath + " (" + GetSaveName() + ")";
      SetPlayerCountryData();
      SetWorldData();
      SkipUnknownSections();
      LoadWorldMap();
      LoadCities();
      StatusColor = Brushes.DarkGreen;
      Status = "Load successful";
      //ByteArrayToFile(fileNameAdnPath, RawData);
    }

    private void LoadSavedGame()
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.InitialDirectory = @"C:\Gry\Imperialism II\SAVE\";
      dialog.Filter = "Imperialism 2 saved game (*.imp)|*.imp|All files (*.*)|*.*";
      if (dialog.ShowDialog().Value)
      {
        FileToByteArray(dialog.FileName);
        if (VerifyHeader())
        {
          FileName = dialog.FileName + " (" + GetSaveName() + ")";
          fileNameAdnPath = dialog.FileName;
          SetPlayerCountryData();
          SetWorldData();
          SkipUnknownSections();
          LoadWorldMap();
          LoadCities();
          StatusColor = Brushes.DarkGreen;
          Status = "Load successful";
          //ByteArrayToFile(fileNameAdnPath, RawData);
        }
        else { Status = "Invalid file header! Edit at your own risk!"; StatusColor = Brushes.DarkRed; }
      }
    }

    bool VerifyHeader()
    {
      //header is: IBMa
      return GetValueString(0, 5) == "IBMAa";
      //return array[0] == 73 && array[1] == 66 && array[2] == 77 && array[3] == 65 && array[4] == 97;
    }

    string GetSaveName()
    {
      return GetValueString(12, 28, true);
    }

    void SetPlayerCountryData()
    {
      PlayerCountryNumber = RawData[6527];
      PlayerCountryName = GetValueString(6528, 12, true);
      PlayerColor = DB.COUNTRY_COLORS[PlayerCountryNumber];
    }

    void SetWorldData()
    {
      WorldCountries = new List<Country>();
      int idx = 6839;
      for (int i = 0; i < 22; i++)
      {
        Country country = new Country
        {
          Name = GetValueString(idx, RawData[idx - 1]),
          Color = i < 6 ? DB.COUNTRY_COLORS[i] : Brushes.Gray,
          Number = i < 6 ? (byte)i : DB.GetCountryNumber(GetValueString(idx, RawData[idx - 1])),
          IsEuropePower = i < 6
        };
        WorldCountries.Add(country);
        idx += RawData[idx - 1] + 1;
      }
      loadIdx = idx;
      NotifyPropertyChanged("WorldCountries");
    }

    void SkipUnknownSections()
    {
      loadIdx += 9 + 3040;

      while (true)
      {
        loadIdx++;
        if (RawData[loadIdx] == 1)
        {
          //10th sections end (or new unknown section starts) with 0x00 01 00 00 (65 536)
          if (RawData[loadIdx + 1] == 0 && RawData[loadIdx + 2] == 0 && RawData[loadIdx + 3] > 0) break;
        }
      }
      loadIdx += 968 * 4;
      loadIdx += 3000;
    }

    void LoadWorldMap()
    {
      while (true)
      {
        loadIdx++;
        if (RawData[loadIdx] == 26)
        {
          //world map data starts 14 bytes after 1A 03 00 0C 03
          if (RawData[loadIdx + 1] == 3 && RawData[loadIdx + 2] == 0 && RawData[loadIdx + 3] == 12 && RawData[loadIdx + 4] == 3)
          {
            mapKeyIdx = loadIdx + 20;
            MapKey = GetValueString(mapKeyIdx, RawData[loadIdx + 19]);
            loadIdx += 20 + RawData[loadIdx + 19];
            mapFieldsStartIdx = loadIdx;
            break;
            //Can we verify map key anyhow?
            //if (Regex.IsMatch(MapKey, @"^[a-zA-Z0-9]{3,29}$")) break;
          }
        }
      }

      WorldFields = new MapField[6480];
      MapField field;
      int fieldsInRowCount = 1;
      int currentRow = 1;
      int worldIndex = 0;

      while (true)
      {
        field = new MapField
        {
          RawData = GetXBytes(loadIdx, 40),
          Idx = (short)worldIndex
        };

        if (HasValue(field.RawData, 255) == false) { loadIdx++; break; }

        field.Row = currentRow;
        field.Col = fieldsInRowCount;
        field.ProvinceId = field.RawData[23] == 255 ? field.RawData[5] : field.RawData[23];

        if (worldIndex > 1)
        {
          if (WorldFields[worldIndex - 1].ProvinceId != field.ProvinceId)
          {
            Thickness newBorder = field.Border;
            newBorder.Left = 1;
            field.Border = newBorder;
          }

          if (worldIndex > 108)
          {
            if (field.Row % 2 == 0)
            {
              if (WorldFields[worldIndex - 108].ProvinceId != field.ProvinceId)
                field.TopLeftBorder = Visibility.Visible;

              if (WorldFields[worldIndex - 107].ProvinceId != field.ProvinceId)
                field.TopRightBorder = Visibility.Visible;
            }
            else
            {
              if (WorldFields[worldIndex - 108].ProvinceId != field.ProvinceId)
                field.TopRightBorder = Visibility.Visible;

              if (WorldFields[worldIndex - 109].ProvinceId != field.ProvinceId)
                field.TopLeftBorder = Visibility.Visible;
            }
          }
        }

        field.Update();
        WorldFields[worldIndex] = field;
        worldIndex++;
        fieldsInRowCount++;

        if (fieldsInRowCount == 109)
        {
          currentRow++;
          fieldsInRowCount = 1;
        }
        loadIdx += 40;
        field = null;
      }
      WorldSize = WorldFields.Length;
    }

    public void UpdateCityOwner(short position, byte newOwner)
    {
      foreach (var city in Cities)
      {
        if (city.Idx == position)
        {
          city.Owner = newOwner;
          break;
        }
      }
    }

    public string GetCityName(short provinceId)
    {
      foreach (var city in Cities)
        if (city.ProvinceId == provinceId) return city.Name;
      return null;
    }

    void LoadCities()
    {
      Cities = new List<City>();
      City city;
      int dataSize = 0;
      citiesStartIdx = loadIdx;
      while (true)
      {
        city = new City();
        dataSize = 209 + RawData[loadIdx + 208];
        city.RawData = GetXBytes(loadIdx, dataSize);
        city.Fill();
        foreach (var field in WorldFields)
          if (city.Idx == field.Idx) city.ProvinceId = field.ProvinceId;

        Cities.Add(city);
        loadIdx += dataSize;
        if (RawData[loadIdx] == 255 && RawData[loadIdx + 1] == 255) { break; }
        city = null;
      }

      foreach (var field in WorldFields)
      {
        if (field.IsSea == false) field.ProvinceName = GetCityName(field.ProvinceId);
      }

      CitiesCount = Cities.Count;
      NotifyPropertyChanged("Cities");
      NotifyPropertyChanged("WorldFields");
    }
  }
  /*
   * FILE STRUCTURE:
   * 0-4        - header (IBMAa)
   * 5-11       - empty (00)
   * 12-28      - Save game name (from 12 to first 00)
   * 29-43      - empty (00)
   * 44-6526    - unknown ??? but static
   * 6527       - human player country number (0 blue France, 1 red England, 2 teal Sweden, 3 orange Holland, 4 green Portugal, 5 yellow Spain)
   * 6528- (max 6539)  - User country name (max 12 bytes to first 00)
   * ??? - gap with unknown values block of the same size but country name has different size and there are some data after shorter names
   * 6838 - world all countries list (structure: [number of chars (1 byte)][name])
   * last country name idx + 9 bytes - unknown ???
   * previous idx + 3040 bytes - unknown 20 sections for 152 bytes
   * prev idx + 10th sections -> unknown 25 sections that starts with 0x10 (16) different length
   * new unknown section starts with 0x01 00 00 has 968 bytes
   * next unknown section has 968 bytes (mostly 0x04 00 04 00)
   * next unknown section has 968 bytes (mostly 0xFF)
   * next unknown section has 968 bytes (mostly 0x00 or 0x02 or 0x01)
   * next part of the file is irregular, so we search for 1A 03 00 0C 03 pattern witch seems always 14 bytes from world map data (15 bytes after 1A 03 00 0C 03) - we can skip about 3000 bytes to speed up process cos there is no way that world map data will start earlier
   * world map fields are constructed this way:
   *    length: 40 bytes per field (6480 fields, 109 by row)
   *    5 and 6 bytes are field OWNER
   *    24 byte field ID - 255 - WATER PROVICES! other numbers are land province id
   *    23 byte - field type (12 - tundra, 15 - scrub forest)
   *    22 byte - field type (4 - barren hills)
   *    32 byte is map exploration (FF unexplored, minus player number shows that field as explored for player)
   *    map data ends with 0x00 
   * cities data starts immediately after map data 0x00 end and it is constructed as follows:
   *    length: 209 + city name (diff size, given in one byte before name) (cities count differs for each map)
   */
}
