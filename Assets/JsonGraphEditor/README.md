# Unity JSON Graph Editor

一个基于GraphView的Unity JSON编辑器，支持可视化编辑JSON数据并保存为JSON文件。

## 功能特性

- 🎨 **可视化编辑**：使用GraphView节点系统可视化编辑JSON数据
- 📁 **加载/保存**：支持从文件加载JSON并保存编辑结果
- 🔗 **节点连接**：通过连接节点来建立JSON结构关系
- 📝 **多种类型**：支持Object、Array、String、Number、Boolean、Null等JSON类型
- 🎯 **直观界面**：清晰的编辑器界面，易于上手

## 安装

1. 将本仓库克隆或下载到Unity项目的Assets文件夹中
2. 在Unity编辑器中会自动加载脚本

## 使用方法

### 打开编辑器

在Unity编辑器菜单栏选择：`Window > Json Graph Editor`

### 基本操作

#### 1. 加载JSON文件
- 在"File Path"输入框中输入JSON文件的完整路径
- 点击"Load JSON"按钮加载文件
- 编辑器会自动解析JSON并创建对应的节点

#### 2. 创建节点
点击以下按钮在编辑区域创建不同类型的节点：
- **Add Object**：创建对象节点
- **Add Array**：创建数组节点
- **Add String**：创建字符串节点
- **Add Number**：创建数字节点
- **Add Boolean**：创建布尔值节点

#### 3. 编辑节点
- 选中节点后，可在节点上编辑Key和Value
- 对于容器类型（Object、Array），通过连接其他节点作为子元素

#### 4. 连接节点
- 拖拽节点的输出端口连接到另一个节点的输入端口
- 这样可以建立父子关系

#### 5. 保存JSON文件
- 在"File Path"输入框中指定保存路径
- 点击"Save JSON"按钮
- 编辑器会将图表结构转换为JSON并保存

## 文件结构

```
Assets/JsonGraphEditor/
├── Editor/
│   ├── JsonGraphEditorWindow.cs      # 主编辑器窗口
│   ├── JsonGraphView.cs              # 图表视图
│   ├── JsonNodeView.cs               # 节点视图
│   └── JsonSerializer.cs             # JSON序列化/反序列化
└── Resources/
    └── JsonGraphView.uss             # 样式表
```

## 脚本说明

### JsonGraphEditorWindow.cs
主编辑器窗口类，负责：
- 创建编辑器UI界面
- 处理加载/保存操作
- 管理节点的添加和删除

### JsonGraphView.cs
继承自GraphView的自定义图表视图，负责：
- 管理图表的缩放和平移
- 处理节点创建和连接
- 提供图表编辑功能

### JsonNodeView.cs
继承自Node的自定义节点类，负责：
- 显示节点的Key和Value
- 提供输入输出端口
- 支持不同的JSON数据类型

### JsonSerializer.cs
JSON序列化工具类，负责：
- 将图表节点转换为JSON字符串
- 解析JSON文件并创建对应节点
- 处理字符串转义等细节

## 注意事项

- 请确保文件路径是绝对路径或相对于项目根目录的路径
- 加载JSON文件时，编辑器会清空当前的所有节点
- 保存前请确保所有节点都正确连接

## 许可证

MIT License

## 贡献

欢迎提交Issue和Pull Request！
