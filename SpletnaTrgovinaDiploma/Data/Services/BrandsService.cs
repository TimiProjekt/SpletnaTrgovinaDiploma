using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class BrandsService : EntityBaseRepository<Brand>, IBrandsService
    {
        public BrandsService(AppDbContext context) : base(context) { }
    }
}
