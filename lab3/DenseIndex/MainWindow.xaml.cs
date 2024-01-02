using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DenseIndex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileManager fm = new FileManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int key;
                if (!int.TryParse(textBoxSearch.Text, out key))
                    throw new Exception("The key should be integer.");
                
                string value = fm.Search(key);
                if (value != null)
                {
                    resultsTextBlock.Text = $"The value by key {key}: {value}";
                }
                else
                {
                    resultsTextBlock.Text = $"The value by key {key} not found";
                }
                resultsTextBlock.Text += $"\nNumber of comparisons: {fm.comparisons}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Search error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = (int)slValue.Value;
                resultsTextBlock.Text = "Generating data...";

                string result = await Task.Run(() => fm.GenerateData(size));

                resultsTextBlock.Text = $"New data generated\n{result}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Generate error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = textBoxAdd.Text;
                if (value.Contains(' '))
                    throw new Exception("The value should not contain empty spaces");
                if (value.Length == 0)
                    throw new Exception("The value should not be empty");
                fm.Add(value);
                resultsTextBlock.Text = $"Element with value {value} is added";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Add error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int key;
                if (!int.TryParse(textBoxDelete.Text, out key))
                    throw new Exception("The key should be integer.");
                if (fm.Delete(key))
                {
                    resultsTextBlock.Text = $"Deleted value with key {key}";
                }
                else
                {
                    resultsTextBlock.Text = $"Value by key {key} not found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Delete error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int key;
                if (!int.TryParse(textBoxEditKey.Text, out key))
                    throw new Exception("The key should be integer.");
                string value = textBoxEditValue.Text;
                if (value.Contains(' '))
                    throw new Exception("The value should not contain empty spaces");
                if (value.Length == 0)
                    throw new Exception("The value should not be empty");
                if (fm.Edit(key, value))
                {
                    resultsTextBlock.Text = $"Edited value with key {key}. New value is {value}";
                }
                else
                {
                    resultsTextBlock.Text = $"Value by key {key} not found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Edit error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
