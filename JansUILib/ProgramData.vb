﻿Class ProgramData
    Public Shared Property BusinessName = ""
    Public Shared Property ProgramVersion = "[DEV BUILD]"
    Public Shared ReadOnly UIfont = New Font("Consolas", 16.0!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
    Public Shared ReadOnly UIFontSmall = New Font("Consolas", 12.0!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
    Public Shared ReadOnly UserDataPath = ".\UserData.accdb"
    Public Shared ReadOnly MenuDataPath = ".\Menu.accdb"
    Public Shared ReadOnly CustomerDataPath = ".\CustomerData.accdb"
End Class