# DbStudio

> 数据库管理工具

# 截图

- 历史链接

![历史链接](/images/Snipaste_2022-01-14_13-56-48.png)

- 新建连接

![新建连接](/images/Snipaste_2022-01-14_13-57-21.png)

- 快照相关

![快照相关](/images/Snipaste_2022-01-14_13-57-35.png)

- 备份还原相关

![备份还原相关](/images/Snipaste_2022-01-14_13-57-53.png)

- 数据库信息相关

![数据库信息相关](/images/Snipaste_2022-01-14_13-58-02.png)

# 发布

```bash
cd src
dotnet publish -c release --self-contained -r win-x64 DbStudio.WpfApp
```