using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Infra.Entities.Base;

public class PersonBaseEntity : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
