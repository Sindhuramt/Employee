using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Employee.Models;

namespace Employee.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext (DbContextOptions<EmployeeContext> options)
            : base(options)
        {
        }

        public DbSet<Employee.Models.EmployeeModel> EmployeeModel { get; set; } = default!;
    }
}
