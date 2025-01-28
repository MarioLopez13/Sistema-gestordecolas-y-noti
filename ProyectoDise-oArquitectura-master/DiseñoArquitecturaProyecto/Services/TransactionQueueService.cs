using StackExchange.Redis;
using TransactionQueue.Models;
using System.Text.Json;


namespace TransactionQueue.Services
{
    public class TransactionQueueService : ITransactionQueueService
    {
        private readonly IDatabase _database;

        public TransactionQueueService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task EnqueueTransactionAsync(Transaction transaction)
        {
            // Serializamos la transacción y la agregamos a la cola
            var serializedTransaction = JsonSerializer.Serialize(transaction);
            await _database.ListRightPushAsync("transactionQueue", serializedTransaction);
        }

        public async Task<Transaction> DequeueTransactionAsync()
        {
            // Obtenemos la siguiente transacción de la cola
            var serializedTransaction = await _database.ListLeftPopAsync("transactionQueue");

            if (serializedTransaction.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<Transaction>(serializedTransaction);
        }
    }
}
