using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using Pms.Data.Models;
using System.Web.Http.Cors;

namespace Pms.Data.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Pms.Data.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Parking>("Parking");
    builder.EntitySet<Camera>("Cameras"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ParkingController : ODataController
    {
        private pmsEntities db = new pmsEntities();

        // GET: odata/Parking
        [EnableQuery]
        public IQueryable<Parking> GetParking()
        {
            return db.Parkings;
        }

        // GET: odata/Parking(5)
        [EnableQuery]
        public SingleResult<Parking> GetParking([FromODataUri] long key)
        {
            return SingleResult.Create(db.Parkings.Where(parking => parking.ParkingID == key));
        }

        // PUT: odata/Parking(5)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Parking> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Parking parking = await db.Parkings.FindAsync(key);
            if (parking == null)
            {
                return NotFound();
            }

            patch.Put(parking);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(parking);
        }

        // POST: odata/Parking
        public async Task<IHttpActionResult> Post(Parking parking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Parking oParking = await db.Parkings.FindAsync(parking.ParkingID);
            if (oParking == null)
            {
                db.Parkings.Add(parking);
            }
            else
            {
                oParking.ParkingName = parking.ParkingName;
                oParking.SlotsOccupied = parking.SlotsOccupied;
                oParking.SlotsUnderMaintenance = parking.SlotsUnderMaintenance;
                oParking.TotalSlots = parking.TotalSlots;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ParkingExists(parking.ParkingID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(parking);
        }

        // PATCH: odata/Parking(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Parking> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Parking parking = await db.Parkings.FindAsync(key);
            if (parking == null)
            {
                return NotFound();
            }

            patch.Patch(parking);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(parking);
        }

        // DELETE: odata/Parking(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            Parking parking = await db.Parkings.FindAsync(key);
            if (parking == null)
            {
                return NotFound();
            }

            db.Parkings.Remove(parking);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.OK);
        }

        // GET: odata/Parking(5)/Cameras
        [EnableQuery]
        public IQueryable<Camera> GetCameras([FromODataUri] long key)
        {
            return db.Parkings.Where(m => m.ParkingID == key).SelectMany(m => m.Cameras);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ParkingExists(long key)
        {
            return db.Parkings.Count(e => e.ParkingID == key) > 0;
        }
    }
}
