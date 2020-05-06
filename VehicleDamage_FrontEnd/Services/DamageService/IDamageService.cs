using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.DamageService
{
    //Interface which extends to the real or fake service
    public interface IDamageService
    {
        Task<APIDTO> DamageCheckImg(ImageDTO model);
    }
}
