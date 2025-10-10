using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.ExternalApi
{
    public class Bsic91AppResponse
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime? TimeStamp { get; set; }
    }
    public class ShippingOrderNumber
    {
        public long ShopId { get; set; }
        public string TMCode { get; set; }
        public List<string> TSCodeList { get; set; }
    }
    public class ShippingOrderNumberData
    {
        public string TGCode { get; set; }
        public string TMCode { get; set; }
        public List<string> TSCodeList { get; set; }
        public string OrderDeliverType { get; set; }
        public string DistributorDef { get; set; }
        public string ShippingOrderCode { get; set; }
        public string ShippingOrderStatus { get; set; }
        public string OrderReceiverName { get; set; }
        public string OrderReceiverStoreId { get; set; }
        public string OrderReceiverStoreName { get; set; }
    }

    public class ShippingOrderNumberResponse : Bsic91AppResponse
    {
        public ShippingOrderNumberData Data { get; set; }
    }

    public class ShippingOrderConfirmResponse : Bsic91AppResponse
    {
        public string Data { get; set; }
    }

    public class ShippingOrderConfirm
    {
        public long ShopId { get; set; }
        public string ShippingOrderCode { get; set; }
        public string OrderDeliverType { get; set; }
    }

    public class AddTrackingNumber
    {
        public string EC_OrderID { get; set; }
        public List<string> TSCode { get; set; }
        public string CollectionID { get; set; }
        public string TrackingNumber { get; set; }

    }

    public class ThirdShippingOrderNumberData
    {
        public string TGCode { get; set; }
        public string TMCode { get; set; }
        public List<string> TSCodeList { get; set; }
        public string OrderDeliverType { get; set; }
        public string FulfillmentCode { get; set; }
        public string OrderReceiverName { get; set; }
        public string OrderReceiverAddress { get; set; }
        public string TemperatureTypeDef { get; set; }
    }

    public class ThirdShippingOrderNumberResponse : Bsic91AppResponse
    {
        public ThirdShippingOrderNumberData Data { get; set; }
    }

    public class ThirdShippingOrderNumber
    {
        public long ShopId { get; set; }
        public string TMCode { get; set; }
        public List<string> TSCodeList { get; set; }

        public string ForwarderDef { get; set; }
    }

    public class ThirdShippingOrderConfirm
    {
        public long ShopId { get; set; }
        public string FulfillmentCode { get; set; }
    }

    public class ThirdShippingOrderConfirmResponse : Bsic91AppResponse
    {
        public string Data { get; set; }
    }

    public class HomeShippingOrderConfirm
    {
        public long ShopId { get; set; }
        public string TMCode { get; set; }
        public List<string> TSCodeList { get; set; }

        public int ForwarderDef { get; set; }
        public string ShippingOrderCode { get; set; }
    }
    public class HomeShippingOrderConfirmResponse : Bsic91AppResponse
    {
        public object Data { get; set; }
    }

    public class CancelShippingOrderCode
    {
        public long ShopId { get; set; }
        public string ShippingOrderCode { get; set; }
        public string OrderDeliverType { get; set; }
    }

    public class CancelShippingOrderCodeResponse : Bsic91AppResponse
    {
        public string Data { get; set; }
    }

    public class CancelShipping
    {
        public long ShopId { get; set; }
        public string FulfillmentCode { get; set; }
    }

    public class CancelShippingResponse : Bsic91AppResponse
    {
        public string Data { get; set; }
    }

    public class Credential
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Token { get; set; }
        public string ExpiredTs { get; set; }
        public bool IsExpired => DateTime.Parse(ExpiredTs) <= DateTime.Now;
    }

    public class Object
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string ExpiredTs { get; set; }
        public string CreatedTs { get; set; }
        public string UserId { get; set; }
    }
}
