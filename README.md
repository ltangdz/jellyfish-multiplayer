**合成大水母 联机版**

原项目github网址：https://github.com/small-lizi/jellyfish



# 环境及配置

1. unity版本：2022.3.57
2. 联机方案，netcode + lobby + relay，使用状态同步（需分别安装对应package）



# 改动

1. 重置UI布局，对变量名规范命名

2. UIManager解耦，新增SettingUI（音量调节，主菜单按钮等）和MainUI（操作按钮，分数显示，设置按钮等等）把UIManager改成对这些UI的引用，ScnMenu改名成MainMenuUI

3. TextMeshPro扩展（原项目用的老版text）

4. 把一些类改成单例类
   eg.

   1. GameManager（原作者为什么已经设置单例类了 还要在其他类去new。。？），并且在GameManager中增加游戏状态枚举
   2. SceneController主要负责水母生成，见名知意原则，改名成SpawnManager
   3. MechanicalArmController，因为游戏里只有一个机械爪，但是如果是多人游戏就会不止一个，所以不改成单例类也行
   4. GameAnimator主要负责动画效果，见名知意原则，改名成AnimateManager

5. Settings全局静态设置类，存放一些全局变量或者常量（PlayerPrefs键值，场景名称，）

6. EventSystem 全局事件管理系统，把各个类中的事件创建，订阅以及取消订阅等操作封装

7. 把一些数据用ScriptableObject统一管理，例如AudioClipRefsSO，JellyfishSO

   注意！！！ScriptableObject一定要放在**Resources** 文件夹下，不然打包后的游戏会出问题！！！

8. 新的InputSystem，代替旧输入系统

9. 水母类JellyfishController，改名Jellyfish，并且删除了对音效文件的引用，使用享元模式重构，每个水母根据自己的等级，在AudioClipRefsSO中找到对应的音效

10. 把Die.cs更名为DeadArea，对死亡逻辑处理进行优化

11. 删除大量冗余代码，进行大量代码重构



# 计划

1. 增加技能：晃瓶子
2. 