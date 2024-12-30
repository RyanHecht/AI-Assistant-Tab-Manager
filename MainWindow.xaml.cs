using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Linq;

namespace AIAssistantPanes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try {
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AIAssistantPanes"
                );
                var options = new CoreWebView2EnvironmentOptions();
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            }
            catch (Exception ex) {
                MessageBox.Show($"WebView2 Environment Error: {ex.Message}");
            }
        }

        private void AddClaudeButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("Claude", "https://claude.ai/new");
        }

        private void AddChatGPTButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("ChatGPT", "https://chat.openai.com");
        }

        private void AddCopilotButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("GitHub Copilot", "https://github.com/copilot");
        }

    private async void AddNewTab(string title, string url)
    {
        try 
        {
            var webView = new Microsoft.Web.WebView2.Wpf.WebView2();
            webView.HorizontalAlignment = HorizontalAlignment.Stretch;
            webView.VerticalAlignment = VerticalAlignment.Stretch;

            var tabItem = new TabItem { Header = title, Content = webView };
            
            var contextMenu = new ContextMenu();
            var renameMenuItem = new MenuItem { Header = "Rename Tab" };
            renameMenuItem.Click += (s, e) => RenameTab(tabItem);
            contextMenu.Items.Add(renameMenuItem);

            var closeMenuItem = new MenuItem { Header = "Close Tab" };
            closeMenuItem.Click += (s, e) => CloseTab(tabItem);
            contextMenu.Items.Add(closeMenuItem);

            var closeOthersMenuItem = new MenuItem { Header = "Close Other Tabs" };
            closeOthersMenuItem.Click += (s, e) => CloseOtherTabs(tabItem);
            contextMenu.Items.Add(closeOthersMenuItem);

            var closeAllMenuItem = new MenuItem { Header = "Close All Tabs" };
            closeAllMenuItem.Click += (s, e) => CloseAllTabs();
            contextMenu.Items.Add(closeAllMenuItem);

            tabItem.ContextMenu = contextMenu;

            AssistantTabs.Items.Add(tabItem);
            AssistantTabs.SelectedItem = tabItem;

            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            webView.CoreWebView2.Navigate(url);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void RenameTab(TabItem tab)
    {
        var input = new Window 
        {
            Title = "Rename Tab",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this
        };

        var panel = new StackPanel { Margin = new Thickness(10) };
        var textBox = new TextBox { Text = tab.Header.ToString(), Margin = new Thickness(0, 0, 0, 10) };
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        
        var okButton = new Button { Content = "OK", Width = 60, Margin = new Thickness(0, 0, 10, 0) };
        okButton.Click += (s, e) => { tab.Header = textBox.Text; input.DialogResult = true; };
        
        var cancelButton = new Button { Content = "Cancel", Width = 60 };
        cancelButton.Click += (s, e) => input.DialogResult = false;

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        panel.Children.Add(textBox);
        panel.Children.Add(buttonPanel);
        input.Content = panel;

        input.ShowDialog();
    }

    private void CloseTab(TabItem tab)
    {
        AssistantTabs.Items.Remove(tab);
    }

    private void CloseOtherTabs(TabItem tab)
    {
        var tabsToClose = AssistantTabs.Items.Cast<TabItem>().Where(t => t != tab).ToList();
        foreach (var t in tabsToClose)
        {
            AssistantTabs.Items.Remove(t);
        }
    }

    private void CloseAllTabs()
    {
        AssistantTabs.Items.Clear();
    }


    private void AssistantTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AssistantTabs.SelectedItem is TabItem selectedTab)
        {
            selectedTab.Focus();
        }
    }
    }
}
