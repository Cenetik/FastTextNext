using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FastTextNext.ViewModels;
using System;

namespace FastTextNext.Views;

public partial class ListTextWindow : Window
{
    public ListTextWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ListTextModel viewModel)
        {
            viewModel.CloseThisView += ViewModel_CloseThisView;
        }
    }

    private void ViewModel_CloseThisView()
    {
        this.Close();
    }
}