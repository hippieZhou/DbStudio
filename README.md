# DbStudio

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-5.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/5.0)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)

**DbStudio** 是一款基于 .NET 5 和 WPF 技术栈开发的现代化数据库管理工具。采用清洁架构（Clean Architecture）设计模式，提供直观的用户界面和强大的数据库管理功能。

## ✨ 核心特性

- 🔗 **连接管理** - 支持多种数据库连接，历史连接记录管理
- 📊 **数据库监控** - 实时数据库信息查看和性能监控
- 💾 **备份与还原** - 完整的数据库备份和还原解决方案
- 📸 **快照管理** - 数据库快照创建、管理和恢复功能
- 🏗️ **索引重建** - 数据库索引优化和重建工具
- 📈 **进度跟踪** - 长时间操作的实时进度显示

## 🏗️ 技术架构

项目采用分层架构设计，确保代码的可维护性和可扩展性：

```
├── DbStudio.Domain          # 领域层 - 核心业务实体
├── DbStudio.Application     # 应用层 - 业务逻辑和用例
├── DbStudio.Infrastructure  # 基础设施层 - 数据访问和外部服务
├── DbStudio.Infrastructure.Shared # 共享基础设施组件
└── DbStudio.WpfApp         # 表示层 - WPF 用户界面
```

### 核心技术栈

- **框架**: .NET 5.0 / WPF
- **架构模式**: Clean Architecture + CQRS + MediatR
- **UI 框架**: HandyControl + CommunityToolkit.Mvvm
- **数据访问**: Dapper + LiteDB
- **日志记录**: Serilog
- **对象映射**: Mapster
- **验证**: FluentValidation
- **依赖注入**: Microsoft.Extensions.DependencyInjection

## 🚀 快速开始

### 系统要求

- Windows 10 或更高版本
- .NET 5.0 Runtime 或更高版本

### 安装与运行

1. **克隆项目**

   ```bash
   git clone https://github.com/your-username/DbStudio.git
   cd DbStudio
   ```

2. **构建项目**

   ```bash
   dotnet build DbStudio.sln
   ```

3. **运行应用程序**
   ```bash
   dotnet run --project src/DbStudio.WpfApp
   ```

### 发布部署

生成独立可执行文件：

```bash
cd src
dotnet publish -c Release --self-contained -r win-x64 DbStudio.WpfApp
```

## 📸 应用截图

### 连接管理

- **历史连接** - 快速访问之前使用过的数据库连接

  ![历史连接](/images/Snipaste_2022-01-14_13-56-48.png)

- **新建连接** - 配置新的数据库连接参数

  ![新建连接](/images/Snipaste_2022-01-14_13-57-21.png)

### 数据库操作

- **快照管理** - 创建和管理数据库快照

  ![快照相关](/images/Snipaste_2022-01-14_13-57-35.png)

- **备份还原** - 数据库备份和还原操作界面

  ![备份还原相关](/images/Snipaste_2022-01-14_13-57-53.png)

- **数据库信息** - 查看数据库详细信息和统计数据

  ![数据库信息相关](/images/Snipaste_2022-01-14_13-58-02.png)

## 🛠️ 开发指南

### 项目结构说明

- **Domain Layer** (`DbStudio.Domain`): 包含核心业务实体和领域逻辑
- **Application Layer** (`DbStudio.Application`): 实现业务用例，使用 CQRS 模式和 MediatR
- **Infrastructure Layer** (`DbStudio.Infrastructure`): 数据访问、外部服务集成
- **Presentation Layer** (`DbStudio.WpfApp`): WPF 用户界面，使用 MVVM 模式

### 开发环境设置

1. 安装 Visual Studio 2019/2022 或 Visual Studio Code
2. 安装 .NET 5.0 SDK
3. 推荐安装的扩展：
   - C# for Visual Studio Code
   - .NET Core Tools

### 代码规范

- 遵循 C# 编码规范
- 使用依赖注入进行组件解耦
- 单元测试覆盖核心业务逻辑
- 使用异步编程模式处理 I/O 操作

## 🤝 贡献指南

我们欢迎社区贡献！请遵循以下步骤：

1. Fork 本项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📄 许可证

本项目基于 MIT 许可证开源 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [Microsoft GitHub Actions for Desktop Apps](https://github.com/microsoft/github-actions-for-desktop-apps)
- [WPF Tutorials by SingletonSean](https://github.com/SingletonSean/wpf-tutorials)
- [Building .NET Framework Apps using GitHub Actions](https://timheuer.com/blog/building-net-framework-apps-using-github-actions/)

## 📞 联系方式

- 作者: hippieZhou
- 项目链接: [https://github.com/hippiezhou/DbStudio](https://github.com/hippiezhou/DbStudio)

---

⭐ 如果这个项目对您有帮助，请给我们一个 Star！
