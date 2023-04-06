using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new StopRotatingBlock(anchorController);
    }
}
