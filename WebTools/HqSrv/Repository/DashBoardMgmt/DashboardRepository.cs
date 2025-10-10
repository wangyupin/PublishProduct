using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.DashBoard;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Azure.Core;

namespace HqSrv.Repository.DashBoardMgmt
{
    public class DashboardRepository
    {
        private readonly POVWebDbContextDapper _context;
        public DashboardRepository(POVWebDbContextDapper context)
        {
            _context = context;
        }

        public async Task<(bool, string)> CheckInvoiceData(CheckInvoiceRequest request)
        {
            string checkStr = @"
                Select SellBranch, TerminalID, SellStore, InvoiceID, MinInvoice, MaxInvoice
                From POVWeb.MachineSet
                Where SellBranch = @SellBranch
	               And SellStore = @SellStore
	               And TerminalID = @terminalID
                   And InvTerm = @InvTerm
                   And MinInvoice <> ''
                   And MaxInvoice <> ''
                   And InvoiceID <> ''
                   And InvoiceID <> '***'
                   And InvoiceID <> '*'
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(checkStr, request, commandTimeout: 180);
                    if (result.ToList().Count != 0)
                    {
                        return (true, message);
                    }
                    else
                    {
                        return (false, message);
                    }

                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (false, message);
                }
            }
        }

        public async Task<(bool, string)> GetInvoiceData(GetInvoiceRequest request)
        {
            string getInvoiceStr = @"
                Begin Transaction
                Declare @MinInvoice varchar(100);
                Declare @MaxInvoice varchar(100);
                Declare @MachineNo varchar(100);
                Declare @MachineID varchar(100);
                Declare @AppID varchar(100);
                Declare @qrApiKey varchar(100);
                Declare @OBU varchar(100);
                Declare @Store varchar(100);
                Declare @ErrorMessage bit = 0;
                Declare @ExistingInvTerm bit = 0; -- 新增变量用于检查是否存在相同的InvTerm

                -- 检查MachineSet表中是否已有相同的InvTerm
                Select @ExistingInvTerm = 1
                From POVWeb.MachineSet
                Where SellBranch = @StoreID
                    And SellStore = @StoreID
                    And TerminalID = @TerminalID
                    And InvTerm = @InvTerm
                    And InvoiceID <> '***'

                -- 如果不存在相同的InvTerm，则继续执行
                IF @ExistingInvTerm = 0
                Begin
                    Select Top 1
                        @MinInvoice = MinInvoice,
                        @MaxInvoice = MaxInvoice,
                        @AppID = AppID,
                        @qrApiKey = qrApiKey,
                        @OBU = OBU
                    From InvoiceSet With (UpdLock, RowLock, ReadPast)
                    Where Store = @StoreID
                        And InvTerm = @InvTerm
                        And MachineNo = ''
                        And MachineID = ''
                    Order By MinInvoice, MaxInvoice Asc
    
                    IF @@ROWCOUNT > 0
                    Begin
                        Update InvoiceSet
                        Set	MachineID = @TerminalID,
                            MachineNo = '*'
                        Where Store = @StoreID 
                            And InvTerm = @InvTerm
                            And MinInvoice = @MinInvoice
                            And MaxInvoice = @MaxInvoice
                            And MachineNo = ''
                            And MachineID = ''
            
                        Update POVWeb.MachineSet
                        Set InvoiceID = @OBU + @MinInvoice,
                            MinInvoice = @OBU + @MinInvoice,
                            MaxInvoice = @OBU + @MaxInvoice,
                            InvTerm = @InvTerm,
                            AppID = @AppID,
                            qrApiKey = @qrApiKey
                        Where SellBranch = @StoreID
                            And SellStore = @StoreID
                            And TerminalID = @TerminalID
                    End
                    Else
                    Begin
                        Set @ErrorMessage = 1;
                        Update POVWeb.MachineSet
                        Set InvoiceID = '',
                            MinInvoice = '',
                            MaxInvoice = '',
                            InvTerm = '',
                            AppID = '',
                            qrApiKey = ''
                        Where SellBranch = @StoreID
                            And SellStore = @StoreID
                            And TerminalID = @TerminalID
                    End
                End

                Commit Transaction

                IF @ErrorMessage = 1
                Begin;
                    THROW 51000, '找不到可用的發票，請重新分配', 1;
                End
            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(getInvoiceStr, request, commandTimeout: 500);
                    return (true, message);

                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (false, message);
                }
            }
        }
    }
}
