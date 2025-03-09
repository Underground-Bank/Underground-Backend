using Quartz;

namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class BackgroundJobDto
    {
        public IJobDetail Job { get; set; }
        public ITrigger Trigger { get; set; }
    }
}