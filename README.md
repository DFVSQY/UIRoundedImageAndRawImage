# 项目简介


本项目扩展了Unity游戏引擎中的UI组件:`Image` 和 `RawImage`，使其可以生成四角圆润的网格，每个角的圆润程度可以单独设置。

使用扩展后的 `RoundedImage` 和 `RoundedRawImage` 组件，可以有以下优势：

* 减少项目中 `Mask` 组件的使用，从而优化项目性能。

* 减少纹理中的Alpha通道，可优化纹理大小和运行时内存大小。

* 支持UI的默认材质，可以便于UI系统的合批操作，从而优化DrawCalls的数量。