using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using JobSearchingSystem.Models;
using System.Data;

namespace JobSearchingSystem.DAL
{
    public class AccountUnitOfWork : UnitOfWork
    {
        public IEnumerable<AccUserItem> GetUserList()
        {
            IEnumerable<AccUserItem> outList = (from u in this.AspNetUserRepository.Get()
                                                select new AccUserItem
                                                {
                                                    UserName = u.UserName,
                                                    RoleName = u.AspNetRoles.LastOrDefault().Name,
                                                    IsActive = u.IsActive
                                                }).AsEnumerable();
            
            return outList;
        }
    }
}