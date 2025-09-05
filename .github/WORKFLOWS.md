# GitHub Actions Workflows

本项目包含以下 GitHub Actions workflows：

## 🚀 Release Workflow (`release.yml`)

**用途**: 手动触发创建项目发布版本

**触发方式**:

- 在 GitHub 仓库的 Actions 页面手动触发
- 需要输入版本号（如 `v1.0.0`）
- 可选择是否标记为预发布版本

**功能**:

- 构建 Windows x64 和 x86 版本
- 运行所有测试
- 创建自包含的可执行文件
- 自动创建 GitHub Release
- 上传构建产物到 Release

**使用步骤**:

1. 进入 GitHub 仓库的 Actions 页面
2. 选择 "Create Release" workflow
3. 点击 "Run workflow"
4. 输入版本号（如 `v1.2.0`）
5. 选择是否为预发布版本
6. 点击 "Run workflow" 开始构建

## 🔧 CI Build Workflow (`dotnet-desktop.yml`)

**用途**: 持续集成构建和测试

**触发方式**:

- 推送到 `master` 或 `main` 分支
- 创建针对 `master` 或 `main` 分支的 Pull Request

**功能**:

- 自动构建项目（Debug 和 Release 配置）
- 运行所有单元测试
- 验证代码质量

## 🏷️ Auto Labeler Workflow (`labeler.yml`)

**用途**: 自动为 Issues 和 Pull Requests 添加标签

**触发方式**:

- 创建或编辑 Issue
- 创建、编辑或同步 Pull Request

**功能**:

- 根据内容自动添加相关标签
- 支持的标签类型：
  - `bug` - 错误报告
  - `enhancement` - 功能请求
  - `documentation` - 文档相关
  - `question` - 问题咨询
  - `performance` - 性能相关
  - `ui/ux` - 界面设计
  - `database` - 数据库相关
  - `configuration` - 配置相关
  - `good first issue` - 适合新手

## 📋 注意事项

1. **Release Workflow** 需要手动触发，确保在发布前所有功能都已测试完毕
2. **CI Workflow** 会在每次代码推送时自动运行，确保代码质量
3. **Labeler** 会根据 Issue/PR 的标题和内容自动添加标签，提高项目管理效率

## 🔧 自定义配置

- 修改 `.github/labeler.yml` 可以调整自动标签规则
- 修改 workflow 文件可以调整构建配置和触发条件
