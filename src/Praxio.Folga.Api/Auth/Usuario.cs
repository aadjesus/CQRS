using Microsoft.AspNetCore.Http;
using Praxio.Folga.Domain.Interfaces;
using System;
using System.Security.Claims;

namespace Praxio.Folga.Api.Auth
{
    public class Usuario : IUsuario
    {
        private int _id;
        public int Id
        {
            get { return _id; }
        }
        public Usuario(IHttpContextAccessor accessor)
        {
            var identity = accessor.HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Int32.TryParse(identity.FindFirst("Id")?.Value, out this._id);
            }
        }

    }
}
