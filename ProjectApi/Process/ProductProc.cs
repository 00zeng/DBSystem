using ProjectApi.Models;
using ProjectShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MySqlSugar;
using System.Web;
using ProjectShare.Process;
using ProjectShare.Database;
using Base.Code;

namespace ProjectApi.Process
{
    public class ProductProc
    {
        public object GetCategoryList()
        {
            try
            {
                List<CategoryWithProductList> categoryList = (List<CategoryWithProductList>)HttpRuntime.Cache.Get(CacheKey.CategoryCache);
                if (categoryList != null)
                {
                    object returnObj = new
                    {
                        errCode = StaticValue.ApiErrCode.SUCCESS,
                        errMsg = "成功",
                        data = categoryList
                    };
                    return returnObj;
                }
                using (var db = SugarDao.GetInstance())
                {
                    categoryList = db.Queryable<jwt_category>()
                            .Where(a => a.active == true).OrderBy("sort asc")
                            .Select<CategoryWithProductList>("id, name").ToList();
                    if (categoryList.Count > 0)
                    {
                        categoryList[0].products = db.Queryable<jwt_product>()
                                    .JoinTable<jwt_product_category>((a, b) => b.product_id == a.id)
                                    .JoinTable<jwt_product_status>((a, c) => c.product_id == a.id)
                                    .Where<jwt_product_category>((a, b) => b.category_id == categoryList[0].id)
                                    .Select<ProductList>("a.id, a.name, a.cover_url, c.cur_price, c.star, c.sales")
                                    .ToList();
                    }
                    HttpRuntime.Cache.Insert(CacheKey.CategoryCache, categoryList,
                                null, DateTime.MaxValue, TimeSpan.FromMinutes(5)); // TODO 缓存不随管理系统更新而更新
                    object returnObj = new
                    {
                        errCode = StaticValue.ApiErrCode.SUCCESS,
                        errMsg = "成功",
                        data = categoryList
                    };
                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                object returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.SYSTEM,
                    errMsg = "系统错误：" + ex.Message
                };
                return returnObj;
            }
        }

        public object GetProductListByCate(int categoryId)
        {
            try
            {
                List<CategoryWithProductList> categoryList
                            = (List<CategoryWithProductList>)HttpRuntime.Cache.Get(CacheKey.CategoryCache);
                if (categoryList == null || categoryList.Count == 0)
                    return GetProductList(categoryId);

                CategoryWithProductList category
                            = categoryList.Where(a => a.id == categoryId).SingleOrDefault();
                if (category == null)
                    return GetProductList(categoryId);

                List<ProductList> productList = category.products;
                if (productList != null && productList.Count() > 0)
                {
                    object returnObj = new
                    {
                        errCode = StaticValue.ApiErrCode.SUCCESS,
                        errMsg = "成功",
                        data = productList
                    };
                    return returnObj;
                }
                using (var db = SugarDao.GetInstance())
                {
                    productList = db.Queryable<jwt_product>()
                                .JoinTable<jwt_product_category>((a, b) => b.product_id == a.id)
                                .JoinTable<jwt_product_status>((a, c) => c.product_id == a.id)
                                .Where<jwt_product_category>((a, b) => b.category_id == categoryId)
                                .Select<ProductList>("a.id, a.name, a.cover_url, c.cur_price, c.star, c.sales")
                                .ToList();

                    HttpRuntime.Cache.Insert(CacheKey.CategoryCache, categoryList,
                                null, DateTime.MaxValue, TimeSpan.FromMinutes(5)); // TODO 缓存不随管理系统更新而更新
                    object returnObj = new
                    {
                        errCode = StaticValue.ApiErrCode.SUCCESS,
                        errMsg = "成功",
                        data = productList
                    };
                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                object returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.SYSTEM,
                    errMsg = "系统错误：" + ex.Message
                };
                return returnObj;
            }
        }

        private object GetProductList(int categoryId)
        {
            using (var db = SugarDao.GetInstance())
            {
                // 查询后直接返回结果，由系统定时缓存
                List<ProductList> productList = db.Queryable<jwt_product>()
                            .JoinTable<jwt_product_category>((a, b) => b.product_id == a.id)
                            .JoinTable<jwt_product_status>((a, c) => c.product_id == a.id)
                            .Where<jwt_product_category>((a, b) => b.category_id == categoryId)
                            .Select<ProductList>("a.id, a.name, a.cover_url, c.cur_price, c.star, c.sales")
                            .ToList();
                object returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.SUCCESS,
                    errMsg = "成功",
                    data = productList
                };
                return returnObj;
            }
        }
    }
}