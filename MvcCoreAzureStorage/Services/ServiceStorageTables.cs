using Azure.Data.Tables;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableclient;
        public ServiceStorageTables(TableServiceClient tableService)
        {
            this.tableclient = tableService.GetTableClient("clientes");
        }
        public async Task CreateClientAsync
            (int id, string nombre, string empresa, int edad, int salario)
        {
            Cliente client = new Cliente
            {
                IdCliente = id,
                Nombre = nombre,
                Empresa = empresa,
                Edad = edad,
                Salario = salario,
            };
            await this.tableclient.AddEntityAsync<Cliente>(client);
        }
        // LAS ENTIDADES DE TABLA, SI DESEAMOS BUSCAR POR SU ID
        // SOLAMENTE, NO PODEMOS DEBEMOS HACERLOS MEDIANTE UNA
        // BUSQUEDA DDE SU PARTITION Y SU ROW KEY
        public async Task<Cliente> FindClienteAsync
            (string partitionKey, string rowKey)
        {
            Cliente cliente = await 
                this.tableclient.GetEntityAsync<Cliente>(partitionKey, rowKey);
            return cliente;
        }
        public async Task DeleteClienteAsync
            (string partitionKey, string rowKey)
        {
            await this.tableclient.DeleteEntityAsync
                (partitionKey, rowKey);
        }
        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            // PARA LAS BUSQUEDAD SE UTILIZAN QUERY Y FILTER, AUNQUE
            //NO BUSQUEMOS SI QUEREMOS TODOS LE MANDAMOS UN FILTRO VACIO
            var query = this.tableclient.QueryAsync<Cliente>(filter: "");
            await foreach (Cliente cliente in query)
            {
                clientes.Add(cliente);
            }
            return clientes;
        }
        public async Task<List<Cliente>> GetClientesEmpresaAsync
            (string empresa)
        {
            //TENEMOS DOS TIPOS DE FILTER, Y LOS DOS SON CON QUERY
            // 1) SI UTILIZAMOS EL queryAsync DEBEMOS ESCRIBIR UNA SINTAXIS ESPECIAL
            // DENTRO DEL FILTER: string filter = "Campo eq valor"
            //var query = this.tableclient.QueryAsync<Cliente>
            //    (filter: $"Empresa eq '{empresa}'");
            // 2) UTILIZAR QUERY PERMITE CONSULTAR CON LAMDA PERO SE PIERDE EL ASYNCRONO
            // Y NOS DEBUELVE TODO DIRECTAMENTE, NO DEBEMOS HACER UN BUCLE PARA EXTRAER LOS DATOS
            var query = this.tableclient.Query<Cliente>
                (cliente => cliente.Empresa == empresa);
            return query.ToList();
        }
    }
}
