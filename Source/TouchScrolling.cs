using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imperialism_2_SGEditor
{
  public class TouchScrolling : DependencyObject
  {
    public static bool GetIsEnabled(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(IsEnabledProperty, value);
    }

    public bool IsEnabled
    {
      get { return (bool)GetValue(IsEnabledProperty); }
      set { SetValue(IsEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(TouchScrolling), new UIPropertyMetadata(false, IsEnabledChanged));

    static Dictionary<object, MouseCapture> _captures = new Dictionary<object, MouseCapture>();

    static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var target = d as ScrollViewer;
      if (target == null) return;

      if ((bool)e.NewValue)
      {
        target.Loaded += Target_Loaded;
      }
      else
      {
        Target_Unloaded(target, new RoutedEventArgs());
      }
    }

    static void Target_Unloaded(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Debug.WriteLine("Target Unloaded");

      var target = sender as ScrollViewer;
      if (target == null) return;

      _captures.Remove(sender);

      //target.Loaded -= Target_Loaded;
      target.Unloaded -= Target_Unloaded;
      target.PreviewMouseRightButtonDown -= Target_PreviewMouseRightButtonDown;
      target.PreviewMouseMove -= Target_PreviewMouseMove;

      target.PreviewMouseRightButtonUp -= Target_PreviewMouseRightButtonUp;
    }

    static void Target_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var target = sender as ScrollViewer;
      if (target == null) return;

      _captures[sender] = new MouseCapture
      {
        VerticalOffset = target.VerticalOffset,
        HorizontalOffset = target.HorizontalOffset,
        Point = e.GetPosition(target),
      };
    }

    static void Target_Loaded(object sender, RoutedEventArgs e)
    {
      var target = sender as ScrollViewer;
      if (target == null) return;

      System.Diagnostics.Debug.WriteLine("Target Loaded");

      target.Unloaded += Target_Unloaded;
      target.PreviewMouseRightButtonDown += Target_PreviewMouseRightButtonDown;
      target.PreviewMouseMove += Target_PreviewMouseMove;

      target.PreviewMouseRightButtonUp += Target_PreviewMouseRightButtonUp;
    }

    static void Target_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      var target = sender as ScrollViewer;
      if (target == null) return;

      target.ReleaseMouseCapture();
    }

    static void Target_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (!_captures.ContainsKey(sender)) return;

      if (e.RightButton != MouseButtonState.Pressed)
      {
        _captures.Remove(sender);
        return;
      }

      var target = sender as ScrollViewer;
      if (target == null) return;

      var capture = _captures[sender];

      var point = e.GetPosition(target);

      var dy = point.Y - capture.Point.Y;
      var dx = point.X - capture.Point.X;

      if (Math.Abs(dy) > 5)
      {
        target.CaptureMouse();
      }

      target.ScrollToVerticalOffset(capture.VerticalOffset - dy);
      target.ScrollToHorizontalOffset(capture.HorizontalOffset - dx);
    }

    internal class MouseCapture
    {
      public double VerticalOffset { get; set; }
      public double HorizontalOffset { get; set; }
      public Point Point { get; set; }
    }
  }
}
