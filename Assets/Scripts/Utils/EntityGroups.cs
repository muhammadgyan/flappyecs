using System.Collections;
using System.Collections.Generic;
using Svelto.ECS;
using UnityEngine;

public static class ECSGroups {
    public static readonly ExclusiveGroupStruct PlayersGroup = new ExclusiveGroup();
    public static readonly ExclusiveGroupStruct ObstacleGroup = new ExclusiveGroup();
    public static readonly ExclusiveGroupStruct DeadObstacleGroup = new ExclusiveGroup();
    public static readonly ExclusiveGroupStruct GameManagerGroup = new ExclusiveGroup();
    public static readonly ExclusiveGroupStruct HUDGroup = new ExclusiveGroup();
}
