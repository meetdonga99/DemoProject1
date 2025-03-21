﻿using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class RolesProvider : BaseProvider
    {
        public RolesProvider()
        {

        }
        public List<webpages_Roles> GetAllRoles()
        {
            var data = (from a in _db.webpages_Roles select a).OrderByDescending(a => a.RoleId).ToList();
            return data;
        }
        public IQueryable<RolesGridModel> GetAllRolesGrid()
        {
            return (from role in _db.webpages_Roles
                    select new RolesGridModel()
                    {
                        Id = role.RoleId,
                        IsActive = role.IsActive,
                        Name = role.RoleName,
                        RoleCode = role.RoleCode
                    }).AsQueryable();
        }
        public int CreateRoles(webpages_Roles role)
        {
            try
            {
                _db.webpages_Roles.Add(role);
                _db.SaveChanges();
                return role.RoleId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateRoles(webpages_Roles role)
        {
            try
            {
                _db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return role.RoleId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public webpages_Roles GetRolesById(int id)
        {
            return _db.webpages_Roles.Find(id);
        }

        public webpages_Roles GetRolesByName(string roleName)
        {
            return _db.webpages_Roles.Where(x => x.RoleName == roleName).FirstOrDefault();
        }
        public List<webpages_Roles> CheckDuplicateRoleCode(string RoleCode)
        {
            var getRoleDetails = (from role in _db.webpages_Roles
                                  where role.RoleCode.ToUpper().Trim() == RoleCode.ToUpper().Trim()
                                  select role).ToList();
            return getRoleDetails;
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                var role = _db.webpages_Roles.Find(roleId);
                if (role == null)
                {
                    return false;
                }
                var userRoles = _db.webpages_UsersInRoles.Where(ur => ur.RoleId == roleId).ToList();
                _db.webpages_UsersInRoles.RemoveRange(userRoles);
                _db.SaveChanges();

                _db.webpages_Roles.Remove(role);
                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsAnyUserForThisRole(int roleId)
        {
            var role_users = (from rec in _db.webpages_UsersInRoles where roleId == rec.RoleId select rec).ToList();
            if(role_users.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public bool IsAnyActiveUserForThisRole(int roleId)
        //{
        //    if (IsAnyUserForThisRole(roleId))
        //    {
        //        var userInRoleIds = _db.webpages_UsersInRoles.Where(u => u.RoleId == roleId).Select(u => u.UserId).ToList();
        //        var activeUserExists = _db.UserProfile.Any(user => userInRoleIds.Contains(user.UserId) && user.IsActive);
        //        return activeUserExists;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
