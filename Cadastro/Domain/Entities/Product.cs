namespace Cadastro.Domain.Entities
{
    public class Product : BaseModel
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public bool IsAvailable { get; set; } 
        public int ClientId { get; set; } 
        public virtual Client Client { get; set; } 
    }
}