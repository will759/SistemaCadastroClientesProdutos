// Cadastro.Infrastructure.Data.EntityConfig/ProductMap.cs

using Cadastro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infrastructure.Data.EntityConfig
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(m => m.Id);

            // Mapeamento das propriedades da entidade
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Value)
                .IsRequired();
            
            // Configuração da Chave Estrangeira para CLIENT
            builder.Property(m => m.ClientId) 
                .IsRequired();
            
            // Configuração da relação (Um Client tem muitos Products)
            builder.HasOne(m => m.Client) 
                .WithMany(c => c.Products) // Assume que a entidade Client tem uma coleção Products
                .HasForeignKey(m => m.ClientId);
        }
    }
}