using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base.Code;
using MySqlSugar;
using ProjectShare.Process;
using ProjectShare.Models;

namespace ProjectWeb.Areas.SystemManage.Application
{
    public class SysMenuApp
    {
        #region 获取菜单列表-首页权限(要添加缓存)
        public object GetMenuList(int roleId, ref object authorizeButton)
        {
            using (var db = SugarDao.GetInstance())
            {
                int menu = Convert.ToInt32(ConstData.AuthorityCategory.MENU);
                int button = Convert.ToInt32(ConstData.AuthorityCategory.BUTTON);
                AuthMenuButton authMenuButton = new SysAuthorityApp().GetAuthorityCache(roleId);
                authorizeButton = GetMenuButtonList(authMenuButton.buttonList);
                return ToMenuJson(authMenuButton.menuList);
            }
        }
        //获取菜单json
        private string ToMenuJson(List<MenuSearch> data)
        {
            List<MenuSearch> menuLevel1 = new List<MenuSearch>();
            List<MenuSearch> menuLevel2 = new List<MenuSearch>();
            List<MenuSearch> menuLevel3 = new List<MenuSearch>();
            data.ForEach(a =>
            {
                if (a.parent_code == "0")
                    menuLevel1.Add(a);
                else if (a.parent_code.Length == 2)
                    menuLevel2.Add(a);
                else if (a.parent_code.Length == 4)
                    menuLevel3.Add(a);
            });

            menuLevel2.ForEach(a2 => a2.ChildNodes = menuLevel3.Where(a3 => a3.parent_code == a2.encode).OrderBy(a3 => a3.sort).ToList());
            menuLevel1.ForEach(a1 => a1.ChildNodes = menuLevel2.Where(a2 => a2.parent_code == a1.encode).OrderBy(a2 => a2.sort).ToList());
            return menuLevel1.ToJson();

        }
        //获取按钮
        private object GetMenuButtonList(List<MenuSearch> data)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            var dataModuleId = data.Distinct(new ExtList<MenuSearch>("parent_code"));
            foreach (var item in dataModuleId)
            {
                dictionary.Add(item.parent_code, data.Where(t => t.parent_code.Equals(item.parent_code)));
            }
            return dictionary;
        }
        #endregion
    }
}
