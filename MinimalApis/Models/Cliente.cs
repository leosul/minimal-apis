using System.ComponentModel.DataAnnotations;

namespace MinimalApis.Models;

public class Cliente
{
    public Guid Id { get; set; }
    
    [Required, MinLength(3)]
    public string Nome { get; set; }
    public string Nif { get; set; }
    public bool Ativo { get; set; }
}
