# emud
## 介绍
* 全称EmpriseMud，基于netcore3.1开发的mud游戏框架。  
* 仅供学习交流技术使用，未经本人同意，不可用于任何商业用途。  
* 如果对你有帮助 请点右上角star，谢谢！  
* 作者qq ：   8720826
* 交流QQ群 ： <a target="_blank" href="//shang.qq.com/wpa/qunwpa?idkey=d1a2ea0e22e7dd1bdf325d2ff69b36004cc8f8f7ef643ab10a5aac257ad9a613">711894778</a>


## 演示
* 演示地址 ： https://emprise.cn/



####  ![登录页面](https://raw.githubusercontent.com/8720826/emud/master/doc/login.png)
####  ![创建角色](https://raw.githubusercontent.com/8720826/emud/master/doc/player.png)
####  ![游戏界面](https://raw.githubusercontent.com/8720826/emud/master/doc/game.png)

## 技术栈
* netcore3.1
* vue
* mysql/sqlserver/PostgreSql
* redis
* hangfire
* efcore
* signalr

## 如何使用
1.  下载代码  
    可以直接download或者git clone等方式获取代码到本地   

2.  编译  
    使用vs2019或者vscode（需要安装C#插件）
	
3.  配置文件  
    配置文件为appsettings.json，本地调试使用appsettings.Development.json  
    设置好游戏主程序和后台管理的数据库连接字符串等
    目前支持以下数据库：
    Mysql
    SqlServer
    PostgreSql
    
4.  发布  
    游戏主程序  ```Emprise.Web```  
	游戏后台管理 ```Emprise.Admin```  
	
	可以发布到IIS或其他web服务器
     
5.  安装  
	游戏免安装，无需数据库脚本  
	首次打开后台管理站点时候将自动创建数据库  
    打开后台管理登录页面，首次输入的账号密码即保存为管理账号  

##  相关下载
* Mysql   
https://www.mysql.com/downloads/

* SqlServer
https://www.microsoft.com/zh-cn/sql-server/sql-server-2019

* PostgreSql
https://www.postgresql.org/

* Redis  
https://redis.io/download

* Visual Studio    
https://visualstudio.microsoft.com/




	


    
