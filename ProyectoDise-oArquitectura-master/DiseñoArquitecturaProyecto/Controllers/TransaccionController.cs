using DiseñoArquitecturaProyecto.Models.DB;
using Microsoft.AspNetCore.Mvc;
using TransactionQueue.Models; // Usando el modelo de Transacción para Redis
using TransactionQueue.Services; // Inyectando el servicio de cola de Redis

namespace DiseñoArquitecturaProyecto.Controllers
{
    public class TransaccionController : Controller
    {
        // Lista simulada de transacciones en base de datos
        private static readonly List<Transaccion> transacciones = new()
        {
            new Transaccion { IdTransaccion = 1, Estado = "Aprobada", Monto = 100, FechaTransaccion = DateTime.Now, IdCliente = 1, IdOrigenCli = 1 },
            new Transaccion { IdTransaccion = 2, Estado = "Rechazada", Monto = 200, FechaTransaccion = DateTime.Now, IdCliente = 2, IdOrigenCli = 2 },
            new Transaccion { IdTransaccion = 3, Estado = "Pendiente", Monto = 300, FechaTransaccion = DateTime.Now, IdCliente = 3, IdOrigenCli = 3 }
        };

        // Lista de logs
        private static readonly List<Log> logs = new();

        // Inyección del servicio de la cola
        private readonly ITransactionQueueService _transactionQueueService;

        // Constructor para inyectar el servicio
        public TransaccionController(ITransactionQueueService transactionQueueService)
        {
            _transactionQueueService = transactionQueueService;
        }

        // Acción para mostrar la vista de validación
        public IActionResult Validar()
        {
            return View();
        }

        // Acción para validar la transacción y encolarla
        [HttpPost]
        public async Task<IActionResult> Validar(decimal id)
        {
            // Buscar la transacción en la lista
            var transaccion = transacciones.FirstOrDefault(t => t.IdTransaccion == id);

            if (transaccion != null)
            {
                // Si la transacción es válida, mapeamos a la clase Transaction para encolarla en Redis
                var transaction = new Transaction
                {
                    Id = transaccion.IdTransaccion.ToString(),
                    Description = $"Transacción de {transaccion.Monto} para cliente {transaccion.IdCliente}",
                    Amount = transaccion.Monto,
                    Timestamp = transaccion.FechaTransaccion
                };

                // Enviar la transacción a la cola de Redis
                await _transactionQueueService.EnqueueTransactionAsync(transaction);

                // Registrar el evento de validación exitosa
                logs.Add(new Log
                {
                    IdLog = logs.Count + 1,
                    Evento = "Validación exitosa",
                    Detalle = $"Transacción encontrada. Estado: {transaccion.Estado}",
                    IdUsuario = 1
                });

                // Mensaje de éxito para la vista
                ViewBag.Resultado = $"<div class='alert alert-success'>Transacción encontrada. Estado: {transaccion.Estado}. Transacción encolada para procesamiento.</div>";
            }
            else
            {
                // Registrar el evento de validación fallida
                logs.Add(new Log
                {
                    IdLog = logs.Count + 1,
                    Evento = "Validación fallida",
                    Detalle = "Transacción inexistente",
                    IdUsuario = 1
                });

                // Mensaje de error para la vista
                ViewBag.Resultado = "<div class='alert alert-danger'>Transacción inexistente</div>";
            }

            return View();
        }
    }
}
