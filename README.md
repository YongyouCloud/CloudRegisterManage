# 云注册管理
## 简介
> 云注册管理功能是 U8V15.0 版本新增功能，集许可证书管理、许可分配、许可占用情况查询等功能为一体，方便管理员对产品授权进行管理。

## 安装
> 安装 U8 时，如果安装模式为经典模式下的全产品、服务器安装选择“加密服务器”、自定义
安装选择“加密服务器”、全产品集中应用模式（U8 远程），安装后在当前机器的开始菜单
中的 U8 目录下，能够看到云注册管理功能。
## 注册产品
> 点击按钮【注册产品】，打开友户通登录界面，登录后可进行产品注册激活。
## 查看特征码
> 特征码信息是从本机关键硬件信息中提取，用于产品注册时将加密服务器和产品进行绑定。在此界面支持查看、复制、导出当前加密服务器的特征码。
## 导入许可
> 通过按钮【选择文件】，选择将友户通上注册生成并下载的许可文件导入加密服务器，从而获取购买的授权信息。\
许可文件导入成功后，点击按钮【查看许可详情】，跳转至“查看许可占用情况” 功能，在此界面能够看到当前产品购买的授权信息。\
许可文件导入成功后，当前界面会显示与导入许可有关的信息，具体如下：
- 加密版本：显示当前加密的版本信息。
- 加密类型：显示当前加密的类型。
- 绑定的特征码：显示当前加密注册时绑定的特征码信息。
- 证书有效期：显示加密证书的到期日期。
- 服务识别码：显示当前加密的服务识别码信息。
- 产品卡号：显示当前加密的产品卡号。
- 公司名称：显示购买该加密的客户公司名称。
- 所属企业帐号 ID： 显示当前加密所属企业帐号的 ID。
- 证书更新日期：显示当前加密证书更新的时间。
- 云端证书校验：
  - 校验计划：显示云端每日校验加密证书的时间。
  - 最近校验： 显示最近一次云端校验加密证书的日期和校验的结果。
  - 当最近校验结果为“失败”或当前已经进入宽限期，会显示警告提示“由于您的证书在云端校验不通过……”
- 加密重注册日期： 显示离线加密下次需要到友户通重注册加密的日期。
  - 已过加密重注册日期但未更新加密证书时，会显示警告提示“由于您未按时在云端重注册加密……”

> 如果当前环境使用的是在线加密，点击按钮【在线同步许可】，系统根据本地加密证书中记录的产品信息， 自动从友户通获取最新用户信息、用户许可分配关系等。

## 分配许可

> 按用户注册制控制的产品， 需要将授权许可分配给具体用户。 只有使用离线加密的客户，才能使用此功能。 使用在线加密的客户，在友户通上注册加密时，完成用户许可分配后方能生成加密证书。

在此界面支持按照手机号码、操作员信息对所选领域分组中的用户进行搜索。
```
【保存】保存设置的用户许可分配关系，并会做相关合法性校验。
【取消】 放弃在当前界面所做的操作。
【选择用户】从用户对照表中选择具体用户，并将其添加至目标领域分组中，使其具有该领
域分组的授权许可。
【移除用户】 将目标用户从当前领域分组中移除，使其不再具有该领域分组的授权许可。
```
## 备份许可
- 点击按钮【备份许可】，备份当前加密服务器上的许可相关文件。\
- 点击按钮【恢复备份】，选择备份的加密证书进行备份恢复。 

## 查看许可占用情况
 在此界面能够获取以下信息：
- 当前环境所用加密的类型与加密的版本。
- 当前客户购买的许可信息。按用户注册制控制的产品、非用户注册制控制的产品、插件等许可信息均能在此查询到。
- 各个子产品当前的已登录数、登录的操作员信息、登录的模块、登录的门户、登录时所用客户端的信息等。

## 用户对照表

> 显示 U8 操作员与友户通账号的对应关系。\
在此界面支持按照手机号码、操作员信息对列表中的用户进行搜索。\
支持导出列表中的用户记录。

## 自助服务
> 通过扫描二维码或点击链接，进入服务圈获取相关服务。