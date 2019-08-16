﻿using System.Collections.ObjectModel;
using Application.BalancedFieldLength.Controls;
using NUnit.Framework;

namespace Application.BalancedFieldLength.Test
{
    [TestFixture]
    public class MainViewModelTest
    {
        [Test]
        public static void Constructor_ExpectedValues()
        {
            // Call
            var mainViewModel = new MainViewModel();

            // Assert
            TabControlViewModel tabControlViewModel = mainViewModel.TabControlViewModel;
            ObservableCollection<ITabViewModel> tabs = tabControlViewModel.Tabs;
            Assert.That(tabs, Has.Count.EqualTo(1));
            Assert.That(tabs[0], Is.TypeOf<GeneralSimulationSettingsTabViewModel>());
            Assert.That(tabControlViewModel.SelectedTabItem, Is.SameAs(tabs[0]));
        }
    }
}