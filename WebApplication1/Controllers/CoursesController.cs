﻿using System;
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
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using WebApplication1.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Course>("Courses");
    builder.EntitySet<Enrollment>("Enrollments"); 
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CoursesController : ODataController
    {
        private CourseContext db = new CourseContext();

        // GET: odata/Courses
        [Queryable]
        public IQueryable<Course> GetCourses()
        {
            return db.Courses;
        }

        // GET: odata/Courses(5)
        [Queryable]
        public SingleResult<Course> GetCourse([FromODataUri] int key)
        {
            return SingleResult.Create(db.Courses.Where(course => course.CourseID == key));
        }

        // PUT: odata/Courses(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != course.CourseID)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(course);
        }

        // POST: odata/Courses
        public async Task<IHttpActionResult> Post(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            return Created(course);
        }

        // PATCH: odata/Courses(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Course> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Course course = await db.Courses.FindAsync(key);
            if (course == null)
            {
                return NotFound();
            }

            patch.Patch(course);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(course);
        }

        // DELETE: odata/Courses(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Course course = await db.Courses.FindAsync(key);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Courses(5)/Enrollments
        [Queryable]
        public IQueryable<Enrollment> GetEnrollments([FromODataUri] int key)
        {
            return db.Courses.Where(m => m.CourseID == key).SelectMany(m => m.Enrollments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int key)
        {
            return db.Courses.Count(e => e.CourseID == key) > 0;
        }
    }
}
