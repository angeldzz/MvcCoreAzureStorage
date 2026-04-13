using Azure;
using Azure.Data.Tables;

namespace MvcCoreAzureStorage.Models
{
    public class Cliente: ITableEntity
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Salario { get; set; }
        // ID CLIENTE: ROW KEY
        // CUANDO EL USUARIO ALMACENE UN ID DE CLIENTE,
        // NOSOTROS ALMACENAMOS ROWKEY

        private int _IdCliente;
        public int IdCliente
        {
            get { return _IdCliente; }
            set
            {
                _IdCliente = value;
                this.RowKey = value.ToString();
            }
        }
        // EMPRESA: PARTITION KEY
        // cUANDO EL USUARIO ALMACENE UNA EMPRESA, NOSOTROS ALMACENAMOS PARTITION KEY
        private string _Empresa;
        public string Empresa
        {
            get { return _Empresa; }
            set
            {
                _Empresa = value;
                this.PartitionKey = value;
            }
        }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
