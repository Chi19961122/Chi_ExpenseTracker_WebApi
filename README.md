Last updated: 20260710

# ExpenseTracker

* 網頁連結： [Chi_ExpenseTracker](https://chiexpensetracker.netlify.app/login)

* 可自行註冊帳戶，或是使用下方測試帳號登入

    * Test account：Test@example.com

    * Password：123456

p.s. 若遇到初次登入等候逾時失敗，可再嘗試一次。此因等待Azure伺服器啟動關係。

# Change Log

* **2026-07-10 全面重構**：專案改版為 `Chi.ExpenseTracker.*` 新架構（.NET 10、`src/`+`tests/` 分層、JWT 身分隔離、RESTful API）。
  新舊差異、API 對照表與部署設定變更，詳見 **[docs/Refactor-20260710.md](docs/Refactor-20260710.md)**。
  ⚠️ API 為 breaking change，前端需依對照表改接；既有資料庫與帳號資料完全相容。

# Introduction

### [簡介]

Chi_ExpenseTracker 是一個專為個人設計的簡單記帳系統，

希望幫助用戶輕鬆追蹤和管理日常財務活動。

### [主要功能]

* 總覽頁：日期區間的總計金額與報表，快速了解近期收支狀況。

* 交易頁：支出和收入記錄，支持多種分類。

* 類別頁：可設定支出與收入的分類，方便用戶管理。

### [技術簡介]

Chi.ExpenseTracker 以 .NET 10 為核心技術，

採用 Entity Framework Core 進行資料存取，

並使用 Microsoft SQL Server 作為後端資料庫。

系統架構上採用分層設計，將應用程式分為 API 接口層、服務邏輯層、資料存取層與共用模型層，

以內建 DI 搭配建構子注入，並以 JWT（sub claim）識別使用者、隔離各使用者的資料。

# Tech Stack

### Core

* ASP.NET Core Web API (.NET 10)

### ORM

* Entity Framework Core 10 (DB First)

### DB

* Microsoft SQL Server

### Tool

* Auth：JWT Bearer Authentication
* Encryption：BCrypt.Net 4.0.3
* Mapping：AutoMapper 15
* Validation：FluentValidation 11
* Test：xUnit + NSubstitute + FluentAssertions

# Information

### API 接口層（src/Chi.ExpenseTracker.Api）

    Api接口 : Controllers\（ApiControllerBase 提供 CurrentUserId，自 JWT sub claim 取得）

    傳入參數與驗證 : Models\Parameters\ + Validators\（FluentValidation）

    傳出模型 : Models\ViewModels\

    統一例外處理 : Middleware\ExceptionHandlingMiddleware.cs（回傳 ProblemDetails）

    appsettings.json : 參數設定檔(預設)
        appsettings.Development.json : 本機參數設定檔(環境變數ASPNETCORE_ENVIRONMENT=Development)
        機密（連線字串、Jwt:Key）請使用 User Secrets 或環境變數，不入版控

    Program.cs : 程式啟動進入點、服務註冊

### 服務邏輯層（src/Chi.ExpenseTracker.Services）

    邏輯服務、服務接口 : Auth\、Categories\、Transactions\

    Entity → DTO 對應 : Mapping\ServiceMappingProfile.cs

    DI 註冊 : DependencyInjection\ServiceCollectionExtensions.cs

### 資料存取層（src/Chi.ExpenseTracker.Repositories）

    DbContext : Data\ExpenseDbContext.cs

    資料庫表格Entity : Entities\

    資料存取 : Users\、Categories\、Transactions\（依聚合分資料夾）

    DI 註冊 : DependencyInjection\RepositoryServiceCollectionExtensions.cs

### 共用模型層（src/Chi.ExpenseTracker.Common）

    Options 設定模型 : Options\JwtOptions.cs

    DTO 與服務輸入模型 : Models\Dtos\、Models\InfoModels\

### 單元測試（tests/Chi.ExpenseTracker.Tests）

    Auth\、Categories\、Transactions\ : 各 Service 的單元測試

# Future

* 分帳功能：多用戶共同管理同一個收支專案，實現分帳功能。
* 預算管理：幫助用戶制定和追蹤預算，避免超支。
* 報告分析：生成詳細的年、月、日財務報告，分析消費習慣，幫助用戶更好地管理財務。
* 報表下載：讓用戶可以將財務數據以Excel格式下載，以便進行進一步的分析和存檔。
