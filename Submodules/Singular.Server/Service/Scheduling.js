
function getScheduleInfo() {
  var s = self;
  var ret = "";
  if (s.Info()) {
    var obj = s.Info();
    //Occurs String
    switch (obj.OccursType()) {
      case "0":
        ret = ret.concat("Every");
        ret = ret.concat(obj.OccursDaily().DayInterval() == 1 ? "" : " ");
        ret = ret.concat(obj.OccursDaily().DayInterval() == 1 ? "" : obj.OccursDaily().DayInterval());
        ret = ret.concat(" Day");
        ret = ret.concat(obj.OccursDaily().DayInterval() > 1 ? "s" : "");
        break;
      case "1":
        ret = ret.concat("Every");
        ret = ret.concat(obj.OccursWeekly().WeekInterval() == 1 ? "" : " ");
        ret = ret.concat(obj.OccursWeekly().WeekInterval() == 1 ? "" : obj.OccursWeekly().WeekInterval());
        ret = ret.concat(" Week");
        ret = ret.concat(obj.OccursWeekly().WeekInterval() > 1 ? "s" : "");
        ret = ret.concat(" on ");
        if (obj.OccursWeekly().Monday() == true) {
          ret = ret.concat("Monday, ");
        }
        if (obj.OccursWeekly().Tuesday() == true) {
          ret = ret.concat("Tuesday, ");
        }
        if (obj.OccursWeekly().Wednesday() == true) {
          ret = ret.concat("Wednesday, ");
        }
        if (obj.OccursWeekly().Thursday() == true) {
          ret = ret.concat("Thursday, ");
        }
        if (obj.OccursWeekly().Friday() == true) {
          ret = ret.concat("Friday, ");
        }
        if (obj.OccursWeekly().Saturday() == true) {
          ret = ret.concat("Saturday, ");
        }
        if (obj.OccursWeekly().Sunday() == true) {
          ret = ret.concat("Sunday, ");
        }
        break;
      case "2":
        switch (obj.OccursMonthlyType()) {
          case "0":
            ret = ret.concat("on day ");
            ret = ret.concat(obj.OccursMonthlyDay().Day());
            ret = ret.concat(" of ");
            ret = ret.concat(obj.OccursMonthlyDay().MonthInterval() > 1 ? "every " : "each");
            ret = ret.concat(obj.OccursMonthlyDay().MonthInterval() > 1 ? Me.MonthInterval : "");
            ret = ret.concat(" Month");
            ret = ret.concat(obj.OccursMonthlyDay().MonthInterval() > 1 ? "s" : "");
            break;
          case "1":
            ret.concat("on the ");
            switch (obj.OccursMonthlyThe().TheDay()) {
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
            break;
            switch (obj.OccursMonthlyThe().TheDayOfWeek()) {
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
            ret = ret.concat(" of ");
            ret = ret.concat(obj.OccursMonthlyThe().MonthInterval() > 1 ? "every " : "each");
            ret = ret.concat(obj.OccursMonthlyThe().MonthInterval() > 1 ? Me.MonthInterval : "");
            ret = ret.concat(" Month");
            ret = ret.concat(obj.OccursMonthlyThe().MonthInterval() > 1 ? "s" : "");
            break;
        }
    }
    //Daily Frequency String
    ret = ret.concat(" ");
    switch (obj.DailyFrequencyType()) {
      case "0":
        ret = ret.concat("at ");
        ret = ret.concat((new Date(obj.DailyFrequencyOnce().AtTime())).toTimeString());
        break;
      case "1":
        ret = ret.concat("every ");
        ret = ret.concat(obj.DailyFrequencyEvery().Unit());
        ret = ret.concat(" ");
        switch (obj.DailyFrequencyEvery().TimeMeasure()) {
          case "1": ret = ret.concat("Hour");
            break;
          case "2": ret = ret.concat("Minute");
            break;
          case "3": ret = ret.concat("Second");
            break;
        }
        ret = ret.concat(obj.DailyFrequencyEvery().Unit() > 1 ? "s" : "");
        ret = ret.concat(" from ");
        ret = ret.concat((new Date(obj.DailyFrequencyEvery().StartTime())).toTimeString());
        ret = ret.concat(" to ");
        ret = ret.concat((new Date(obj.DailyFrequencyEvery().EndTime())).toTimeString());
        break;
    }
    //Duration String
    ret = ret.concat(" ");
    ret = ret.concat("Starting on ");
    ret = ret.concat((new Date(obj.Duration().StartDate())).toDateString());
    if (obj.Duration().HasEndDate() == true) {
      if (obj.Duration().EndDate()) {
        ret = ret.concat(" and ending on ");
        ret = ret.concat((new Date(obj.Duration().EndDate())).toDateString());
      }
    }
  }
  return ret;
}