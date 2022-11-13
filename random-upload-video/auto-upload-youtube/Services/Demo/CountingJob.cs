using BaseService.MemoryService;
using BaseService.Shared.Models;
using System.Diagnostics;

namespace API.Services.Demo
{
    public class CountingJobInput
    {
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
    }

    public class CountingJobOutput
    {
        public int TotalNumberCounted { get; set; }
    }

    public class CountingJob : MemoryJob<CountingJobInput, CountingJobOutput>
    {
        protected override async Task<(JobResult Result, string Message)> ExecuteProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                // init
                Output = new CountingJobOutput();

                // execute job
                for (var i = Input.FromNumber; i <= Input.ToNumber; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                    Output.TotalNumberCounted++;

                    ProcessingPercentage = (int)Math.Floor((double)Output.TotalNumberCounted * 100 / (Input.ToNumber - Input.FromNumber + 1));
                    ProcessingMessage = $"Đang đếm đến số {i}";
                }

                return (JobResult.Success, "");
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return (JobResult.Error, "Lỗi không xác định");
            }
            finally
            {
                // release resources
            }
        }
    }
}
