﻿using Data.DatabaseContext;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using AutoGenerated;

namespace App.Implementations 
{
    public class DiscountService : IDiscountService.IDiscountServiceBase
    {
        private readonly DiscountDbContext _context;
        private static readonly Random Random = new Random();
        private static readonly object Lock = new object();

        public DiscountService(DiscountDbContext context)
        {
            _context = context;
        }





        public override async Task<GenerateResponse> GenerateCodes(GenerateRequest request, ServerCallContext context)
        {
            var response = new GenerateResponse();
            if (request.Count > 2000)
            {
                response.Result = false;
                return response;
            }

            var codes = new List<string>();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    for (int i = 0; i < request.Count; i++)
                    {
                        string code;
                        do
                        {
                            code = GenerateCode((int)request.Length); // Cast request.Length to int
                        } while (await _context.DiscountCodes.AnyAsync(c => c.Code == code));

                        codes.Add(code);
                        _context.DiscountCodes.Add(new DiscountCode { Code = code });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    response.Result = false;
                    return response;
                }
            }

            response.Result = true;
            response.Codes.AddRange(codes);
            return response;
        }






        public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
        {
            var response = new UseCodeResponse();
            var code = await _context.DiscountCodes.FirstOrDefaultAsync(c => c.Code == request.Code);
            if (code == null || code.Used)
            {
                response.Result = 0; // Not found or already used
            }
            else
            {
                code.Used = true;
                await _context.SaveChangesAsync();
                response.Result = 1; // Successfully used
            }
            return response;
        }







        private static string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            lock (Lock)
            {
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[Random.Next(s.Length)]).ToArray());
            }
        }






    }
}
