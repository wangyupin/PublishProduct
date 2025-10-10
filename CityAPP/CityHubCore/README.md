# CityHubCore 說明
提供ASP.NET Core 5.0 相關共用類別, 架構參考Clean Architecture 與 D.D.D. 
Ref: https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/?blogsub=confirming#fundamental-blocks-of-the-system

# 開發準則：
	* 每個目錄代表一層Layer，UI類獨自一個專案
	* 上層不依賴下層
	* 跨層使用 DI (Dependency Injection)

# Layer / Folder Content Definition
## 1. Domain
	- 負責保管Biz內容、status and rules。是架構的核心。可參照同層，但不可依賴其它層。
	- 如：存放Biz類的Class(Models, Enums, Exceptions, Repository Interface)
### Subfolder
	\Common 通用類，或不在分類內
	\InfraInterface 需要Infra實作的Interface
	\Enums
	\Exceptions
	\BasicEntities 基本設定檔，不需因交易後而結帳的項目：如User, Product, Catalog, 
	\TradeEntities 交易類檔，如Order, OrderDetail, OrderList 交易後，需要結帳的項目。
	\SummaryEntities 結算類檔，如OrderSummaryDaily, OrderSummaryMonthly, 

## 2. Application 
	- 定義需要Infrastucture.Interface讓Infra實作, 如Mail, SMS, Cache, 外部API (電商平台/電子發票)。
	- 使用Domain.Class或Infrastucture.Interface來實作使用案例。
	- 以Component概念：同一目錄存放Use Case Interface、實作、或相關的Behaviours，Vaildator。
	- 如 Notify use case會有
			INotifyService(Interface, 讓UI使用)
			NotifyService(實作, 實作中會使用到Infrastucture.Interface來完成流程。)
			NotifyServiceVaildator(實作Vaildator)
###	Subfolder
	\Common 通用類，或不在分類內，如Behaviours, Exception, Attributes
	\InfraInterface 需要Infra實作的Interface
	\{Use case}
	\ECSrv 先將ECSrv會用到的放於此

## 3. Infrastucture (Infra) `主要處理需外部資源的項目`
	- 實作Domain與Application需要的Interface
	- 如Mail, SMS, Cache, 外部API (電商平台/電子發票)
###	Subfolder
	\Domain
	\Application
	\Common 通用類，或不在分類內

## 4. User Interface (UI) `主要處理讓外部Input的項目`
	- 各自專案表述，如ECWeb...
	- 如Web Portal (User/Consumers)、API Service (3rd party apps)、Console commands (con jobs)
