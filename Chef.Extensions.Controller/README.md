## List of Features

### 以 Json.NET 來取代 JsonResult 的序列化元件

```csharp
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data);
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType);
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, Encoding contentEncoding);
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, JsonRequestBehavior behavior);
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, JsonRequestBehavior behavior);
public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior);
```
### 優先使用快取的 View 渲染結果

若網頁採用 SPA（Single Page Application）模型，為減少伺服器經常重複執行相同 HTML 結果的渲染程序，`CacheView()` 系列方法會將渲染結果快取一段時間 - `duration`（以秒為單位），並且計算渲染結果的 Checksum 做為 ETag 回應，渲染結果沒有異動則回應 304（NotModified）。

此外，當必須在快取時間到期前強制更新 HTML 渲染結果時，可在 Cache-Control 中加入 `must-refresh[=<seconds>]`，seconds 的作用是延長快取到期，作為一段緩衝時間。

```csharp
public static ActionResult CacheView(this System.Web.Mvc.Controller me, int duration = 900);
public static ActionResult CacheView(this System.Web.Mvc.Controller me, string viewName, int duration = 900);
public static ActionResult CacheView(this System.Web.Mvc.Controller me, object model, int duration = 900);
public static ActionResult CacheView(this System.Web.Mvc.Controller me, string viewName, object model, int duration = 900);
```