   M S B A M L   `   `   `  ����  CloudRegisterManage$    WPF.Bootstrap.Demo.Views.Panel   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      H DWPF.Bootstrap, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null4	bootstrap$http://github.com/scheshan/bootstrap  5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   +  %WPF.Bootstrap.Controls.BootstrapPanel  $��10q�6#      Type$ Default=�6/   $��Default Panel��6>   .��5      �� 	content6!      Footer 5      �� 5      footer6$   5        5   
   $��10q�6#   $��1��6/   $ Primary=�6?   $��Primary Panel��6N   .��5      �� 	content6!    5       �� 5!      footer6$   5#        5%   
   $
 Info=�6#   $��10q�6/   $��1��6;   $��
Info Panel��6H   .��5&      �� 	content6!    5'      �� 5(      footer6$   5*        5,   
   $ Success=�6#   $��10q�62   $��1��6>   $��1��6K   $��Success Panel��6[   .��5-      �� 	content6!    5.      �� 5/      footer6$   51        53   
   $ Warning=�6#   $��10q�62   $��2��6>   $��Warning Panel��6K   .��54      �� 	content6!    55      �� 56      footer6$   58        5:   
   $ Danger=�6#   $��10q�61   $��2��6=   $��1��6J   $��Danger Panel��6Z   .��5;      �� 	content6!    5<      �� 5=      footer6$   5?        5A   
   $��10q�6#   $��3��6/   $��Default Panel��6<   .��5B      �� panel with no footer6.   5C        5E   
   $ Danger=�6#   $��10q�61   $��3��6=   $��1��6J   .��5F      ��  panel with no title and footer68   5G      5H      5I      