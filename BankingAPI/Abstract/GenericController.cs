using Domain.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FluentValidation;
using Microsoft.AspNet.OData;
using System.Text.RegularExpressions;
using System;

namespace BankingAPI.Abstract
{
    public abstract class GenericController<TEntity, TValidator> : ReadOnlyController<TEntity> where TEntity : Entity where TValidator : AbstractValidator<TEntity>
    {
        readonly TValidator Validator;

        public GenericController(DbContext context, TValidator validator) : base(context)
        {
            Validator = validator;
        }

        public virtual IActionResult Post([FromBody] TEntity entity)
        {
            if (entity is null)
                return BadRequest();

            try
            {
                entity = Sanitize(entity);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (!IsValid(entity))
                return BadRequest();

            entity = SetCreatedAndUpdatedTimes(entity, DateTimeOffset.Now);

            Repository.Add(entity);
            Context.SaveChanges();
            return Created(entity);
        }

        public virtual IActionResult Delete([FromODataUri] long key)
        {
            var entity = Repository.Find(key); ;

            if (entity is null)
                return NotFound();

            Repository.Remove(entity);
            Context.SaveChanges();
            return Ok();
        }

        public IActionResult Patch([FromODataUri] long key, [FromBody] Delta<TEntity> entityDelta)
        {
            var entity = Repository.Find(key);
            
            if (entity is null)
                return NotFound();

            entity.DateUpdated = DateTimeOffset.Now;

            entityDelta.Patch(entity);
            
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                    return NotFound();
                else
                    throw;
            }

            return Updated(entity);
        }

        public IActionResult Put([FromODataUri]long key, [FromBody] TEntity update)
        {
            if (update is null)
                return BadRequest();

            update.DateUpdated = DateTimeOffset.Now;

            try
            {
                update = Sanitize(update);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }            

            if (!IsValid(update))
                return BadRequest(ModelState);

            if (key != update.Id)
                return BadRequest();

            Context.Entry(update).State = EntityState.Modified;
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                    return NotFound();
                else
                    throw;
            }
            return Updated(update);
        }

        bool EntityExists(long key) => Repository.Any(x => x.Id == key);

        bool IsValid(TEntity entity)
        {
            var results = Validator.Validate(entity);
            return results.IsValid;
        }
    
        TEntity Sanitize(TEntity entity)
        {
            var t = entity.GetType();
            var properties = t.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                var p = t.GetProperty(properties[i].Name);

                if (p.PropertyType == typeof(string))
                {
                    var value = p.GetValue(entity).ToString().Trim();
                    if (Regex.IsMatch(value, @"^[a-zA-Z0-9]+$"))
                        p.SetValue(entity, value);
                    else
                        throw new Exception($"Property '{p.Name}' can must only contain letters and numbers. '{value}' is not a valid value.");
                }                    
            }
            return entity;
        }

        public TEntity SetCreatedAndUpdatedTimes(TEntity entity, DateTimeOffset now)
        {
            entity.DateCreated = now;
            entity.DateUpdated = now;
            return entity;
        }
    }
}
