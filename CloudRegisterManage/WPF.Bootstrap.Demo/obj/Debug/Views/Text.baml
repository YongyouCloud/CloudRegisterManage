   M S B A M L   `   `   `  ����  CloudRegisterManage#    WPF.Bootstrap.Demo.Views.Text   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      H DWPF.Bootstrap, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null4	bootstrap$http://github.com/scheshan/bootstrap  5      .��5	      �� .+�5   
   ��    	BgPrimary#��[  6   $��10q�69     �� Padding$  10q�6E   
�������._�5      �� $��
bg-primary��6   5      �� 5   
    
 BgInfo#��[ 6   $��10q�66   $  10q�6B   ���#333._�5      �� $��bg-info��6   5      �� 5   
     	BgSuccess#��[ 6   $��10q�69   $  10q�6E   ���#333._�5      �� $��
bg-success��6   5      �� 5   
     	BgWarning#��[ 6   $��10q�69   $  10q�6E   ���#333._�5      �� $��
bg-warning��6   5      �� 5   
     BgDanger#��[ 6   $��10q�68   $  10q�6D   ���#333._�5      �� $��	bg-danger��6   5      �� 5   
   $��10q�6   $��textbox��6   6/   �� 5!   
   $��10q�6   {�.  $��textbox disabled��60   6J   �� 5#   
   $��10q�6    �� 
IsReadOnly . $��textbox readonly��60   6J   � 5%   
   $��10q�6    � ColumnDefinitions 5&      �� 5'      �� 5(      �� 5)      �� 5*      �� 5+      �� 5,      .+�5/      ��   	TextMuted#��[ 6   $��Center=�6@   muted6d   �� 50        TextInfo#��[ 6   $��Center=�6?   $��1��6\   info6r   �� 51        TextPrimary#��[ 6   $��Center=�6B   $��2��6_   	primary6x   �� 52        TextSuccess#��[ 6   $��Center=�6B   $��3��6_   	success6x   �� 53       	 TextWarning#��[	 6   $��Center=�6B   $��4��6_   	warning6x   �� 54       
 
TextDanger#��[
 6   $��Center=�6A   $��5��6^   danger6v   55      "  WPF.Bootstrap.Controls.Alert  57   
      Type$ Success=�6   $��10q�6)   .��58      �� �� 59      $�
Well done!��6   $
��Bold"�6(    �� 5:      $9�3You successfully read this important alert message.��6   5;      5<        5>   
   $
 Info=�6   $��10q�6&   .��5?      �� �� 5@      $�	Heads up!��6   $
��Bold"�6'    �� 5A      $D�>This alert needs your attention, but it's not super important.��6   5B      5C        5E   
   $ Warning=�6   $��10q�6)   .��5F      �� �� 5G      $�Warning!��6   $
��Bold"�6&    �� 5H      $9�3Better check yourself, you're not looking too good.��6   5I      5J        5L   
   $ Danger=�6   $��10q�6(   .��5M      �� �� 5N      $�Oh snap!��6   $
��Bold"�6&    �� 5O      $6�0Change a few things up and try submitting again.��6   5P      5Q      5R      5S      