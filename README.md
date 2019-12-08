# emud
## 介绍
* 全称EmpriseMud，基于netcore3.1开发的mud游戏框架。
* 仅供学习交流技术使用，未经本人同意，不可用于任何商业用途。
* 如果对你有帮助 请点右上角star，谢谢！
## 技术栈
* netcore3.1
* vue
* mysql
* redis
* hangfire
* efcore
* signalr

## 如何使用
1. 下载代码
    可以直接download或者git clone等方式获取代码到本地
    
2. 编译
    使用vs2019或者vscode（需要安装C#插件）
    下载地址 https://visualstudio.microsoft.com
    
3. 发布
    选择Emprise.web，可以发布到IIS或其他web服务器
    
4. 配置文件
    配置文件为appsettings.json，本地调试使用appsettings.Development.json
    
5. 后台管理

    源码地址：https://github.com/8720826/emud.admin

    以同样方式获取代码后，修改配置文件

    设置数据库连接字符串等，编译运行，将自动创建数据库

    打开后台管理登录页面，首次输入的账号密码即保存为管理账号

    
