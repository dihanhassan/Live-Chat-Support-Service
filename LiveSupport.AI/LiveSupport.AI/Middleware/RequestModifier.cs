
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

using System;
using System.Threading.Tasks;

namespace LiveSupport.AI.Middleware
{
    public class RequestModifier
    {
        private readonly RequestDelegate _next;


        public RequestModifier(RequestDelegate next)
        {
            _next = next;
           
        }

        public async Task InvokeAsync(HttpContext context)
        
        {
            try
            {
               

               await _next(context);
               
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
       
    }
}
