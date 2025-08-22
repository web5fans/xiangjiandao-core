# Xiangjiandao

## 环境准备

```shell
# mysql 数据库
docker run --restart always --name mysql -v /mnt/d/docker/mysql/data:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=123456 -p 3306:3306 -d mysql:latest

# rabbitmq 消息队列
docker run --restart always -d --hostname node1 --name rabbitmq -p 15672:15672 -p 5672:5672 rabbitmq:3-management

# redis 缓存分布式锁
docker run --restart always --name redis -v /mnt/d/docker/redis:/data -p 6379:6379 -d redis:5.0.7 redis-server
```

## 依赖对框架与组件

+ [NetCorePal Cloud Framework](https://github.com/netcorepal/netcorepal-cloud-framework)
+ [ASP.NET Core](https://github.com/dotnet/aspnetcore)
+ [EFCore](https://github.com/dotnet/efcore)
+ [CAP](https://github.com/dotnetcore/CAP)
+ [MediatR](https://github.com/jbogard/MediatR)
+ [FluentValidation](https://docs.fluentvalidation.net/en/latest)
+ [Swashbuckle.AspNetCore.Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## 数据库

项目使用的数据库 schema 保存在项目根目录的 `database.sql` 中，使用如下命名可以生成增量更新的数据 schema 更新脚本

```shell
# 安装工具  SEE： https://learn.microsoft.com/zh-cn/ef/core/cli/dotnet#installing-the-tools
dotnet tool install --global dotnet-ef --version 9.0.0

# 强制更新数据库
dotnet ef database update -p src/Xiangjiandao.Infrastructure 

# 创建迁移 SEE：https://learn.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
# 注意： version 应当替换成当前正在开发的版本号
dotnet ef migrations add version -p src/Xiangjiandao.Infrastructure 

# 生成增量更新的数据库脚本
dotnet ef migrations script -p src/Xiangjiandao.Infrastructure  -o a.sql
```

## 项目部署

### 环境变量

```shell
# 需要使用私有镜像仓库，请自行配置, 以下配置只是一个示例
# 仓库地址
DOCKER_REPO="custom.image.repository"
# docker 用户名
REPOSITORY_USERNAME="username"
# docker 密码
REPOSITORY_PASSWORD="password"
# 镜像名称
IMAGE_NAME="image_name"
# 镜像 Tag
IMAGE_TAG="image_tag"
# Helm 仓库地址
HELM_REPO="custom.helm.repository"
# Helm 用户名
HELM_REPOSITORY_USERNAME="username"
# Helm 密码
HELM_REPOSITORY_PASSWORD="password"
# Chart 包名称
CHART_NAME="chart_name"
# Chart 包版本
CHART_VERSION="chart_version"
# Chart 包压缩包的文件名称
CHART_TGZ="$CHART_NAME-$CHART_VERSION.tgz"
# Chart 包安装的应用名称
PROGRAM_NAME="program_name"
# 灰度名称，默认空字符串
GRAY_NAME=""
# Helm 发布名称
HELM_DEPLOY_NAME="helm_deploy_name"
# Value 配置文件
VALUES="values.yaml"
# Kubeconfig 集群配置
KUBE_CONFIG="kube-config.yaml"
```

### 构建

```shell
# 登陆镜像仓库
echo "$REPOSITORY_PASSWORD" | docker login $DOCKER_REPO -u "$REPOSITORY_USERNAME" --password-stdin

# 进入到项目根目录
cd /path/to/xiangjiandao-core

# 使用 docker 构建镜像, 注意替换成预期 image-nmae 以及 image-tag，下同
docker buildx build -f src/Xiangjiandao.Web/Dockerfile -t $IMAGE_NAME:$IMAGE_TAG .

# 推送镜像到仓库
docker push $IMAGE_NAME:$IMAGE_TAG
```

### 打包 Helm-Chart 包

````shell
# 进入到项目根目录下的 `helm-chart/xiangjiandao` 目录
cd /path/to/xiangjiandao/helm-chart/xiangjiandao

# 登录到 Helm 仓库
echo "$HELM_REPOSITORY_PASSWORD" | helm repo add $PROGRAM_NAME $HELM_REPO --username "$HELM_REPOSITORY_USERNAME" --password-stdin

# 打包 Chart 包
helm package . --app-version=$IMAGE_TAG --version=$CHART_VERSION

# 推送到 Helm 仓库
helm cm-push $CHART_TGZ $PROGRAM_NAME
````

### 发布

```shell
# 登录 Helm 仓库
echo "$HELM_REPOSITORY_PASSWORD" | helm repo add $PROGRAM_NAME $CHART_REPO --username "$HELM_REPOSITORY_PASSWORD" --password-stdin

# 进入到项目根目录
cd /path/to/xiangjiandao-core

# 更新 Helm 仓库
helm repo update

# 安装或更新 Helm-Chart 包
helm upgrade --install $HELM_DEPLOY_NAME $PROGRAM_NAME/$CHART_NAME --version $CHART_VERSION --namespace=xiangjiandao -f $VALUES --set env.envName=$GRAY_NAME --kubeconfig=$KUBE_CONFIG
```

## 项目配置

项目配置保存在项目根目录的 `values.yaml` 文件的 `appconfig` 一节中, 具体说明见下。

> 下面的配置不是标准的 json 代码，标准的 json 不支持注释，如果需要修改配置，请直接到 `values.yaml` 中进行修改
 
```json5
{
  // 基础配置
  "App": {
    "UseSkyAPM": false,
    "UseFeiShu": false,
    "SkyAPMWriteData": true,
    "OfficialWebsite": "xiangjiandao.example.com", // 官方网站地址，用于邮件模板中跳转到官网
    "OfficialEmail": "xiangjiandao@example.com" // 官方联系邮箱，用于在邮件模板中，联系官方邮箱
  },
  // 飞书配置，用于飞书认证访问 devops 开发人员相关的页面, 如果需要，请参考飞书相关文档
  "Feishu": {
    "ClientId": "client-id",
    "ClientSecret": "example-client-secret"
  },
  "ConnectionStrings": {
    // 数据库配置，请替换成正确的域名以及用户名,密码等
    "MySql": "Server=127.0.0.1;Port=3306;User ID=root;Password=example-password;Database=xiangjiandao;SslMode=none"
  },
  // Redis 配置
  "Redis": {
    "Host": "127.0.0.1",
    "Port": 6379,
    "Database": 1,
    "Password": "example-password"
  },
  // RabbitMQ 配置
  "RabbitMQ": {
    "HostName": "example.com",
    "Port": 5672,
    "UserName": "example-user-name",
    "Password": "example-password",
    "VirtualHost": "xiangjiandao"
  },
  // Jwt 配置
  "Jwt": {
    "SecretKey": "example-secret-key", // 使用 openssl 生成的私钥
    "iss": "xiangjiandao",
    "Audience": "account",
    "ExpirationInMinutes": 43200,
    "Kid": "example-kid" // 与私钥配对的 kid
  },
  // 邮箱配置，用于邮件验证码
  "Email": {
    "SenderName": "example-sender-name", // 发送者名称
    "SenderAddress": "xiangjiandao@example.com", // 发送者邮箱地址
    "Subject": "example-subject", // 邮件主题
    "EmailServerHost": "example.email.host.com", // 邮箱服务器地址
    "EmailServerPort": 465, // 邮箱服务器端口
    "UserName": "example-user-name", // 用户名称
    "Password": "example-password", // 用户密码
    "SecureSocketOptions": 1
  },
  // 徽章以及稻米发放的 excel 模板配置
  "Template": {
    "MedalDistribution": "file-id", // 这是一个 fileId, 通过文件服务上传 excel 模板文件，将会获得一个 fileId, 下同
    "ScoreDistribution": "file-id"
  },
  // Exceptionless 配置，默认关闭
  "Exceptionless": {
    "Enable": false
  },
  // 阿里云短信服务，具体配置请参考阿里云相关文档
  "AliYunSms":{
    "AccessKeyId": "example-access-key-id",
    "AccessKeySecret" : "example-key-secret",
    "SignName": "example-sign-name",
    "TemplateCode": "example-template-code",
    "Endpoint": "example.endpoint.com"
  },
  // 阿里云内容审核服务
  "AliYunModeration": {
    "EnableTextModeration": false, // 是否开启贴文文本审核
    "EnableImageModeration": false, // 是否开启贴文图片审核
    "AccessKeyId": "example-access-key", // 服务 AccessKey
    "AccessKeySecret" : "example-access-key-secret", // 服务 AccessKeySecret
    "Endpoint": "example.endpint.com", // Endpoint 就近选取
    "RegionId": "cn-hangzhou", // 和 Endpoint 匹配
    "ImageCallbackUrl": "https://example.domain.com/api/v1/external/image/" // 如果开启图片审核，需要配置该回调地址，允许外网访问，请将 example.domain.com 配置为服务的域名，并保证可被外网访问
  },
  // BlueSky 相关配置
  "BlueSky":{
    "PdsDomain": "https://web5.example.com", // Pds 域名
    "BskyDomain": "https://bsky.example.com", // Bsky 域名
    "PlcDomain": "https://plc.example.com", // Plc 域名
    "PostDomain": "https://post.example.com", // Post 域名
    "EmailDomain": "web5.example.com", // 注册用户默认的邮箱域名配置
    "AdminToken": "example-admin-token" // BlueSky 管理员的 AccessToken
  },
  // 反向代理配置，将所有 BlueSky 的请求转发到 BlueSky 相关的服务
  "ReverseProxy": {
    "Routes": {
      "Bsky" : {
        "ClusterId": "bsky",
        "Match": {
          "Path": "/bsky/xrpc/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/xrpc/{**catch-all}" }
        ]
      },
      "Pds" : {
        "ClusterId": "pds",
        "Match": {
          "Path": "/pds/xrpc/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/xrpc/{**catch-all}" }
        ]
      },
      "Plc" : {
        "ClusterId": "plc",
        "Match": {
          "Path": "/plc/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/xrpc/{**catch-all}" }
        ]
      },
      "Post" : {
        "ClusterId": "post",
        "Match": {
          "Path": "/post/api/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/{**catch-all}" }
        ]
      }
    },
    "Clusters": {
      "bsky": {
        "Destinations": {
          "bsky1": {
            "Address": "https://bsky.example.com" // Bsky 域名
          }
        }
      },
      "pds": {
        "Destinations": {
          "pds1": {
            "Address": "https://web5.example.com" // Pds 域名
          }
        }
      },
      "plc": {
        "Destinations": {
          "plc1": {
            "Address": "https://plc.example.com" // Plc 域名
          }
        }
      },
      "post": {
        "Destinations": {
          "post1": {
            "Address": "https://post.example.com" // Post 域名
          }
        }
      }
    }
  }
}
```

## 关于监控

这里使用了`prometheus-net`作为与基础设施prometheus集成的监控方案，默认通过地址 `/metrics` 输出监控指标。

更多信息请参见：[https://github.com/prometheus-net/prometheus-net](https://github.com/prometheus-net/prometheus-net)
