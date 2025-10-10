using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.ExternalApi
{
    public class GetTrackingNumber
    {
        public List<string> EC_OrderID { get; set; }
        public string Platform { get; set; }
    }
    public class GetMomoTrackingNumber: GetTrackingNumber
    {
        
    }
  
   
    
    public class MomoLoginInfo
    {
        public string EntpCode { get; set; }
        public string EntpID { get; set; }
        public string EntpPwd { get; set; }
        public string OtpBackNo { get; set; }

    }
    public class ECGeneralResponse
    {
        public string Status { get; set; }
        public ErrorItem Errors { get; set; } 

    }
    public class ErrorItem
    {
       
        public string EC_OrderID { get; set; }
        public string TransactionID { get; set; }
        public string ErrorMsg { get; set; }
    }
    public class TrackingNumberErrorResponse
    {
        public string PlatformID { get; set; }
        public string CollectionID { get; set; }
        public List<ErrorItem> ErrorItems { get; set; }
        public string Msg { get; set; }
    }
    public class MomoBasicRequest
    {
        public string DoAction { get; set; }
        public MomoLoginInfo LoginInfo { get; set; }
        public dynamic SendInfoList { get; set; }
    }
    public class MomoBasicCheck
    {
        public List<string> BasicCheckMsgList { get; set; }
        public string ReqString { get; set; }
        public string RepString { get; set; }
    }
    //店商併箱
    public class UnsendStoresCombineBox
    {
        public string CompleteOrderNo { get; set; }
        public string BoxYn { get; set; }
        public string Remark5VStr { get; set; }
    }

    public class UnsendStoresCombineBoxItem
    {
        public int UndoCnt { get; set; }
        public object[] UndoList { get; set; }
        public int ConfirmOkCnt { get; set; }
        public string[] ConfirmOkList { get; set; }
        public int ConfirmFailCnt { get; set; }
        public object[] ConfirmFailList { get; set; }
        public int ConfirmRepeatCnt { get; set; }
        public object[] ConfirmRepeatList { get; set; }
    }
    public class UnsendStoresCombineBoxReponse : MomoBasicCheck
    {
        public List<UnsendStoresCombineBoxItem> ResultInfo { get; set; }
    }

    public class UnsendStoresFinish
    {
        public string CompleteOrderNo { get; set; }
        public string Remark5VStr { get; set; }
        public string ShipTypeStr { get; set; }
        public string PackTypeStr { get; set; }
        public string PackUnit { get; set; }
    }
    public class UnsendStoresFinishItem
    {
        public int UndoCnt { get; set; }
        public object[] UndoList { get; set; }
        public int ConfirmOkCnt { get; set; }
        public string[] ConfirmOkList { get; set; }
        public int ConfirmFailCnt { get; set; }
        public object[] ConfirmFailList { get; set; }
        public int ConfirmRepeatCnt { get; set; }
        public object[] ConfirmRepeatList { get; set; }
    }
    public class UnsendStoresFinishReponse : MomoBasicCheck
    {
        public List<UnsendStoresFinishItem> ResultInfo { get; set; }
    }

    public class SendingStoresQueryReq
    {
        public string FromDate { get; set; }
        public string FromHour { get; set; }
        public string FromMinute { get; set; }
        public string ToDate { get; set; }
        public string ToHour { get; set; }
        public string ToMinute { get; set; }
        public string Status { get; set; }
        public string Dely_gb { get; set; }
        public string OrderNo { get; set; } //只查一筆單的時候
    }
    public class SendingStoresQuery
    {
        public string Slip_no { get; set; }
        public string CompleteOrderNo { get; set; }
    }
    public class SendingStoresQueryReponse : MomoBasicCheck
    {
        public List<SendingStoresQuery> DataList { get; set; }
    }

   
    public class SendingThirdQueryReq
    {
        public string FromDate { get; set; }
        public string FromHour { get; set; }
        public string FromMinute { get; set; }
        public string ToDate { get; set; }
        public string ToHour { get; set; }
        public string ToMinute { get; set; }
        public string Status { get; set; }
        public string OrderNo { get; set; } //只查一筆單的時候
        public string Logistics { get; set; }
    }

    public class SendingThirdQuery
    {
        public string Slip_no { get; set; }
        public string CompleteOrderNo { get; set; }
    }

    public class SendingThirdQueryReponse : MomoBasicCheck
    {
        public List<SendingThirdQuery> DataList { get; set; }
    }


    public class UnsendCompanyConfirm
    {
        public string CompleteOrderNo { get; set; }
        public string Remark5VStr { get; set; }
        public string MsgNote { get; set; }
        public string DelyGbStr { get; set; }
        public string SlipNo { get; set; }
        public string ShipTypeStr { get; set; }
        public string PackTypeStr { get; set; }
        public string PackUnit { get; set; }
    }
    public class UnsendCompanyConfirmReponse : MomoBasicCheck
    {
        public int UndoCnt { get; set; }
        public object[] UndoList { get; set; }
        public int ConfirmOkCnt { get; set; }
        public string[] ConfirmOkList { get; set; }
        public int ConfirmFailCnt { get; set; }
        public object[] ConfirmFailList { get; set; }
        public int ConfirmRepeatCnt { get; set; }
        public object[] ConfirmRepeatList { get; set; }
    }

}
