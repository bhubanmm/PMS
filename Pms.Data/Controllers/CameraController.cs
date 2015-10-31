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
    builder.EntitySet<Camera>("Camera");
    builder.EntitySet<Parking>("Parkings"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CameraController : ODataController
    {
        private pmsEntities db = new pmsEntities();

        // GET: odata/Camera
        /// <summary>
        /// Gets the cameras.
        /// </summary>
        /// <returns>A list of cameras.</returns>
        [EnableQuery]
        public IQueryable<Camera> GetCamera()
        {
            return db.Cameras;
        }

        // GET: odata/Camera(5)
        /// <summary>
        /// Gets the camera object for the provided key.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>The Camera object.</returns>
        [EnableQuery]
        public SingleResult<Camera> GetCamera([FromODataUri] long key)
        {
            return SingleResult.Create(db.Cameras.Where(camera => camera.CameraId == key));
        }

        // PUT: odata/Camera(5)
        /// <summary>
        /// Puts the specified camera object.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <param name="patch">The patch/delta of camera object.</param>
        /// <returns>The Camera object.</returns>
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Camera> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Camera camera = await db.Cameras.FindAsync(key);
            if (camera == null)
            {
                return NotFound();
            }

            patch.Put(camera);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CameraExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(camera);
        }

        // POST: odata/Camera
        /// <summary>
        /// Posts the specified camera object.
        /// </summary>
        /// <param name="camera">The camera object.</param>
        /// <returns>The Camera object.</returns>
        public async Task<IHttpActionResult> Post(Camera camera)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Camera oCamera = await db.Cameras.FindAsync(camera.CameraId);
            if (oCamera == null)
            {
                db.Cameras.Add(camera);
            }
            else
            {
                oCamera.CameraName = camera.CameraName;
                oCamera.RelatedParkingId = camera.RelatedParkingId;
                oCamera.SecurityKey = camera.SecurityKey;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CameraExists(camera.CameraId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(camera);
        }

        // PATCH: odata/Camera(5)
        /// <summary>
        /// Patches the specified camera object.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <param name="patch">The patch/delta of camera object.</param>
        /// <returns>The camera object.</returns>
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Camera> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Camera camera = await db.Cameras.FindAsync(key);
            if (camera == null)
            {
                return NotFound();
            }

            patch.Patch(camera);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CameraExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(camera);
        }

        // DELETE: odata/Camera(5)
        /// <summary>
        /// Deletes the specified camera.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>The HTTP Status Code.</returns>
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            Camera camera = await db.Cameras.FindAsync(key);
            if (camera == null)
            {
                return NotFound();
            }

            db.Cameras.Remove(camera);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Camera(5)/Parking
        /// <summary>
        /// Gets the parking related to a particular Camera.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>A Parking object.</returns>
        [EnableQuery]
        public SingleResult<Parking> GetParking([FromODataUri] long key)
        {
            return SingleResult.Create(db.Cameras.Where(m => m.CameraId == key).Select(m => m.Parking));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Does the Camera the exists.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns><c>true</c> if the camera exists, else <c>false</c></returns>
        private bool CameraExists(long key)
        {
            return db.Cameras.Count(e => e.CameraId == key) > 0;
        }
    }
}
