using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InternetShop.InfoDB;
using static Dapper.SqlMapper;

namespace InternetShop
{
    public static class DapperExtensions
    {
        public static IEnumerable<Goods> GetAllGoodsWithOrders(this IDbConnection connection)
        {
            var goodsDict = new Dictionary<int, Goods>();
            connection.Query<Goods, GoodsOrder, Goods>(
                sql: "SELECT * FROM Goods g LEFT JOIN GoodsOrder o ON g.id_goods = o.id_goods",
                map: (goods, order) =>
                {
                    if (!goodsDict.TryGetValue(goods.id_goods, out var goodsEntry))
                    {
                        goodsEntry = goods;
                        goodsEntry.GoodsOrders = new List<GoodsOrder>();
                        goodsDict.Add(goodsEntry.id_goods, goodsEntry);
                    }

                    if (order != null)
                    {
                        goodsEntry.GoodsOrders.Add(order);
                    }

                    return goodsEntry;
                },
                splitOn: "id_goodsOrder"
            );
            return goodsDict.Values;
        }
    }
}
