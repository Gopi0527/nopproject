﻿using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Mapping;

public interface IProductMappingService
{
    public Product GetNopProductEntity(ApiProductModel model);
    public ProductAttribute GetNopProductAttributeEntity(ApiProductAttributeModel productAttribute);

    public ProductAttributeValue GetNopProductAttributeValueEntity(ApiProductAttributeValueModel productAttributeValue,
        ProductAttribute nopProductAttribute, Product nopProduct);

    public ProductAttributeMapping GetNopProductAttributeMappingEntity(ProductAttribute nopProductAttribute,
        Product nopProduct, ApiProductAttributeModel productAttribute);

    public SpecificationAttributeOption GetNopSpecificationAttributeOption(ApiProductModel model,
        int specificationAttributeId);

    public ProductSpecificationAttribute GetNopProductSpecificationAttribute(Product nopProduct,
        SpecificationAttributeOption nopSpecificationAttributeOption);
}