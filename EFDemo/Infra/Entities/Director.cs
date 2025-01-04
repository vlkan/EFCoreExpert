using EFDemo.Infra.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Infra.Entities;

public class Director : PersonBaseEntity
{
    public virtual ICollection<Movie> Movies { get; set; }
}