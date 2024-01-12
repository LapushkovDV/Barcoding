**Состав проекта по штрихкодированию:**
  - Мобильное приложение Android (Xamarin Forms (C# + XAML)) - TSD;
  - Админ-панель для управления ТСД (текущий) V1 (ASP.NET Core 2.1 (С#+HTML+CSS+JS)) - ERP_API_SERVICE;
  - Админ-панель для управления ТСД (будущий) V2 (ASP.NET 6 Blazor (С#+HTML+CSS)) - ERP_Admin_Panel;
  - Компоненты штрихкодирования для ERP (Язык программирования ERP - VIP) - C_Barcode и SHK_VIP;
  - Документы, связанные с проектом - Documents.

<details><summary><b>Структура репозитария:</b></summary>
<pre>
+---C_Barcode
|   +---doc
|   +---exe
|   +---font
|   +---table
|   \---vip
|       +---alter
|       +---menu
|       \---tune
+---Documents
|   +---DataBaseSource
|   +---ExampleData
|   +---Instructions
|   +---WorkDocuments
|   \---_EXMPL
+---ERP_Admin_Panel
|   \---ERP_Admin_Panel
|       +---CodePages
|       |   +---ComponentsRazor
|       |   |   +---Devices
|       |   |   +---Menus
|       |   |   +---Roles
|       |   |   \---Users
|       |   \---Shared
|       +---Data
|       |   \---ViewModels
|       +---Pages
|       |   \---ComponentsRazor
|       |       +---Devices
|       |       +---Menus
|       |       +---Roles
|       |       \---Users
|       +---Properties
|       +---Resources
|       |   \---Pages
|       |       \---ComponentsRazor
|       +---Services
|       |   +---Cryptography
|       |   +---Database
|       |   |   \---Models
|       |   +---Interfaces
|       |   +---Modal
|       |   +---StateProviders
|       |   +---Toast
|       |   \---Token
|       +---Shared
|       \---wwwroot
|           +---css
|           |   +---awesome
|           |   |   +---css
|           |   |   +---fonts
|           |   |   +---less
|           |   |   \---scss
|           |   +---bootstrap
|           |   \---open-iconic
|           |       \---font
|           |           +---css
|           |           \---fonts
|           +---fonts
|           |   \---Roboto
|           \---images
+---ERP_API_Service
|   +---ERP_API_Service
|   |   +---.config
|   |   +---Admin
|   |   +---Content
|   |   |   +---Images
|   |   |   \---Scripts
|   |   +---Controllers
|   |   +---Managers
|   |   +---Models
|   |   |   +---Auth
|   |   |   \---Json
|   |   +---Properties
|   |   |   \---PublishProfiles
|   |   +---Views
|   |   |   +---Admin
|   |   |   |   +---Event
|   |   |   |   +---Imei
|   |   |   |   +---Menu
|   |   |   |   +---Role
|   |   |   |   +---RoleMenu
|   |   |   |   +---Setting
|   |   |   |   +---User
|   |   |   |   +---UserImei
|   |   |   |   \---UserRole
|   |   |   \---Shared
|   |   \---WebObjects
|   +---_DB
|   \---_Publish
|       \---Content
|           +---Images
|           \---Scripts
+---SHK_VIP
|   +---font
|   +---fr3
|   +---lot
|   +---Source
|   \---vih
\---TSD
    +---TSD
    |   +---Model
    |   |   \---User
    |   +---Services
    |   |   +---Converters
    |   |   +---Cryptography
    |   |   +---DataBase
    |   |   |   +---Interfaces
    |   |   |   \---Tables
    |   |   +---DependencyProperties
    |   |   |   \---Behaviors
    |   |   +---Extensions
    |   |   +---Interfaces
    |   |   +---MarkupExtensions
    |   |   |   \---ValuesConverters
    |   |   +---Rest
    |   |   +---Scanner
    |   |   \---Tasks
    |   |       \---Models
    |   +---ViewModels
    |   \---Views
    |       +---FlyoutPages
    |       |   \---ContentViews
    |       \---Resources
    |           \---Resources_XML
    \---TSD.Android
        +---Assets
        +---Properties
        +---Resources
        |   +---drawable
        |   +---mipmap-anydpi-v26
        |   +---mipmap-hdpi
        |   +---mipmap-mdpi
        |   +---mipmap-xhdpi
        |   +---mipmap-xxhdpi
        |   +---mipmap-xxxhdpi
        |   +---values
        |   \---xml
        \---Services
            +---Database
            +---Dependency
            \---ElementsRenderer

</pre>
</details>

