using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intrastructure.Data;
using Application.Interfaces;

namespace Intrastructure.Repositories;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
        => _context = context;

    public async Task<List<Product>> ObtenerFacturasAsync()
    {
        return await _context.Products
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }
}