using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new StartRotatingBlock(anchorController);
    }
}
