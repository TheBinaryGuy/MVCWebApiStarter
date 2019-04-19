using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class DataProtectionKeyRepository : IXmlRepository
    {
        private readonly AppDbContext _db;

        public DataProtectionKeyRepository(AppDbContext db)
        {
            _db = db;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return new ReadOnlyCollection<XElement>(_db.DataProtectionKeys.Select(k => XElement.Parse(k.XmlData)).ToList());
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var entity = _db.DataProtectionKeys.SingleOrDefault(k => k.FriendlyName == friendlyName);
            if (null != entity)
            {
                entity.XmlData = element.ToString();
                _db.DataProtectionKeys.Update(entity);
            }
            else
            {
                _db.DataProtectionKeys.Add(
                    new DataProtectionKey
                    {
                        FriendlyName = friendlyName,
                        XmlData = element.ToString()
                    });
            }

            _db.SaveChanges();
        }
    }
}
