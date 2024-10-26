using Buratino.DI;
using Buratino.Entities.Abstractions;
using Buratino.Models;
using Buratino.Models.DomainService.DomainStructure;
using Buratino.ViewDto.Crud;
using Buratino.Xtensions;
using Microsoft.AspNetCore.Mvc;

namespace Buratino.Controllers
{
    public class EntityController<T> : AutorityController, IEntityController<T> where T : IEntityBase
    {
        protected IDomainService<T> DomainService { get; set; }

        public EntityController()
        {
            DomainService = Container.GetDomainService<T>();
        }

        // GET: EntityController
        public virtual IActionResult Index()
        {
            var entityList = DomainService.GetAll().ToArray();
            return View("List", new CrudListDto()
            {
                EntityType = typeof(T),
                EntityList = entityList as IEnumerable<IEntityBase>,
                ListName = typeof(T).Name,
                ColumnSettings = GetColumnSettings()
            });
        }

        protected virtual IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(T).GetDefaultList().Select(x => new ColumnSettings(x));
        }

        // GET: EntityController/Create
        public virtual IActionResult Create()
        {
            return View("Create", new CrudCreateDto()
            {
                Entity = Activator.CreateInstance<T>(),
                EntityName = typeof(T).Name,
            });
        }

        // POST: EntityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Create(IFormCollection collection)
        {
            var entity = Activator.CreateInstance<T>();
            CreateOrEditEntity(entity, collection);

            var accProp = entity.GetProp("Account");
            if (accProp != null)
            {
                accProp.SetValue(entity, Account);
            }
            DomainService.Save(entity);
            return Redirect($"Edit?id={entity.Id}");
        }

        // GET: EntityController/Edit/5
        public virtual IActionResult Edit(Guid id)
        {
            var entity = DomainService.Get(id);
            return View("Edit", new CrudEditDto()
            {
                Entity = entity,
                EntityName = typeof(T).Name,
            });
        }

        // POST: EntityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Edit(Guid id, IFormCollection collection)
        {
            var entity = DomainService.Get(id);
            CreateOrEditEntity(entity, collection);
            DomainService.Save(entity);
            return View("Edit", new CrudEditDto()
            {
                Entity = entity,
                EntityName = typeof(T).Name,
            });
        }

        // GET: EntityController/Delete/5
        public virtual IActionResult Delete(Guid id)
        {
            var entity = DomainService.Get(id);
            DomainService.Delete(entity);
            return RedirectToAction("Index");
        }

        protected virtual T CreateOrEditEntity(T entity, IFormCollection collection)
        {
            var type = entity.GetType();
            foreach (var item in collection.Keys)
            {
                if (item.StartsWith("_") || item == "Image")
                    continue;
                var value = collection[item].ToString();
                var prop = type.GetProperty(item);
                prop.SetValue(entity, value.StringValueCast(prop.PropertyType));
            }
            SaveFiles(entity, collection.Files);
            return entity;
        }

        protected virtual void SaveFiles(T entity, IFormFileCollection files)
        {
            var type = entity.GetType();
            foreach (var item in files)
            {
                var guid = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                string path = Path.Combine("Images", guid);
                //System.IO.File.Create(path);
                using (var st = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    item.CopyTo(st);
                }
                var prop = type.GetProperty(item.Name);
                prop.SetValue(entity, guid);
            }
        }
    }
}
