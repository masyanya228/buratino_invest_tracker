using Buratino.Entities.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Buratino.Models
{
    public interface IEntityController<T> where T : IEntityBase
    {
        /// <summary>
        /// Реестр по сущности
        /// </summary>
        /// <returns></returns>
        IActionResult Index();
        
        /// <summary>
        /// Страница создания сущности
        /// </summary>
        /// <returns></returns>
        IActionResult Create();
        
        /// <summary>
        /// Сохраняет сущность в репозиторий
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        IActionResult Create(IFormCollection collection);

        /// <summary>
        /// Страница редактирования сущности
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IActionResult Edit(Guid id);

        /// <summary>
        /// Изменяет сущность и сохраняет её в репозиторий
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        IActionResult Edit(Guid id, IFormCollection collection);

        /// <summary>
        /// Удаляет сущность из репозитория
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IActionResult Delete(Guid id);
    }
}