using TransactionQueue.Models;

namespace TransactionQueue.Services
{
    public interface ITransactionQueueService
    {
        Task EnqueueTransactionAsync(Transaction transaction);
        Task<Transaction> DequeueTransactionAsync();
    }
}
