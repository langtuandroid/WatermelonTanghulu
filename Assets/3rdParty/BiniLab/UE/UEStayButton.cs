using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEStayButton : UEButton
{
    private Action<bool> _stayAction = null;

    public void SetStayAction(Action<bool> action)
    {
        _stayAction = action;
    }
    
    protected override void Update() 
    {
        base.Update();

        // if (ButtonPressTime >= NumberConst.BUTTON_LONG_PRESS_TIME && _stayAction != null)
        //     _stayAction.Invoke(ButtonPressed);
        
        if (!ButtonPressed)
        {
            ResetButtonPressedTime();
        }
    }
}
