using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.ExternalApi.OMO91
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string LogId { get; set; }
        public object Data { get; set; }
    }

    // GetMemberInfo
    public class GetMemberInfoRequest
    {
        public string OuterMemberCode { get; set; }
        public string MemberCode { get; set; }
        public string CellPhone { get; set; }
    }

    public class GetMemberInfoResponse
    {
        public string OuterMemberCode { get; set; }
        public string MemberCode { get; set; }
        public string CellPhone { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredDateTime { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string OmniPadBarcode { get; set; }
        public string LocationCountry { get; set; }
        public string LocationState { get; set; }
        public string LocationCity { get; set; }
        public string LocationDistrict { get; set; }
        public string LocationAddress { get; set; }
        public string LocationZipCode { get; set; }
        public string MemberTier { get; set; }
        public int MemberTierLevel { get; set; }
        public float CurrentLevelTradeSum { get; set; }
        public float TotalTradeSum { get; set; }
        public DateTime MemberTierEndDateTime { get; set; }

    }

    // GetMemberPoint
    public class GetMemberPointRequest
    {
        public string OuterMemberCode { get; set; }
        public string MemberCode { get; set; }
        public string CellPhone { get; set; }
    }

    public class RedeemPointProfile
    {
        public decimal Point { get; set; }
        public decimal Amount { get; set; }
        public decimal ThresholdAmount { get; set; }
    }
    public class GetMemberPointResponse
    {
        public decimal TotalBalancePoint { get; set; }
        public DateTime? PointExpireDate { get; set; }
        public RedeemPointProfile RedeemPointProfile { get; set; }
    }

    // GetMemberOwnCoupon
    public class GetMemberOwnCouponRequest
    {
        public string OuterMemberCode { get; set; }
        public string MemberCode { get; set; }
        public string CellPhone { get; set; }
    }

    public class GetMemberOwnCouponResponse
    {
        public int CouponId { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public List<string> CouponCustomCode { get; set; }
        public DateTime CouponUsingEndDateTime { get; set; }
        public List<string> PlatformDrawOuts { get; set; }
        public string DiscountType { get; set; }
        public float DiscountPercent { get; set; }
        public float DiscountPrice { get; set; }
        public bool IsUsingMinPrice { get; set; }
        public bool IsMaxDiscountLimit { get; set; }
        public bool IsForOnline { get; set; }
        public bool IsForOffline { get; set; }
    }

    // SetOuterMemberId
    public class SetOuterMemberIdRequest
    {
        public string OuterMemberCode { get; set; }
        public string CellPhone { get; set; }
    }

    public class SetOuterMemberIdResponse
    {
        public string OuterMemberCode { get; set; }
        public string CellPhone { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredDateTime { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string OmniPadBarcode { get; set; }
        public string MemberTier { get; set; }
        public float CurrentLevelTradeSum { get; set; }
        public float TotalTradeSum { get; set; }
        public DateTime MemberTierEndDateTime { get; set; }
    }

    // CreateOrderPOSAction

    public class OrderCode
    {
        public string OuterCode1 { get; set; }
        public string OuterCode2 { get; set; }
        public string OuterCode3 { get; set; }
        public string OuterCode4 { get; set; }
        public string OuterCode5 { get; set; }

    }
    public class CreateOrderPOSActionRequest
    {
        public string OuterMemberCode { get; set; }
        public OrderCode OrderCode { get; set; }
        public decimal Point { get; set; }
        public List<string> CouponCodes { get; set; }
    }

    public class CreateOrderPOSActionResponse
    {
        public string ActionId { get; set; }
    }

    //ReturnOrderPOSAction
    public class ReturnOrderPOSActionRequest
    {
        public string OuterMemberCode { get; set; }
        public string RelativeActionId { get; set; }
        public OrderCode ReturnOrderCode { get; set; }
        public decimal ReturnPoint { get; set; }
        public List<string> ReturnCouponCodes { get; set; }
    }

    public class ReturnOrderPOSActionResponse
    {
        public string ActionId { get; set; }
    }

}
