using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using FastTextNext.ViewModels;
using System;
using System.Diagnostics;

namespace FastTextNext.Views;

public partial class MainView : UserControl
{
    Avalonia.Media.IBrush? backgroundBrush;

    public MainView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;

        Loaded += MainView_Loaded;


        MainText.GotFocus += MainText_GotFocus;
        MainText.LostFocus += MainText_LostFocus;
        MainText.PointerEntered += MainText_PointerEntered;
        MainText.PointerReleased += MainText_PointerReleased;
        MainText.PointerCaptureLost += MainText_PointerCaptureLost;
        MainText.PointerMoved += MainText_PointerMoved;
    }

    private void MainText_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        //MainText.Background = Brushes.LightGray;
    }

    private void MainText_PointerCaptureLost(object? sender, Avalonia.Input.PointerCaptureLostEventArgs e)
    {
        //MainText.Background = Brushes.LightGray;
    }

    private void MainText_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        //MainText.Background = Brushes.LightGray;
    }

    private void MainText_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        //if (backgroundBrush != null)
          //  MainText.Background = backgroundBrush;
        //MainText.Background = Brushes.LightGray;
       // Debug.WriteLine("main text PointerEntered");
    }

    private void MainText_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //MainText.Background = Brushes.LightGray;
        /*if (backgroundBrush != null) 
            MainText.Background = backgroundBrush;*/
        //Debug.WriteLine("main text lost focus");
    }

    private void MainText_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
    {
       // MainText.Background = Brushes.LightGray;
        //backgroundBrush = MainText.Background;        
        //Debug.WriteLine("main text got focus");
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainText.Focus();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.SetFocusOnMainText += ViewModel_SetFocusOnMainText;
        }
    }

    private void ViewModel_SetFocusOnMainText()
    {
        MainText.Focus();
    }
}
