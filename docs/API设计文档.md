# EzChat Sharp - API 设计文档

## 概述

EzChat Sharp 使用基于 TCP Socket 的自定义协议进行客户端-服务器通信。所有消息均采用 JSON 格式，支持异步双向通信。

## 通信协议

### 消息格式

#### 基础消息结构
```json
{
  "type": "MessageType",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "uuid-string",
  "data": {
    // 消息具体内容
  }
}
```

#### 字段说明
- `type`: 消息类型，枚举值
- `timestamp`: ISO 8601 格式的时间戳
- `messageId`: 唯一消息标识符
- `data`: 消息载荷，根据消息类型变化

### 消息类型定义

## 客户端到服务器消息

### 1. 用户登录 (LOGIN)

#### 请求
```json
{
  "type": "LOGIN",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-001",
  "data": {
    "username": "john_doe",
    "clientVersion": "1.0.0"
  }
}
```

#### 字段说明
- `username`: 用户名，3-20个字符，只能包含字母、数字、下划线
- `clientVersion`: 客户端版本号

#### 响应
成功：
```json
{
  "type": "LOGIN_SUCCESS",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-002",
  "data": {
    "userId": "user-123",
    "username": "john_doe",
    "serverVersion": "1.0.0",
    "defaultRoom": "general",
    "serverTime": "2024-01-01T12:00:01.000Z"
  }
}
```

失败：
```json
{
  "type": "LOGIN_FAILED",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-002",
  "data": {
    "errorCode": "USERNAME_TAKEN",
    "errorMessage": "用户名已被使用",
    "retryAllowed": true
  }
}
```

#### 错误码
- `USERNAME_TAKEN`: 用户名已被使用
- `INVALID_USERNAME`: 用户名格式无效
- `SERVER_FULL`: 服务器已满
- `BANNED_USER`: 用户被禁止

### 2. 用户登出 (LOGOUT)

#### 请求
```json
{
  "type": "LOGOUT",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-003",
  "data": {
    "reason": "USER_QUIT"
  }
}
```

#### 字段说明
- `reason`: 登出原因（USER_QUIT, CONNECTION_LOST, KICKED）

### 3. 加入聊天室 (JOIN_ROOM)

#### 请求
```json
{
  "type": "JOIN_ROOM",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-004",
  "data": {
    "roomName": "general",
    "password": "optional-password"
  }
}
```

#### 响应
成功：
```json
{
  "type": "JOIN_ROOM_SUCCESS",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-005",
  "data": {
    "roomName": "general",
    "roomId": "room-001",
    "memberCount": 5,
    "maxMembers": 50,
    "roomTopic": "欢迎来到通用聊天室"
  }
}
```

### 4. 离开聊天室 (LEAVE_ROOM)

#### 请求
```json
{
  "type": "LEAVE_ROOM",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-006",
  "data": {
    "roomName": "general"
  }
}
```

### 5. 发送消息 (SEND_MESSAGE)

#### 请求
```json
{
  "type": "SEND_MESSAGE",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-007",
  "data": {
    "content": "Hello, everyone!",
    "targetRoom": "general",
    "messageType": "TEXT",
    "replyToMessageId": "msg-005" // 可选，回复消息
  }
}
```

#### 字段说明
- `content`: 消息内容，最大1000字符
- `targetRoom`: 目标聊天室
- `messageType`: 消息类型（TEXT, EMOJI, SYSTEM）
- `replyToMessageId`: 回复的消息ID（可选）

### 6. 获取聊天室列表 (LIST_ROOMS)

#### 请求
```json
{
  "type": "LIST_ROOMS",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-008",
  "data": {
    "includeEmpty": false,
    "sortBy": "memberCount" // memberCount, name, createdAt
  }
}
```

### 7. 获取用户列表 (LIST_USERS)

#### 请求
```json
{
  "type": "LIST_USERS",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-009",
  "data": {
    "roomName": "general", // 可选，指定房间
    "onlineOnly": true
  }
}
```

### 8. 创建聊天室 (CREATE_ROOM)

#### 请求
```json
{
  "type": "CREATE_ROOM",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-010",
  "data": {
    "roomName": "my-room",
    "roomTopic": "我的私人聊天室",
    "maxMembers": 10,
    "isPrivate": false,
    "password": "optional-password"
  }
}
```

### 9. 私聊消息 (PRIVATE_MESSAGE)

#### 请求
```json
{
  "type": "PRIVATE_MESSAGE",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-011",
  "data": {
    "targetUserId": "user-456",
    "content": "Hello there!",
    "messageType": "TEXT"
  }
}
```

### 10. 心跳包 (HEARTBEAT)

#### 请求
```json
{
  "type": "HEARTBEAT",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "messageId": "msg-012",
  "data": {
    "clientStatus": "ACTIVE"
  }
}
```

## 服务器到客户端消息

### 1. 登录成功 (LOGIN_SUCCESS)
已在上面定义

### 2. 登录失败 (LOGIN_FAILED)
已在上面定义

### 3. 接收消息 (MESSAGE_RECEIVED)

```json
{
  "type": "MESSAGE_RECEIVED",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-013",
  "data": {
    "messageId": "msg-007",
    "senderId": "user-123",
    "senderName": "john_doe",
    "content": "Hello, everyone!",
    "roomName": "general",
    "messageType": "TEXT",
    "timestamp": "2024-01-01T12:00:00.000Z",
    "replyToMessageId": "msg-005"
  }
}
```

### 4. 用户加入通知 (USER_JOINED)

```json
{
  "type": "USER_JOINED",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-014",
  "data": {
    "userId": "user-456",
    "username": "jane_doe",
    "roomName": "general",
    "joinTime": "2024-01-01T12:00:01.000Z"
  }
}
```

### 5. 用户离开通知 (USER_LEFT)

```json
{
  "type": "USER_LEFT",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-015",
  "data": {
    "userId": "user-456",
    "username": "jane_doe",
    "roomName": "general",
    "leaveTime": "2024-01-01T12:00:01.000Z",
    "reason": "USER_QUIT"
  }
}
```

### 6. 聊天室列表 (ROOM_LIST)

```json
{
  "type": "ROOM_LIST",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-016",
  "data": {
    "rooms": [
      {
        "roomId": "room-001",
        "roomName": "general",
        "roomTopic": "通用聊天室",
        "memberCount": 15,
        "maxMembers": 50,
        "isPrivate": false,
        "hasPassword": false,
        "createdAt": "2024-01-01T10:00:00.000Z"
      },
      {
        "roomId": "room-002",
        "roomName": "tech-talk",
        "roomTopic": "技术讨论",
        "memberCount": 8,
        "maxMembers": 20,
        "isPrivate": false,
        "hasPassword": true,
        "createdAt": "2024-01-01T11:00:00.000Z"
      }
    ],
    "totalCount": 2
  }
}
```

### 7. 用户列表 (USER_LIST)

```json
{
  "type": "USER_LIST",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-017",
  "data": {
    "roomName": "general",
    "users": [
      {
        "userId": "user-123",
        "username": "john_doe",
        "status": "ONLINE",
        "joinTime": "2024-01-01T11:30:00.000Z",
        "lastActivity": "2024-01-01T11:59:30.000Z"
      },
      {
        "userId": "user-456",
        "username": "jane_doe",
        "status": "AWAY",
        "joinTime": "2024-01-01T11:45:00.000Z",
        "lastActivity": "2024-01-01T11:55:00.000Z"
      }
    ],
    "totalCount": 2
  }
}
```

### 8. 私聊消息 (PRIVATE_MESSAGE_RECEIVED)

```json
{
  "type": "PRIVATE_MESSAGE_RECEIVED",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-018",
  "data": {
    "messageId": "msg-011",
    "senderId": "user-456",
    "senderName": "jane_doe",
    "content": "Hello there!",
    "messageType": "TEXT",
    "timestamp": "2024-01-01T12:00:00.000Z"
  }
}
```

### 9. 错误消息 (ERROR)

```json
{
  "type": "ERROR",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-019",
  "data": {
    "errorCode": "ROOM_NOT_FOUND",
    "errorMessage": "指定的聊天室不存在",
    "originalMessageId": "msg-004",
    "retryAllowed": false,
    "suggestedAction": "LIST_ROOMS"
  }
}
```

#### 常见错误码
- `ROOM_NOT_FOUND`: 聊天室不存在
- `ROOM_FULL`: 聊天室已满
- `PERMISSION_DENIED`: 权限不足
- `INVALID_MESSAGE_FORMAT`: 消息格式无效
- `MESSAGE_TOO_LONG`: 消息过长
- `RATE_LIMIT_EXCEEDED`: 发送频率过高
- `USER_NOT_FOUND`: 用户不存在
- `DUPLICATE_USERNAME`: 用户名重复

### 10. 服务器关闭通知 (SERVER_SHUTDOWN)

```json
{
  "type": "SERVER_SHUTDOWN",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-020",
  "data": {
    "reason": "MAINTENANCE",
    "message": "服务器将在5分钟后关闭进行维护",
    "shutdownTime": "2024-01-01T12:05:00.000Z",
    "estimatedDowntime": "PT30M" // ISO 8601 duration
  }
}
```

### 11. 心跳响应 (HEARTBEAT_ACK)

```json
{
  "type": "HEARTBEAT_ACK",
  "timestamp": "2024-01-01T12:00:01.000Z",
  "messageId": "msg-021",
  "data": {
    "serverTime": "2024-01-01T12:00:01.000Z",
    "serverStatus": "HEALTHY",
    "connectedUsers": 25
  }
}
```

## 连接流程

### 1. 建立连接
```
Client                    Server
  │                         │
  │──── TCP Connect ────────►│
  │◄──── TCP Accept ────────│
  │                         │
```

### 2. 用户认证
```
Client                    Server
  │                         │
  │──── LOGIN ──────────────►│
  │                         │ (验证用户名)
  │◄─── LOGIN_SUCCESS ──────│
  │                         │
```

### 3. 加入默认聊天室
```
Client                    Server
  │                         │
  │──── JOIN_ROOM ──────────►│ (general)
  │                         │
  │◄─── JOIN_ROOM_SUCCESS ──│
  │◄─── USER_JOINED ────────│ (广播给其他用户)
  │                         │
```

### 4. 正常通信
```
Client A                  Server                  Client B
  │                         │                         │
  │──── SEND_MESSAGE ──────►│                         │
  │                         │──── MESSAGE_RECEIVED ──►│
  │◄─── MESSAGE_RECEIVED ───│                         │
  │                         │                         │
```

### 5. 断开连接
```
Client                    Server
  │                         │
  │──── LOGOUT ─────────────►│
  │                         │ (清理用户状态)
  │◄─── USER_LEFT ──────────│ (广播给其他用户)
  │                         │
  │──── TCP Close ──────────►│
  │                         │
```

## 消息限制和约束

### 消息大小限制
- 单条消息最大: 64KB
- 消息内容最大: 1000字符
- 用户名长度: 3-20字符
- 聊天室名长度: 3-30字符

### 频率限制
- 普通消息: 每秒最多5条
- 私聊消息: 每秒最多3条
- 命令消息: 每秒最多2条
- 心跳包: 每30秒1次

### 连接限制
- 单个IP最大连接数: 5
- 服务器最大连接数: 1000
- 连接超时时间: 30秒
- 空闲超时时间: 300秒

## 错误处理

### 网络错误
- 连接断开: 客户端自动重连
- 超时: 重发消息（最多3次）
- 格式错误: 返回ERROR消息

### 业务错误
- 权限不足: 返回PERMISSION_DENIED
- 资源不存在: 返回NOT_FOUND错误
- 参数无效: 返回INVALID_PARAMETER错误

### 重试机制
```json
{
  "retryPolicy": {
    "maxRetries": 3,
    "retryDelay": "PT1S", // 1秒
    "backoffMultiplier": 2.0,
    "maxRetryDelay": "PT30S" // 最大30秒
  }
}
```

## 安全考虑

### 输入验证
- 所有字符串字段进行长度检查
- 特殊字符过滤和转义
- JSON格式验证
- 消息类型枚举验证

### 防护措施
- 频率限制防止刷屏
- 连接数限制防止DoS
- 消息大小限制防止内存攻击
- 用户名唯一性检查

### 日志记录
- 记录所有连接和断开事件
- 记录所有错误和异常
- 记录可疑活动（频繁重连、大量错误等）

---

*本API文档将随着功能开发持续更新*