using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;
using VehicleDamage_FrontEnd.Models.Staff;

namespace VehicleDamage_FrontEnd.Services.BEService
{
    //Interface which extends to the real or fake service
    public interface IBEService
    {
        Task<VehicleDTO> GetVehicleAsync(string lplateNum);

        Task<IEnumerable<ClockHistoryDTO>> GetClockHistoriesAsync(string lplateNum);

        Task<IEnumerable<DamageHistoryDTO>> GetDamageAsync(string lplateNum);

        Task<IEnumerable<VehicleDTO>> GetVehiclesAsync(VehFilterAPI apiModel);


        Task<IEnumerable<MakeDTO>> GetMakesAsync();


        Task<string> UpdateVehicleAsync(VehicleDTO vehDTO);

        Task<string> UpdateDamageHistoryAsync(DamageHistoryDTO dhDTO);

        Task<string> InsertVehicleAsync(VehicleDTO vehDTO);

        Task<string> InsertClockHistoryAsync(ClockHistoryDTO hisDTO);

        Task<string> InsertDamageHistoryAsync(DamageHistoryDTO hisDTO);

    }
}
