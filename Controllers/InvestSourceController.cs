﻿using Buratino.Entities;
using Buratino.Models.Xtensions;
using Buratino.ViewDto.Crud;

namespace Buratino.Controllers
{
    public class InvestSourceController : EntityController<InvestSource>
    {

        protected override IEnumerable<ColumnSettings> GetColumnSettings()
        {
            return typeof(InvestSource).GetPropertyList("Name", "ProfitType", "SourceGroup").Select(x => new ColumnSettings(x));
        }
    }
}
