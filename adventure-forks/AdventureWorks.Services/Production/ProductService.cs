﻿using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace AdventureWorks.Services.Production
{
    using Common;

    public class ProductService : IProductService
    {
        private readonly DbModel.Entities _entities = new DbModel.Entities();
        private readonly ILogger _log;

        public ProductService()
        {
            _log = ServiceConfigurator.GetLogger();
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                var entities = _entities.Products;
                return entities.ToArray().Select(MapProductFromDb);
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception");
                throw;
            }
        }

        public Product GetProduct(int id)
        {
            try
            {
                _log.Debug($"Get product with id = {id}");
                return MapProductFromDb(_entities.Products.Find(id));
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception");
                throw;
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                var productToUpdate = _entities.Products.Find(product.Id);
                if (productToUpdate == null) return;
                _entities.Entry(productToUpdate).CurrentValues.SetValues(MapProductForDb(product));
                _entities.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception");
                throw;
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                var entity = _entities.Products.SingleOrDefault(x => x.ProductID == id);
                if (entity == null) return;
                _entities.Products.Remove(entity);
                _entities.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception");
                throw;
            }
        }

        public void AddProduct(Product product)
        {
            try
            {
                _entities.Products.Add(MapProductForDb(product));
                _entities.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception");
                throw;
            }
        }

        private Product MapProductFromDb(DbModel.Product product)
        {
            return new Product()
            {
                Id = product.ProductID,
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                MakeFlag = product.MakeFlag,
                FinishedGoodsFlag = product.FinishedGoodsFlag,
                Color = product.Color,
                SafetyStockLevel = product.SafetyStockLevel,
                ReorderPoint = product.ReorderPoint,
                StandardCost = product.StandardCost,
                ListPrice = product.ListPrice,
                Size = product.Size,
                SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                //SizeUnitMeasure = MapUnitMeasureFromDb(product.SizeUnitMeasureCode),
                //WeightUnitMeasure = MapUnitMeasureFromDb(product.WeightUnitMeasureCode),
                Weight = product.Weight,
                DaysToManufacture = product.DaysToManufacture,
                ProductLine = product.ProductLine,
                Class = product.Class,
                Style = product.Style,
                ProductSubcategoryId = product.ProductSubcategoryID,
                //ProductSubcategory = _entities.ProductSubcategories
                //        .Where(x => x.ProductSubcategoryID == product.ProductSubcategoryID.Value).Select(x =>
                //            new ProductSubcategory
                //            {
                //                Id = x.ProductSubcategoryID,
                //                Name = x.Name,
                //                ModifiedDate = x.ModifiedDate,
                //                ProductCategory = _entities.ProductCategories
                //                    .Where(y => y.ProductCategoryID == x.ProductCategoryID).Select(c =>
                //                        new ProductCategory
                //                        {
                //                            Id = c.ProductCategoryID,
                //                            ModifiedDate = c.ModifiedDate,
                //                            Name = c.Name
                //                        }).Single()
                //            }).Single(),
                ProductModelId = product.ProductModelID,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                DiscontinuedDate = product.DiscontinuedDate,
                ModifiedDate = product.ModifiedDate
            };
        }

        private UnitMeasure MapUnitMeasureFromDb(string unitMeasureCode)
        {
            return _entities.UnitMeasures.Where(x => x.UnitMeasureCode == unitMeasureCode)
                .Select(x =>
                    new UnitMeasure { Code = x.UnitMeasureCode, ModifiedDate = x.ModifiedDate, Name = x.Name })
                .Single();
        }

        private DbModel.Product MapProductForDb(Product product)
        {
            return new DbModel.Product
            {
                ProductID = product.Id,
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                MakeFlag = product.MakeFlag,
                FinishedGoodsFlag = product.FinishedGoodsFlag,
                Color = product.Color,
                SafetyStockLevel = product.SafetyStockLevel,
                ReorderPoint = product.ReorderPoint,
                StandardCost = product.StandardCost,
                ListPrice = product.ListPrice,
                Size = product.Size,
                SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                Weight = product.Weight,
                DaysToManufacture = product.DaysToManufacture,
                ProductLine = product.ProductLine,
                Class = product.Class,
                Style = product.Style,
                ProductSubcategoryID = product.ProductSubcategoryId,
                ProductModelID = product.ProductModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                DiscontinuedDate = product.DiscontinuedDate,
                ModifiedDate = product.ModifiedDate
            };
        }
    }
}
