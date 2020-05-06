using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;
using VehicleDamage_FrontEnd.Models.Staff;

namespace VehicleDamage_FrontEnd.Services.BEService
{
    public class FakeBEService : IBEService
    {

        private static readonly MakeDTO[] _makes =
           {
                new MakeDTO {id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), name = "Vauxhall"},
                new MakeDTO {id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), name = "BMW"},
                new MakeDTO {id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), name = "Ford"}
            };


        private static readonly DamageHistoryDTO[] _damagehistories =
    {
                new DamageHistoryDTO { Id = Guid.Parse("860ee42c-5626-4e6f-b801-2d06173bca61"), driverID = Guid.NewGuid(), lplateNum = "1234", time = DateTime.Parse("03/May/2020 02:34:18"), resolved = true, state = "Damaged"},
                new DamageHistoryDTO { Id = Guid.Parse("36444a8f-a6fd-403f-8e8c-674a06602c18"), driverID = Guid.NewGuid(), lplateNum = "2345", time = DateTime.Parse("03/May/2020 02:34:18"), resolved = false, state = "Pending"},
                new DamageHistoryDTO { Id = Guid.Parse("38bf7bcc-890e-4fec-8cfe-90cc65180d8c"), driverID = Guid.NewGuid(), lplateNum = "2345", time = DateTime.Parse("03/May/2020 02:34:18"), resolved = true, state = "Damaged"}

            };

        private static readonly ClockHistoryDTO[] _clockhistories =
{
                new ClockHistoryDTO { Id = Guid.NewGuid(), driverID = Guid.NewGuid(), lplateNum = "1234", time = DateTime.Now, state = "In"},
                new ClockHistoryDTO { Id = Guid.NewGuid(), driverID = Guid.NewGuid(), lplateNum = "2345", time = DateTime.Now.AddHours(2), state = "In"},
                new ClockHistoryDTO { Id = Guid.NewGuid(), driverID = Guid.NewGuid(), lplateNum = "2345", time = DateTime.Now, state = "Out"}

            };



        private static readonly VehicleDTO[] _vehicles =
            {
                new VehicleDTO { licenseNum = "1234", make = _makes[0], model = "Corsa", colour = "Red", state = "Out", active = true},
                new VehicleDTO { licenseNum = "2345", make = _makes[1], model = "Corsa", colour = "Blue", state= "Under Investigation", active = true},
                new VehicleDTO { licenseNum = "3456", make = _makes[1], model = "Fiesta", colour = "Green", state = "In", active = true},
                new VehicleDTO { licenseNum = "4567", make = _makes[2], model = "Fiesta", colour = "Black", state = "In", active = true },
                new VehicleDTO { licenseNum = "9999", make = _makes[2], model = "Fiesta", colour = "White", state =" In", active = false }
            };




        public Task<VehicleDTO> GetVehicleAsync(string lplateNum)
        {
            var vehicle = _vehicles.FirstOrDefault(r => r.licenseNum == lplateNum && r.active == true);
            return Task.FromResult(vehicle);
        }

        public Task<IEnumerable<VehicleDTO>> GetVehiclesAsync(VehFilterAPI apiModel)
        {
            var vehicles = _vehicles.AsEnumerable();

            if (apiModel != null)
            {
                if (apiModel.state != "All")
                {
                    vehicles = vehicles.Where(r => r.state == apiModel.state);
                }
                if (apiModel.makeID != Guid.Empty)
                {
                    vehicles = vehicles.Where(r => r.make.id == apiModel.makeID);
                }
                if (apiModel.lPlate != null)
                {
                    vehicles = vehicles.Where(r => r.licenseNum.Contains(apiModel.lPlate, StringComparison.OrdinalIgnoreCase));
                }
            }

            return Task.FromResult(vehicles);
        }

        public Task<IEnumerable<ClockHistoryDTO>> GetClockHistoriesAsync(string lplateNum) 
        {
            var histories = _clockhistories.Where(x => x.lplateNum == lplateNum);

            return Task.FromResult(histories);
        }

        public Task<IEnumerable<DamageHistoryDTO>> GetDamageAsync(string lplateNum) 
        {
            var histories = _damagehistories.Where(x => x.lplateNum == lplateNum);

            return Task.FromResult(histories);
        }


        public Task<IEnumerable<MakeDTO>> GetMakesAsync()
        {
            var makes = _makes.AsEnumerable();
            return Task.FromResult(makes);
        }


        public Task<string> UpdateVehicleAsync(VehicleDTO vehDTO)
        {
            if (vehDTO == null)
            {
                return Task.FromResult("dto passed as null");
            }

            return Task.FromResult("Success");
        }

        public Task<string> InsertVehicleAsync(VehicleDTO vehDTO) 
        {
            return Task.FromResult("Success");
        }

        public Task<string> InsertClockHistoryAsync(ClockHistoryDTO hisDTO)
        {
            if (hisDTO == null)
            {
                return Task.FromResult("dto passed as null");
            }

            return Task.FromResult("Success");
        }


        public Task<string> InsertDamageHistoryAsync(DamageHistoryDTO damDTO)
        {
            if (damDTO == null)
            {
                return Task.FromResult("dto passed as null");
            }

            return Task.FromResult("Success");
        }

        public Task<string> UpdateDamageHistoryAsync(DamageHistoryDTO dhDTO) 
        {
            if (dhDTO == null)
            {
                return Task.FromResult("dto passed as null");
            }

            return Task.FromResult("Success");
        }

    }
}
