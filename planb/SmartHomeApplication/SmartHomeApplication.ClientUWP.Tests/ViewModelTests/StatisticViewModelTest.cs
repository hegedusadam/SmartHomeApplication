using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeApplication.ClientUWP.Model;
using SmartHomeApplication.ClientUWP.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeApplication.ClientUWP.Tests.ViewModelTests
{
    [TestClass]
    public class StatisticViewModelTest
    {

        [TestMethod]
        public void GetMinutesOn_GivenTwoChange_CalculatesTimeOn()
        {

            Change changeOn = new Change()
            {
                state = true,
                date = DateTime.Now
            };


            Change changeOff = new Change()
            {
                state = false,
                date = DateTime.Now.Add(new TimeSpan(1000))
            };

            ObservableCollection<Change> changes = new ObservableCollection<Change>();
            changes.Add(changeOn);
            changes.Add(changeOff);

            StatisticViewModel.GetMinutesOn(changes);

            Assert.AreEqual(changes.Count, 2);
            Assert.AreEqual(changeOff.timeOn, new TimeSpan(1000));
        }

        [TestMethod]
        public void GetMinutesOn_GivenEmptyList_DoesNothing()
        {
            ObservableCollection<Change> changes = new ObservableCollection<Change>();

            StatisticViewModel.GetMinutesOn(changes);

            Assert.AreEqual(changes.Count, 0);
        }

        [TestMethod]
        public void GetTotalTime_GivenEmptyList_ReturnsZero()
        {
            ObservableCollection<Change> changes = new ObservableCollection<Change>();
            StatisticViewModel.GetMinutesOn(changes);

            TimeSpan time = StatisticViewModel.GetTotalTimeOn(changes);

            Assert.AreEqual(time.Milliseconds, 0);
        }

        [TestMethod]
        public void GetTotalTime_GivenTwoChanges_ReturnsOneSecond()
        {
            Change changeOn = new Change()
            {
                state = true,
                date = DateTime.Now
            };


            Change changeOff = new Change()
            {
                state = false,
                date = DateTime.Now.Add(new TimeSpan(0, 0, 1))
            };

            ObservableCollection<Change> changes = new ObservableCollection<Change>();
            changes.Add(changeOn);
            changes.Add(changeOff);

            StatisticViewModel.GetMinutesOn(changes);

            TimeSpan time = StatisticViewModel.GetTotalTimeOn(changes);

            Assert.AreEqual(1, time.Seconds);
        }
    }
}
