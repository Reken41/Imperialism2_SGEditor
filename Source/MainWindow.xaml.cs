using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Imperialism_2_SGEditor
{
  public partial class MainWindow : Window
  {
    public MainWindowVM VM { get; set; }


    public MainWindow()
    {
      InitializeComponent();
      VM = new MainWindowVM();
      DataContext = VM;
    }

    private void UniformGrid_Loaded(object sender, RoutedEventArgs e)
    {
      //VM.LoadSlot0();
    }

    private void MapFieldItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      //Title = DB.NotifyCounter.ToString();
      //DB.NotifyCounter = 0;
      //if (VM.SelectedMapField != null)
      //  VM.SelectedMapField.Update();
      Border border = (Border)sender;
      MapField field = (MapField)border.Tag;

      if (AutoBtnChkBx.IsChecked.Value && AutoBtnList.SelectedItem != null)
      {
        Country country = (Country)AutoBtnList.SelectedItem;
        if (AutoFieldBtnChkBx.IsChecked.Value)
        {
          field.Owner = country.Number;
          VM.UpdateCityOwner(field.Idx, country.Number);
          field.Update();
        }
        else
        {
          foreach (var fieldTemp in VM.WorldFields)
          {
            if (fieldTemp.ProvinceId == field.ProvinceId && fieldTemp.IsSea == field.IsSea)
            {
              fieldTemp.Owner = country.Number;
              if (fieldTemp.IsSea == false) VM.UpdateCityOwner(fieldTemp.Idx, country.Number);
              fieldTemp.Update();
            }
          }
        }
      }
      else
      {
        if (VM.SelectedMapField != null) VM.SelectedMapField.IsSelected = false;
        VM.SelectedMapField = field;
        //VM.SelectedMapField.RawData[22] = 5;
        //VM.SelectedMapField.Update();

        VM.RowItem.Clear();
        VM.RowItem.Add(VM.SelectedMapField);

        if (FieldDetailsDataGrid.Columns.Count == 0)
        {
          int columnIndex = 0;
          foreach (var data in VM.SelectedMapField.RawData)
          {

            FieldDetailsDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                  Header = DB.HEADERS[columnIndex],
                  MinWidth = 30,
                  Binding = new Binding(string.Format("RawData[{0}]", columnIndex))
                  {
                    NotifyOnTargetUpdated = true
                  }
                });
            columnIndex++;
          }
        }
        //FieldDetailsDataGrid.ItemsSource = VM.RowItem;
        VM.SelectedMapField.IsSelected = true;
      }
    }

    private void FieldDetailsDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
    {
      if (VM.SelectedMapField != null)
        VM.SelectedMapField.Update();
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (VM.SelectedMapField != null)
      {
        VM.SelectedMapField.Update();
        VM.RowItem.Clear();
        VM.RowItem.Add(VM.SelectedMapField);
      }
    }
  }
}
