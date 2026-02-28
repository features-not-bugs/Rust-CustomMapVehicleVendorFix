using Facepunch;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Custom Map Vehicle Vendor Fix", "Pinkstink", "1.0.5")]
    [Description("Links all of the VehicleVendor NPC with the VehicleSpawner entity for custom maps")]
    public class CustomMapVehicleVendorFix : RustPlugin
    {
        void OnServerInitialized()
        {
            var vehicleVendors = UnityEngine.Object.FindObjectsOfType<VehicleVendor>();
            if (vehicleVendors == null || vehicleVendors.Length < 1)
            {
                PrintWarning("Failed to find any Vehicle Vendor entities");
                return;
            }

            foreach (var vehicleVendor in vehicleVendors)
            {
                if (vehicleVendor == null)
                    continue;

                if (vehicleVendor.GetVehicleSpawner() != null && vehicleVendor.vehicleSpawner != null)
                    continue;

                var vehicleSpawners = Pool.Get<List<VehicleSpawner>>();
                Vis.Entities(vehicleVendor.transform.position, 100f, vehicleSpawners);
                if (vehicleSpawners == null || vehicleSpawners.Count < 1)
                {
                    PrintError($"Failed to find Vehicle Spawner for Vendor @ {vehicleVendor.transform.position}");
                    Pool.FreeUnmanaged(ref vehicleSpawners);
                    continue;
                }

                VehicleSpawner vehicleSpawner;
                if (vehicleSpawners.Count > 1)
                {
                    vehicleSpawner = vehicleSpawners.OrderBy(x => Vector3.Distance(x.transform.position, vehicleVendor.transform.position)).First();
                }
                else
                {
                    vehicleSpawner = vehicleSpawners[0];
                }

                if (vehicleSpawner == null)
                {
                    PrintError($"Failed to find a nearby Vehicle Spawner for Vendor @ {vehicleVendor.transform.position}");
                    Pool.FreeUnmanaged(ref vehicleSpawners);
                    continue;
                }

                vehicleVendor.spawnerRef.Set(vehicleSpawner);
                vehicleVendor.vehicleSpawner = vehicleSpawner;
                Puts($"Set Vehicle Spawner for Vehicle Vendor @ {vehicleVendor.transform.position}");
                Pool.FreeUnmanaged(ref vehicleSpawners);
            }
        }
    }
}
