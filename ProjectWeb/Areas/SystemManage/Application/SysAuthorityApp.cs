using System;
using System.Collections.Generic;
using System.Linq;
using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using ProjectShare.Models;
using ProjectWeb.Application;

namespace ProjectWeb.Areas.SystemManage.Application
{
    public class SysAuthorityApp
    {
        public string GetAuthority(int roleId)
        {
            AuthMenuButton authMenuBtn = new AuthMenuButton();
            int menu = Convert.ToInt32(ConstData.AuthorityCategory.MENU);
            int button = Convert.ToInt32(ConstData.AuthorityCategory.BUTTON);
            List<AuthorityTreeModel> menuTree = new List<AuthorityTreeModel>();
            List<AuthorityTreeModel> btnTree = new List<AuthorityTreeModel>();
            using (var db = SugarDao.GetInstance())
            {
                menuTree = db.Queryable<daoben_sys_menu>()
                            .JoinTable<daoben_sys_authority>((a, b) => a.id == b.value_id && b.type == menu && b.role_id == roleId)
                            .Where<daoben_sys_authority>((a, b) => a.active == true)
                            .OrderBy(a => a.sort)
                            .Select<AuthorityTreeModel>(string.Format("a.id,a.encode as value,a.parent_code as parentId,a.name as text,{0} as category,a.sort,if(b.id>0, 1, 0) as checkstate, true as isexpand, true as complete, true as showcheck", menu))
                            .ToList();
                btnTree = db.Queryable<daoben_sys_menu_button>()
                            .JoinTable<daoben_sys_authority>((a, b) => a.id == b.value_id && b.type == button && b.role_id == roleId)
                            .Where<daoben_sys_authority>((a, b) => a.active == true)
                            .OrderBy(a => a.sort)
                            .Select<AuthorityTreeModel>(string.Format("a.id,a.encode as value,a.menu_code as parentId,a.name as text,{0} as category,a.sort,if(b.id>0, 1, 0) as checkstate, true as isexpand, true as complete, true as showcheck", button))
                            .ToList();
            }

            List<AuthorityTreeModel> menuLevel1 = new List<AuthorityTreeModel>();
            List<AuthorityTreeModel> menuLevel2 = new List<AuthorityTreeModel>();
            List<AuthorityTreeModel> menuLevel3 = new List<AuthorityTreeModel>();
            List<AuthorityTreeModel> btnLevel2 = new List<AuthorityTreeModel>(); 
            List<AuthorityTreeModel> btnLevel3 = new List<AuthorityTreeModel>();
            menuTree.ForEach(a => {
                if (a.parentId == "0")
                    menuLevel1.Add(a);
                else if (a.parentId.Length == 2)
                    menuLevel2.Add(a);
                else if (a.parentId.Length == 4)
                    menuLevel3.Add(a);
            });
            btnTree.ForEach(a => {
                if (a.parentId.Length == 2)
                    btnLevel2.Add(a);
                else if (a.parentId.Length == 4)
                    btnLevel3.Add(a);
            });
            menuLevel3.ForEach(a3 => {
                a3.ChildNodes = btnLevel3.Where(b3 => b3.parentId == a3.value).ToList();
                if (a3.ChildNodes.Count > 0)
                    a3.hasChildren = true;
                else
                    a3.hasChildren = false;
            });
            menuLevel2.ForEach(a2 => {
                a2.ChildNodes = menuLevel3.Where(a3 => a3.parentId == a2.value).ToList();
                if (a2.ChildNodes.Count > 0)
                    a2.hasChildren = true;
                else
                {   // 有下级menu就不会再有下级button
                    a2.ChildNodes = btnLevel3.Where(b3 => b3.parentId == a2.value).ToList();
                    if (a2.ChildNodes.Count > 0)
                        a2.hasChildren = true;
                    else
                        a2.hasChildren = false;
                }
            });
            menuLevel1.ForEach(a1 => {
                a1.ChildNodes = menuLevel2.Where(a2 => a2.parentId == a1.value).ToList();
                if (a1.ChildNodes.Count > 0)
                    a1.hasChildren = true;
                else
                    a1.hasChildren = false;
            });
            return menuLevel1.ToJson();
        }

        #region 更新角色权限
        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="authList">权限</param>
        /// <returns></returns>
        public string EditAuthority(int roleId, List<daoben_sys_authority> authList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_role roleInfo = db.Queryable<daoben_ms_role>().InSingle(roleId);
                    if (roleInfo == null || roleInfo.inactive)
                        return "指定角色不存在或已注销，操作失败!";

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    try
                    {
                        db.Delete<daoben_sys_authority>(a => a.role_id == roleId);
                        if (authList.Count() > 20)
                            db.SqlBulkCopy(authList);
                        else
                            db.InsertRange(authList);
                        db.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTran();
                        ExceptionApp.WriteLog("SysAuthorityApp(EditAuthority)：" + ex.Message);
                        return "系统出错：" + ex.Message;
                    }
                    //清除缓存
                    new CacheApp().RemoveSingleCache(CachePrefix.AUTHORITY.ToString() + roleId.ToString());
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 权限判断
        public bool ActionValidate(int roleId, string reqUrl)
        {
            AuthMenuButton authMenuBtn = GetAuthorityCache(roleId);
            if (authMenuBtn.menuList.Exists(t => t.url.Equals(reqUrl, StringComparison.OrdinalIgnoreCase))
                        || authMenuBtn.buttonList.Exists(t => t.url.Equals(reqUrl, StringComparison.OrdinalIgnoreCase)))
                return true;
            return false;
        }
        #endregion

        #region 获取角色权限
        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        public AuthMenuButton GetAuthorityCache(int roleId)
        {
            string cacheSuffix = roleId == 0 ? "" : roleId.ToString();
            AuthMenuButton authMenuBtn = CacheFactory.Cache().GetCache<AuthMenuButton>(CachePrefix.AUTHORITY.ToString() + cacheSuffix);
            //List<MenuSearch> authorityData = new List<MenuSearch>();
            //List<MenuSearch> cacheData = CacheFactory.Cache().GetCache<List<MenuSearch>>(CachePrefix.AUTHORITY.ToString() + cacheSuffix);
            if (authMenuBtn == null)
            {
                authMenuBtn = new AuthMenuButton();
                int menu = Convert.ToInt32(ConstData.AuthorityCategory.MENU);
                int button = Convert.ToInt32(ConstData.AuthorityCategory.BUTTON);
                using (var db = SugarDao.GetInstance())
                {
                    if (roleId == ConstData.ROLE_ID_ADMIN)
                    {
                        //系统管理员
                        authMenuBtn.menuList = db.Queryable<daoben_sys_menu>()
                                    .Where(a => a.active == true).OrderBy(a => a.sort)
                                    .Select<MenuSearch>(String.Format("id,encode,name,url,active,{0} as category,sort,parent_code,param,icon", menu))
                                    .ToList();
                        authMenuBtn.buttonList = db.Queryable<daoben_sys_menu_button>()
                                    .Where(a => a.active == true).OrderBy(a => a.sort)
                                    .Select<MenuSearch>(String.Format("id,encode,name,url,active,{0} as category,sort,menu_code as parent_code,param,'' as icon", button))
                                    .ToList();
                    }
                    else
                    {
                        authMenuBtn.menuList = db.Queryable<daoben_sys_menu>()
                                     .JoinTable<daoben_sys_authority>((a, b) => a.id == b.value_id, JoinType.Right)
                                     .Where<daoben_sys_authority>((a, b) => a.active == true && b.role_id.Equals(roleId) && b.type == menu)
                                     .OrderBy(a => a.sort)
                                     .Select<MenuSearch>(String.Format("a.id,a.encode,a.name,a.url,a.active,{0} as category,a.sort,a.parent_code,a.param,a.icon", menu))
                                     .ToList();
                        authMenuBtn.buttonList = db.Queryable<daoben_sys_menu_button>()
                                       .JoinTable<daoben_sys_authority>((a, b) => a.id == b.value_id, JoinType.Right)
                                       .Where<daoben_sys_authority>((a, b) => a.active == true && b.type == button && b.role_id.Equals(roleId))
                                       .OrderBy(a => a.sort)
                                       .Select<MenuSearch>(String.Format("a.id,a.encode,a.name,a.url,a.active,{0} as category,sort,a.menu_code as parent_code,a.param,'' as icon", button))
                                       .ToList();
                    }
                    CacheFactory.Cache().WriteCache(authMenuBtn, CachePrefix.AUTHORITY.ToString() + cacheSuffix, DateTime.Now.AddMinutes(5));
                }
            }
            return authMenuBtn;
        }
        #endregion
    }
}
