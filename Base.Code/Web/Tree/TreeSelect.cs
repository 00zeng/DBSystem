using System.Collections.Generic;
using System.Text;

namespace Base.Code
{
    public static class TreeSelect
    {
        public static string TreeSelectJson(this List<TreeSelectModel> data,string parentId="0")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(TreeSelectJson(data, parentId, "", parentId));
            sb.Append("]");
            return sb.ToString();
        }
        private static string TreeSelectJson(List<TreeSelectModel> data, string parentId, string blank,string parentOrgId)
        {
            StringBuilder sb = new StringBuilder();
            var ChildNodeList = data.FindAll(t => t.parentId == parentId);
            var tabline = "";
            if (parentId != parentOrgId)
            {
                tabline = "　　";
            }
            if (ChildNodeList.Count > 0)
            {
                tabline = tabline + blank;
            }
            foreach (TreeSelectModel entity in ChildNodeList)
            {
                entity.text = tabline + entity.text;
                string strJson = entity.ToJson();
                sb.Append(strJson);
                sb.Append(TreeSelectJson(data, entity.id, tabline, parentOrgId));
            }
            return sb.ToString().Replace("}{", "},{");
        }
    }
}