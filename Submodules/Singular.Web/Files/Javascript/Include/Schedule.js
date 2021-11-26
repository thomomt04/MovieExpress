


var ServiceHelper = (function () {
  var self = this;

  self.getScheduleInfo = function(schedule, scheduleType) {

    if (scheduleType == 2) {
      return 'N/A';
    }

    var ret = "";

    if (schedule) {

      //Occurs String
      if (schedule.OccursType() == 0) {
        //Daily
        ret = ret.concat("Every");
        ret = ret.concat(schedule.OccursDaily().DayInterval() == 1 ? "" : " ");
        ret = ret.concat(schedule.OccursDaily().DayInterval() == 1 ? "" : schedule.OccursDaily().DayInterval());
        ret = ret.concat(" Day");
        ret = ret.concat(schedule.OccursDaily().DayInterval() > 1 ? "s" : "");
      }
      if (schedule.OccursType() == 1) {
        // Weekly
        ret = ret.concat("Every");
        ret = ret.concat(schedule.OccursWeekly().WeekInterval() == 1 ? "" : " ");
        ret = ret.concat(schedule.OccursWeekly().WeekInterval() == 1 ? "" : schedule.OccursWeekly().WeekInterval());
        ret = ret.concat(" Week");
        ret = ret.concat(schedule.OccursWeekly().WeekInterval() > 1 ? "s" : "");
        ret = ret.concat(" on ");
        if (schedule.OccursWeekly().Monday() == true) {
          ret = ret.concat("Monday, ");
        }
        if (schedule.OccursWeekly().Tuesday() == true) {
          ret = ret.concat("Tuesday, ");
        }
        if (schedule.OccursWeekly().Wednesday() == true) {
          ret = ret.concat("Wednesday, ");
        }
        if (schedule.OccursWeekly().Thursday() == true) {
          ret = ret.concat("Thursday, ");
        }
        if (schedule.OccursWeekly().Friday() == true) {
          ret = ret.concat("Friday, ");
        }
        if (schedule.OccursWeekly().Saturday() == true) {
          ret = ret.concat("Saturday, ");
        }
        if (schedule.OccursWeekly().Sunday() == true) {
          ret = ret.concat("Sunday, ");
        }
      }
      if (schedule.OccursType() == 2) {
        if (schedule.OccursMonthlyType() == 0) {

          ret = ret.concat("on day ");
          ret = ret.concat(schedule.OccursMonthlyDay().Day());
          ret = ret.concat(" of ");
          ret = ret.concat(schedule.OccursMonthlyDay().MonthInterval() > 1 ? "every " : "each");
          ret = ret.concat(schedule.OccursMonthlyDay().MonthInterval() > 1 ? Me.MonthInterval : "");
          ret = ret.concat(" Month");
          ret = ret.concat(schedule.OccursMonthlyDay().MonthInterval() > 1 ? "s" : "");
        }
        if (schedule.OccursMonthlyType() == 1) {
          ret.concat("on the ");
          switch (schedule.OccursMonthlyThe().TheDay().toString()) {
            case "1": ret = ret.concat("First ");
              break;
            case "2": ret = ret.concat("Second ");
              break;
            case "3": ret = ret.concat("Third ");
              break;
            case "4": ret = ret.concat("Fourth ");
              break;
            case "-1": ret = ret.concat("Last ");
              break;
          }
          switch (schedule.OccursMonthlyThe().TheDayOfWeek().toString()) {
            case "0": ret = ret.concat("Monday");
              break;
            case "1": ret = ret.concat("Tuesday");
              break;
            case "2": ret = ret.concat("Wednesday");
              break;
            case "3": ret = ret.concat("Thursday");
              break;
            case "4": ret = ret.concat("Friday");
              break;
            case "5": ret = ret.concat("Saturday");
              break;
            case "6": ret = ret.concat("Sunday");
              break;
            case "7": ret = ret.concat("Day");
              break;
            case "8": ret = ret.concat("Week Day");
              break;
            case "9": ret = ret.concat("Weekend Day");
              break;
          }
        }

        ret = ret.concat(" of ");
        ret = ret.concat(schedule.OccursMonthlyThe().MonthInterval() > 1 ? "every " : "each");
        ret = ret.concat(schedule.OccursMonthlyThe().MonthInterval() > 1 ? Me.MonthInterval : "");
        ret = ret.concat(" Month");
        ret = ret.concat(schedule.OccursMonthlyThe().MonthInterval() > 1 ? "s" : "");
      }

      //Daily Frequency String
      ret = ret.concat(" ");
      if (schedule.DailyFrequencyType() == 0) {

        ret = ret.concat("at ");
        ret = ret.concat((new Date(schedule.DailyFrequencyOnce().AtTime())).format('HH:mm'));
      }
      if (schedule.DailyFrequencyType() == 1) {
        ret = ret.concat("every ");
        ret = ret.concat(schedule.DailyFrequencyEvery().Unit());
        ret = ret.concat(" ");
        switch (schedule.DailyFrequencyEvery().TimeMeasure().toString()) {
          case "1": ret = ret.concat("Hour");
            break;
          case "2": ret = ret.concat("Minute");
            break;
          case "3": ret = ret.concat("Second");
            break;
        }
        ret = ret.concat(schedule.DailyFrequencyEvery().Unit() > 1 ? "s" : "");
        ret = ret.concat(" from ");
        ret = ret.concat((new Date(schedule.DailyFrequencyEvery().StartTime())).format('HH:mm'));
        ret = ret.concat(" to ");
        ret = ret.concat((new Date(schedule.DailyFrequencyEvery().EndTime())).format('HH:mm'));
      }

      //Duration String
      ret = ret.concat(" ");
      ret = ret.concat("Starting on ");
      ret = ret.concat((new Date(schedule.Duration().StartDate())).format('dd MMM yyyy'));
      if (schedule.Duration().HasEndDate() == true) {
        if (schedule.Duration().EndDate()) {
          ret = ret.concat(" and ending on ");
          ret = ret.concat((new Date(schedule.Duration().EndDate())).format('dd MMM yyyy'));
        }
      }
    }
    return ret;
  }

  self.GetScheduleProgress = function(s) {
    if (!s) s = ViewModel.LastLogProgram();
    //ViewModel.LastLogProgram(null);

    ViewModel.CallServerMethod('GetProgress', { ScheduleID: s.ServerProgramTypeID(), ToDate: ViewModel.LogToDate() }, function (data) {
      ViewModel.ServerProgressList.Set(data);
      ViewModel.LastLogProgram(s);
    }, true);

  }

  self.SaveScheduleList = function() {
    ViewModel.CallServerMethod('SaveList', { List: ViewModel.ServerProgramTypeList.Serialise(), ShowLoadingBar: true }, function (data) {
      ViewModel.ServerProgramTypeList.Set(data);
      Singular.AddMessage(3, 'Save', 'Saved Successfully.').Fade(2000);
    }, true);
  }

  self.ExportData = function() {
    ViewModel.DownloadFile('ExportLog', { ScheduleID: ViewModel.LastLogProgram().ServerProgramTypeID(), ToDate: ViewModel.LogToDate() }, function () {

    });
  }

  var CurrentUIPropertyName;
  self.UIText = '';

  self.OpenCustomUI = function (ServerProgram, MenuItem) {
    self.UIText = MenuItem.Text;
    ViewModel.CurrentUIProgram(ServerProgram);
    CurrentUIPropertyName = MenuItem.PropertyName;
    ViewModel[CurrentUIPropertyName].Set();
  }

  self.ShowCustomMenu = function (e, ServerProgram) {

    Singular.ContextMenu.Show(ServerProgram.ServiceMenuItems(), e.target, function (item) {
      Singular.ContextMenu.Hide();
      
      self.OpenCustomUI(ServerProgram, item);
    });
  }

  self.CloseCustomUI = function () {
    ViewModel.CurrentUIProgram(null);
    ViewModel[CurrentUIPropertyName].Clear();
  }

  self.SendServiceMessage = function (MethodName) {

    Singular.Validation.IfValid(ViewModel[CurrentUIPropertyName](), function () {

      ViewModel.CallServerMethod('SendServiceMessage',
        { ServerProgramID: ViewModel.CurrentUIProgram().ServerProgramTypeID(), MethodName: MethodName, MethodArgs: ViewModel[CurrentUIPropertyName].Serialise(), ShowLoadingBar: true },
        function (data) {

          self.CloseCustomUI();
          Singular.ShowMessage(self.UIText, 'Message has been sent to the service, please check the log for progress');

        }, true);
    });

  }

  return self;
})();