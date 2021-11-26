using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METT.Service
{
  class SystemProgram : Singular.Service.ScheduleProgramBase
  {

    public SystemProgram() : base(1, "System") { }

    protected override bool StartSchedule()
    {
      WriteProgress("METT System Service Started");
      WriteProgress("Scheduler Version: " + Program.SchedulerVersionNo);
      //WriteProgress("Business Layer Version: " + IFRS9Lib.CommonData.VersionNo);
      return true;
    }

    protected override bool StopSchedule()
    {
      return false;
    }

    protected override void TimeUp()
    {
      WriteProgress("METT System Service running (v" + Program.SchedulerVersionNo + ")");

      //var cProc = new Singular.CommandProc("[cmdProcs].[cmdClearOldProgramLogs]");
      //cProc.CommandType =  System.Data.CommandType.StoredProcedure;
      //cProc = cProc.Execute();

      //WriteProgress("Old Logs Cleared");

    }


  }
}
